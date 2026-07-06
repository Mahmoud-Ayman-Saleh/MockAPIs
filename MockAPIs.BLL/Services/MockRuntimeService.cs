using System.Text.Json;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Exceptions;
using MockAPIs.BLL.Interfaces;
using MockAPIs.DAL.Interfaces;
using MockAPIs.DAL.Models;
using MockAPIs.DAL.Repositories.Interfaces;

namespace MockAPIs.BLL.Services
{
    public class MockRuntimeService : IMockRuntimeService
    {
        private readonly IMockRuntimeRepository runtimeRepository;

        public MockRuntimeService(IMockRuntimeRepository _runtimeRepository)
        {
            runtimeRepository = _runtimeRepository;
        }

        public async Task<object> GetListAsync(
            string token,
            string resourceSlug,
            string? search,
            int? page,
            int? limit)
        {
            var resource = await ResolveResourceAsync(token, resourceSlug);

            // check GET list is enabled for this resource
            if (!resource.EndpointConfig.GetList)
                throw new MethodNotAllowedException("GET list method is not enabled for this resource");

            var records = await runtimeRepository.GetAllRecords(resource.Id);

            // deserialize all records from JSON string to dictionary
            var parsed = records
                .Select(r => DeserializeRecord(r.Id, r.Data))
                .ToList();

            // apply search if enabled and search param provided
            if (resource.EndpointConfig.EnableSearch && !string.IsNullOrWhiteSpace(search))
            {
                parsed = ApplySearch(parsed, search);
            }

            // apply pagination if enabled and page param provided
            if (resource.EndpointConfig.EnablePagination && page.HasValue)
            {
                var pageNum = page.Value < 1 ? 1 : page.Value;
                var pageSize = limit ?? 10;
                var total = parsed.Count;
                var totalPages = (int)Math.Ceiling((double)total / pageSize);

                var paged = parsed
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PaginatedResponseDto
                {
                    Data = paged,
                    Page = pageNum,
                    Limit = pageSize,
                    Total = total,
                    TotalPages = totalPages
                };
            }

            // no pagination — return flat list
            return parsed;
        }

        public async Task<Dictionary<string, object?>> GetByIdAsync(
            string token,
            string resourceSlug,
            Guid recordId)
        {
            var resource = await ResolveResourceAsync(token, resourceSlug);

            if (!resource.EndpointConfig.GetById)
                throw new MethodNotAllowedException("GET by id method is not enabled for this resource");

            var record = await runtimeRepository.GetRecordById(resource.Id, recordId);

            if (record == null)
                throw new NotFoundException("Record not found");

            return DeserializeRecord(record.Id, record.Data);
        }

        public async Task<Dictionary<string, object?>> CreateRecordAsync(
            string token,
            string resourceSlug,
            Dictionary<string, object?> body)
        {
            var resource = await ResolveResourceAsync(token, resourceSlug);

            if (!resource.EndpointConfig.Post)
                throw new MethodNotAllowedException("POST method is not enabled for this resource");

            // generate a new id for this record
            var newId = Guid.NewGuid();

            // inject the id into the body
            body["id"] = newId.ToString();

            var record = new MockRecord
            {
                Id = newId,
                ResourceId = resource.Id,
                Data = JsonSerializer.Serialize(body),
                CreatedAt = DateTime.UtcNow
            };

            await runtimeRepository.AddRecord(record);
            await runtimeRepository.SaveChanges();

            return body;
        }

        public async Task<Dictionary<string, object?>> UpdateRecordAsync(
            string token,
            string resourceSlug,
            Guid recordId,
            Dictionary<string, object?> body)
        {
            var resource = await ResolveResourceAsync(token, resourceSlug);

            if (!resource.EndpointConfig.Put)
                throw new MethodNotAllowedException("PUT method is not enabled for this resource");

            var record = await runtimeRepository.GetRecordById(resource.Id, recordId);

            if (record == null)
                throw new NotFoundException("Record not found");

            // deserialize existing data so we can merge with new body
            var existing = DeserializeRecord(record.Id, record.Data);

            // merge — new body values overwrite existing ones
            // fields not in the body stay as they were
            foreach (var key in body.Keys)
            {
                existing[key] = body[key];
            }

            // always keep id consistent
            existing["id"] = recordId.ToString();

            record.Data = JsonSerializer.Serialize(existing);

            await runtimeRepository.UpdateRecord(record);
            await runtimeRepository.SaveChanges();

            return existing;
        }

        public async Task DeleteRecordAsync(
            string token,
            string resourceSlug,
            Guid recordId)
        {
            var resource = await ResolveResourceAsync(token, resourceSlug);

            if (!resource.EndpointConfig.Delete)
                throw new MethodNotAllowedException("DELETE method is not enabled for this resource");

            var record = await runtimeRepository.GetRecordById(resource.Id, recordId);

            if (record == null)
                throw new NotFoundException("Record not found");

            await runtimeRepository.DeleteRecord(record);
            await runtimeRepository.SaveChanges();
        }

        // ── Private Helpers ────────────────────────────────────

        // resolves token → project → resource in every request
        // throws NotFoundException if anything in the chain is missing
        private async Task<Resource> ResolveResourceAsync(string token, string resourceSlug)
        {
            var project = await runtimeRepository.GetProjectByToken(token);

            if (project == null)
                throw new NotFoundException("Project not found");

            var resource = await runtimeRepository.GetResourceWithConfig(project.Id, resourceSlug);

            if (resource == null)
                throw new NotFoundException("Resource not found");

            return resource;
        }

        // deserializes JSONB string to dictionary
        // injects the real DB Guid as "id" to ensure consistency
        private Dictionary<string, object?> DeserializeRecord(Guid id, string json)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                       ?? new Dictionary<string, JsonElement>();

            // convert JsonElement values to plain objects
            var result = dict.ToDictionary(
                kvp => kvp.Key,
                kvp => ConvertJsonElement(kvp.Value)
            );

            // always inject the real DB id
            result["id"] = id.ToString();

            return result;
        }

        // converts JsonElement to plain C# types for clean JSON output
        private object? ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String  => element.GetString(),
                JsonValueKind.Number  => element.TryGetInt64(out var l) ? l : element.GetDouble(),
                JsonValueKind.True    => true,
                JsonValueKind.False   => false,
                JsonValueKind.Null    => null,
                JsonValueKind.Array   => element.EnumerateArray()
                                                .Select(ConvertJsonElement)
                                                .ToList(),
                JsonValueKind.Object  => element.EnumerateObject()
                                                .ToDictionary(p => p.Name, p => ConvertJsonElement(p.Value)),
                _                     => element.ToString()
            };
        }

        // searches all string values in every record for the search term
        private List<Dictionary<string, object?>> ApplySearch(
            List<Dictionary<string, object?>> records,
            string search)
        {
            var term = search.ToLower();

            return records.Where(record =>
                record.Values.Any(value =>
                    value?.ToString()?.ToLower().Contains(term) == true
                )
            ).ToList();
        }
    }
}
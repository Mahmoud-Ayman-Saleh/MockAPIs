using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Interfaces;
using MockAPIs.DAL.Interfaces;

namespace MockAPIs.BLL.Services
{
    public class EndpointConfigService : IEndpointConfigService
    {
        private readonly IEndpointConfigRepository endpointConfigRepository;
        private readonly IUnitOfWork unitOfWork;

        public EndpointConfigService(IEndpointConfigRepository _endpointConfigRepository, IUnitOfWork _unitOfWork)
        {
            endpointConfigRepository = _endpointConfigRepository;
            unitOfWork = _unitOfWork;
        }

        public async Task<EndpointConfigResponseDto> Update(
            Guid resourceId,
            UpdateEndpointConfigDto dto,
            Guid userId)
        {
            var resourceOwned = await endpointConfigRepository
                .IsResourceOwnedByUserAsync(resourceId, userId);

            if (!resourceOwned)
                throw new KeyNotFoundException("Resource not found");

            var config = await endpointConfigRepository.GetByResourceIdAsync(resourceId);

            if (config == null)
                throw new KeyNotFoundException("Endpoint config not found");

            config.GetList = dto.GetList;
            config.GetById = dto.GetById;
            config.Post = dto.Post;
            config.Put = dto.Put;
            config.Delete = dto.Delete;
            config.EnablePagination = dto.EnablePagination;
            config.EnableSearch = dto.EnableSearch;

            await unitOfWork.EndpointsConfig.Update(config);
            await unitOfWork.EndpointsConfig.SaveChanges();

            return EndpointConfigResponseDto.FromEntity(config);
        }
    }
}
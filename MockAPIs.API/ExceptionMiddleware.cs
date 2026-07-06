using MockAPIs.BLL.Exceptions;
using System.Net;
using System.Text.Json;

namespace MockAPIs.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                await WriteErrorResponse(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex.Message);
                await WriteErrorResponse(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex.Message);
                await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (MethodNotAllowedException ex)
            {
                _logger.LogWarning(ex.Message);
                await WriteErrorResponse(context, HttpStatusCode.MethodNotAllowed, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred");
                await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "Something went wrong");
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = JsonSerializer.Serialize(new { message });
            await context.Response.WriteAsync(response);
        }
    }
}
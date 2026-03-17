using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Models;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Exceptions;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAppLogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, IAppLogger<ExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            ValidationProblemDetails problem = new();

            switch(exception)
            {
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    _logger.LogWarning("Bad request: {0}", badRequestException.Message);
                    problem = new ValidationProblemDetails()
                    {
                        Status = (int)statusCode,
                        Title = badRequestException.Message,
                        Detail = badRequestException.InnerException?.Message,
                        Type = nameof(BadRequestException),
                        Errors = badRequestException.ValidationErrors
                    };
                    break;
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    _logger.LogWarning("Not found: {0}", notFoundException.Message);
                    problem = new ValidationProblemDetails()
                    {
                        Status = (int)statusCode,
                        Title = notFoundException.Message,
                        Detail = notFoundException.InnerException?.Message,
                        Type = nameof(NotFoundException)
                    };
                    break;
                default:
                    _logger.LogError("Unhandled exception: {0}", exception);
                    problem = new ValidationProblemDetails()
                    {
                        Status = (int)statusCode,
                        Title = exception.Message,
                        Detail = exception.StackTrace,
                        Type = nameof(HttpStatusCode.InternalServerError)
                    };
                    break;
            }
            
            context.Response.StatusCode = (int)statusCode;            
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
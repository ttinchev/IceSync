using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IceSync.Presentation.Api.Configuration
{
    /// <summary>
    /// Exception handler for the whole application.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">delegate.</param>
        /// <param name="logger">logger.</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Loggs error and returns appropriate response.
        /// </summary>
        /// <param name="context">The Http Context object.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = error switch
                {
                    KeyNotFoundException => (int)HttpStatusCode.NotFound, // not found error
                    Exception => (int)HttpStatusCode.BadRequest, // custom application error
                    _ => (int)HttpStatusCode.InternalServerError, // unhandled error
                };
                _logger.LogCritical($"Something went wrong: {error}");

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}

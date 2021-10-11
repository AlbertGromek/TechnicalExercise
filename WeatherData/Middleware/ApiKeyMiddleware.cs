using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace WeatherData
{
    public class ApiKeyMiddleware
    {
        private readonly IConfiguration _configuration;

        private readonly RequestDelegate _next;
        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _configuration = configuration;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized. No API Key provided.");
                return;
            }
            var keys = _configuration.GetValue<string>("APIKeys");
            var validApiKeys = _configuration.GetSection("APIKeys").Get<List<string>>();

            var match = validApiKeys.FirstOrDefault(validApiKeys => validApiKeys.Contains(extractedApiKey));
            if(match == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized API Key.");
                return;
            }
            await _next(context);
        }
    }
}
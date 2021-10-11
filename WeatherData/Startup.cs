using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AspNetCoreRateLimit;

namespace WeatherData
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services for rate limits (https://github.com/stefanprodan/AspNetCoreRateLimit)
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<ClientRateLimitOptions>
            (Configuration.GetSection("APIKeyLimit"));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitCounterStore,
            MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration,
            RateLimitConfiguration>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            services.AddControllers();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseClientRateLimiting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}

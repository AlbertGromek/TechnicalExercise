using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weather.Application.Configurations;
using Weather.Application.Interfaces;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Weather.Infrastructure.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(workerApp =>
    {
    })
    .ConfigureOpenApi() 
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<OpenWeatherServiceOptions>(
            context.Configuration.GetSection(OpenWeatherServiceOptions.SectionName));
        services.Configure<WeatherAIServiceOptions>(
            context.Configuration.GetSection(WeatherAIServiceOptions.SectionName));

        services.AddScoped<IWeatherService, OpenWeatherService>();
        services.AddScoped<IWeatherAIService, WeatherAIService>();
        services.AddHttpClient();
    })
    .Build();

host.Run();
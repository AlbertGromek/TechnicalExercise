using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weather.Application.Configurations;
using Weather.Application.Interfaces;
using Weather.Infrastructure;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

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

        services.AddScoped<IWeatherService, OpenWeatherService>();
        services.AddHttpClient();
    })
    .Build();

host.Run();

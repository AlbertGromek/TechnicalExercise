using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weather.Application.Configurations;
using Weather.Application.Interfaces;
using Weather.Infrastructure;

var builder = FunctionsApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Configuration.AddConfiguration(config);

builder.ConfigureFunctionsWebApplication();
builder.Services.Configure<OpenWeatherServiceOptions>(
    builder.Configuration.GetSection(OpenWeatherServiceOptions.SectionName));
builder.Services.AddScoped<IWeatherService, OpenWeatherService>();
builder.Services.AddHttpClient();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
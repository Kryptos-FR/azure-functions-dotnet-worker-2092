using Functions.Isolated;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddAzureAppConfiguration(options =>
        {
            options.Connect(Settings.AppConfigConnectionString).Select(KeyFilter.Any);
            if (!string.IsNullOrEmpty(Settings.AppConfigLabel))
                options.Select(KeyFilter.Any, Settings.AppConfigLabel);
            options.UseFeatureFlags();
        });
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.AddAzureAppConfiguration();
        services.AddLogging();
        services.ConfigureFunctionsApplicationInsights();
    })
    .ConfigureFunctionsWorkerDefaults(app =>
    {
        app.UseAzureAppConfiguration();
    })
    .Build();

await host.RunAsync();
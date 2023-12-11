using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Functions.InProcess.Startup))]

namespace Functions.InProcess;

internal sealed class Startup : FunctionsStartup
{
    public const string APPCONF_KEY = "appConfigConnectionString";

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
            .AddLogging()
            .BuildServiceProvider(true);

        var webJobBuilder = builder.Services.AddWebJobs(x => { return; });
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        var azureConnectionString = Environment.GetEnvironmentVariable(APPCONF_KEY);
        builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
        {
            options.Connect(azureConnectionString).Select(KeyFilter.Any);
            options.UseFeatureFlags();
        });
    }
}
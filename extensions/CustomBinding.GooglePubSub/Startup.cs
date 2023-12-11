using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CustomBinding.GooglePubSub.Startup))]

namespace CustomBinding.GooglePubSub;

class Startup : FunctionsStartup
{

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
            .AddOptions<BindingOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("gpubsub").Bind(settings);
                builder.Services.AddSingleton(settings);
            });

        builder.Services
            .AddLogging()
            .BuildServiceProvider(true);

        var wbBuilder = builder.Services.AddWebJobs(x => { return; });
        wbBuilder.AddGPubSubBinding(); // auto-registration
    }
}
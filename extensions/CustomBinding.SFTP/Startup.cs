using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CustomBinding.SFTP.Startup))]

namespace CustomBinding.SFTP;

internal sealed class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
            .AddLogging()
            .BuildServiceProvider(true);
            
        var wbBuilder = builder.Services.AddWebJobs(x => { return; });
        wbBuilder.AddSFTPBinding(); // auto-registration
    }
}

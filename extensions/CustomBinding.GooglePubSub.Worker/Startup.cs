using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Core;
using Microsoft.Extensions.DependencyInjection;

[assembly: WorkerExtensionStartup(typeof(CustomBinding.GooglePubSub.Worker.GPubSubExtensionStartup))]

namespace CustomBinding.GooglePubSub.Worker;

public sealed class GPubSubExtensionStartup : WorkerExtensionStartup
{
    public override void Configure(IFunctionsWorkerApplicationBuilder applicationBuilder)
    {        
        applicationBuilder.Services.Configure<WorkerOptions>(o =>
        {
            o.InputConverters.RegisterAt<GPubSubControllerConverter>(0);
        });
    }
}
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Core;
using Microsoft.Extensions.DependencyInjection;

[assembly: WorkerExtensionStartup(typeof(CustomBinding.SFTP.Worker.SFTPExtensionStartup))]

namespace CustomBinding.SFTP.Worker;

public sealed class SFTPExtensionStartup : WorkerExtensionStartup
{
    public override void Configure(IFunctionsWorkerApplicationBuilder applicationBuilder)
    {        
        applicationBuilder.Services.Configure<WorkerOptions>(o =>
        {
            o.InputConverters.RegisterAt<SFTPControllerConverter>(0);
        });
    }
}

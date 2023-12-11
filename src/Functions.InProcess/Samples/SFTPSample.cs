using System.Text;
using CustomBinding.SFTP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Functions.InProcess;

public sealed class SFTPSample
{
    private readonly ILogger _logger;

    public SFTPSample(ILogger<SFTPSample> logger)
    {
        _logger = logger;
    }

    [FunctionName(nameof(SFTPSample))]
    public async Task<IActionResult> ListFiles(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest request,
        [SFTPBinding(
            Host ="%sftp:host%",
            Port="%sftp:port%",
            Login="%sftp:login%",
            Password="%sftp:password%")] ISFTPController sftpController
    )
    {
        try
        {
            var input = await request.ReadAsStringAsync();
            var files = sftpController.ListFiles(input);

            var response = new StringBuilder();
            foreach (var file in files)
            {
                response.AppendLine($"Found file: {file.FullName}");
            }

            return new OkObjectResult(response.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when processing {nameof(ListFiles)} request");
            throw;
        }
    }
}

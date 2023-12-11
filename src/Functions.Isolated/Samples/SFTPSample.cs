using System.Net;
using System.Text;
using CustomBinding.SFTP;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Functions.Isolated;

public sealed class SFTPSample
{
    private readonly ILogger _logger;

    public SFTPSample(ILogger<SFTPSample> logger)
    {
        _logger = logger;
    }

    [Function(nameof(SFTPSample))]
    public async Task<HttpResponseData> ListFiles(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData request,
        [SFTPBinding(
            Host ="%sftp:host%", // issue: those aren't resolved if coming from Azure AppConfig
            Port="%sftp:port%",  // though it works from appSettings
            Login="%sftp:login%",
            Password="%sftp:password%")] ISFTPController sftpController,
        CancellationToken token)
    {
        try
        {
            var input = await request.ReadAsStringAsync();
            var files = sftpController.ListFiles(input);

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            foreach (var file in files)
            {
                await response.WriteStringAsync($"Found file: {file.FullName}\r\n", token);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when processing {nameof(ListFiles)} request");
            throw;
        }
    }
}

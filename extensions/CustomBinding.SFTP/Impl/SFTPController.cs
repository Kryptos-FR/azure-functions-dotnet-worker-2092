using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace CustomBinding.SFTP;

internal sealed class SFTPController : ISFTPController
{
    private readonly SFTPBindingAttribute _config;
    private readonly ILogger<SFTPBindingExtension> _logger;
    private readonly TelemetryClient _telemetryClient;

    internal static readonly Dictionary<string, SftpClient> _sftpClients = [];
    internal static readonly object _syncRoot = new();

    public SFTPController(SFTPBindingAttribute config, ILogger<SFTPBindingExtension> logger, TelemetryClient telemetryClient)
    {
        _config = config;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public void ClientDisconnection()
    {
        _logger.LogDebug(nameof(ClientDisconnection));

        // NOTE: mock implementation
        return;
    }

    public IEnumerable<SftpFile> ListFiles(string folder)
    {
        _logger.LogDebug(nameof(ListFiles));

        // NOTE: mock implementation
        return Enumerable.Empty<SftpFile>();
    }

    public string ReadFile(string folder, string fileName)
    {
        _logger.LogDebug(nameof(ReadFile));

        // NOTE: mock implementation
        return string.Empty;
    }

    public void WriteFile(string folder, string fileName, string content, Encoding encoding)
    {
        _logger.LogDebug(nameof(WriteFile));

        // NOTE: mock implementation
        return;
    }
}

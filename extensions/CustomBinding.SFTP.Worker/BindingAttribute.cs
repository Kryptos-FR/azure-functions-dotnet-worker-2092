using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

namespace CustomBinding.SFTP;

[AttributeUsage(AttributeTargets.Parameter)]
public class SFTPBindingAttribute : InputBindingAttribute
{
    public string Host { get; init; } = string.Empty;

    public string Port { get; init; } = "22"; // Adding default for backward compatibility

    public string Login { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string RsaKey { get; init; } = string.Empty;
}

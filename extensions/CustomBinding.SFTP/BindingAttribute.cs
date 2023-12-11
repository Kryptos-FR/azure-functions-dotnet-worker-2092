using Microsoft.Azure.WebJobs.Description;

namespace CustomBinding.SFTP;

[Binding]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class SFTPBindingAttribute : Attribute
{
    [AutoResolve]
    public string Host { get; set; }

    [AutoResolve]
    public string Port { get; set; } = "22"; // Adding default for backward compatibility

    [AutoResolve]
    public string Login { get; set; }

    [AutoResolve]
    public string Password { get; set; }

    [AutoResolve]
    public string RsaKey { get; set; }
}

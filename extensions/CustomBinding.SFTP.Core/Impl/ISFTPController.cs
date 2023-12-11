using System.Text;
using Renci.SshNet.Sftp;

namespace CustomBinding.SFTP;

public interface ISFTPController
{
    void ClientDisconnection();
    IEnumerable<SftpFile> ListFiles(string folder);
    string ReadFile(string folder, string fileName);
    void WriteFile(string folder, string fileName, string content, Encoding encoding);
}

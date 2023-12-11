pushd extensions
dotnet nuget push --source "C:\Projects\Packages" CustomBinding.GooglePubSub\bin\Debug\CustomBinding.GooglePubSub.1.2.3.4.nupkg
dotnet nuget push --source "C:\Projects\Packages" CustomBinding.SFTP\bin\Debug\CustomBinding.SFTP.1.2.3.4.nupkg
dotnet nuget push --source "C:\Projects\Packages" CustomBinding.SFTP.Core\bin\Debug\CustomBinding.SFTP.Core.1.2.3.4.nupkg
dotnet nuget push --source "C:\Projects\Packages" CustomBinding.SFTP.Worker\bin\Debug\CustomBinding.SFTP.Worker.1.2.3.4.nupkg
popd

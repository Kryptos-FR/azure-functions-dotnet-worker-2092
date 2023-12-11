pushd extensions
dotnet nuget push --source "C:\Projects\Packages" CustomBinding.GooglePubSub\bin\Debug\CustomBinding.GooglePubSub.1.2.3.4.nupkg
dotnet nuget push --source "C:\Projects\Packages" CustomBinding.SFTP\bin\Debug\CustomBinding.SFTP.1.2.3.4.nupkg
popd

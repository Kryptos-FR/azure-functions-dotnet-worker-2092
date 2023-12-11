pushd extensions
dotnet build CustomBinding.GooglePubSub.Core
dotnet build CustomBinding.GooglePubSub.Worker
dotnet build CustomBinding.GooglePubSub
dotnet build CustomBinding.SFTP.Core
dotnet build CustomBinding.SFTP.Worker
dotnet build CustomBinding.SFTP
popd

# note: this will fail the first time, until the above packages are published (see publish.ps1)
pushd src
dotnet build Functions.InProcess
dotnet build Functions.Isolated
popd

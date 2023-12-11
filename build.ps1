pushd extensions
dotnet build CustomBinding.GooglePubSub
dotnet build CustomBinding.SFTP
popd

pushd src
dotnet build Functions.InProcess
dotnet build Functions.Isolated
popd

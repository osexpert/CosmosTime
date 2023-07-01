set myKey=
set ver=0.0.1

nuget push bin\Packages\Release\NuGet\CosmosTime.%ver%.nupkg -s https://api.nuget.org/v3/index.json -k %myKey%
nuget push bin\Packages\Release\NuGet\CosmosTime.Serialization.Dapper.%ver%.nupkg -s https://api.nuget.org/v3/index.json -k %myKey%
nuget push bin\Packages\Release\NuGet\CosmosTime.Serialization.JsonNet.%ver%.nupkg -s https://api.nuget.org/v3/index.json -k %myKey%
nuget push bin\Packages\Release\NuGet\CosmosTime.Serialization.SystemTextJson.%ver%.nupkg -s https://api.nuget.org/v3/index.json -k %myKey%
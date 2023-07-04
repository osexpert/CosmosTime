REM set myKey=
set ver=0.0.1

nuget push bin\Packages\Release\NuGet\CosmosTime.%ver%.nupkg -src https://api.nuget.org/v3/index.json -ApiKey %myKey%
nuget push bin\Packages\Release\NuGet\CosmosTime.Serialization.Dapper.%ver%.nupkg -src https://api.nuget.org/v3/index.json -ApiKey %myKey%
nuget push bin\Packages\Release\NuGet\CosmosTime.Serialization.JsonNet.%ver%.nupkg -src https://api.nuget.org/v3/index.json -ApiKey %myKey%
nuget push bin\Packages\Release\NuGet\CosmosTime.Serialization.SystemTextJson.%ver%.nupkg -src https://api.nuget.org/v3/index.json -ApiKey %myKey%
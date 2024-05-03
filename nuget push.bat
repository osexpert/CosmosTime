REM set key=
REM set ver=

nuget push bin\Packages\Release\NuGet\CosmosTime.%ver%.nupkg -src https://api.nuget.org/v3/index.json -ApiKey %key%
nuget push bin\Packages\Release\NuGet\CosmosTime.Dapper.%ver%.nupkg -src https://api.nuget.org/v3/index.json -ApiKey %key%
nuget push bin\Packages\Release\NuGet\CosmosTime.JsonNet.%ver%.nupkg -src https://api.nuget.org/v3/index.json -ApiKey %key%
nuget push bin\Packages\Release\NuGet\CosmosTime.Json.%ver%.nupkg -src https://api.nuget.org/v3/index.json -ApiKey %key%
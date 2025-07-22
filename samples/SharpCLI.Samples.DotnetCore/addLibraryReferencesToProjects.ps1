dotnet build -c Release ../../src/SharpCLI/SharpCLI.csproj

echo "add reference to MinimalApi sample"
dotnet remove SharpCLI.Samples.DotnetCore.MinimalApi/SharpCLI.Samples.DotnetCore.MinimalApi.csproj reference ../../src/SharpCLI/SharpCLI.csproj
dotnet add SharpCLI.Samples.DotnetCore.MinimalApi/SharpCLI.Samples.DotnetCore.MinimalApi.csproj reference ../../src/SharpCLI/SharpCLI.csproj

echo "add reference to .NET Framework 4.7.2 sample"
dotnet remove SharpCLI.Samples.DotnetFramework472/SharpCLI.Samples.DotnetFramework472.csproj reference ../../src/SharpCLI/SharpCLI.csproj
dotnet add SharpCLI.Samples.DotnetFramework472/SharpCLI.Samples.DotnetFramework472.csproj reference ../../src/SharpCLI/SharpCLI.csproj

echo "Done"
dotnet clean
dotnet build --configuration Release
dotnet pack --configuration Release
dotnet nuget push "D:\Work\Compilers\Microsoft-Visual-Studio\Final-Logger\Final-Logger\Final-Logger\bin\Release\logs_wateen.1.0.0.nupkg" --api-key oy2bgzkwt2accti4yakachlnu4ezyblezhw5dfzgk44xem --source https://api.nuget.org/v3/index.json
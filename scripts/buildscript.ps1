$currentFolder = (Get-Location).Path
echo $currentFolder
dotnet build ../src/Webserver.API/Webserver.API.csproj
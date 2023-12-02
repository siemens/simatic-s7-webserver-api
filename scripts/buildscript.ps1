$currentFolder = $MyInvocation.MyCommand.Path | Split-Path -Parent;
#$currentFolder = (Get-Location).Path
echo $currentFolder
dotnet build ../src/Webserver.API/Webserver.API.csproj
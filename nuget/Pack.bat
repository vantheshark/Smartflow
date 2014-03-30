del Smartflow.*.nupkg

SETLOCAL
SET VERSION=1.0.1

nuget pack Smartflow\Package.nuspec -Version %VERSION%
nuget pack Smartflow.RabbitMQ\Package.nuspec -Version %VERSION%

ENDLOCAL
pause
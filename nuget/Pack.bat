del Smartflow.*.nupkg

SETLOCAL
SET VERSION=1.0.0

nuget pack Smartflow\Package.nuspec -Version %VERSION%

ENDLOCAL
pause
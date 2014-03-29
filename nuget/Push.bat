@echo OFF
@echo Publishing following pack:
@echo:
DIR /B *.nupkg
@echo:
SETLOCAL
SET VERSION=1.0.0
pause
nuget push Burrow.NET.%VERSION%.nupkg

pause
ENDLOCAL
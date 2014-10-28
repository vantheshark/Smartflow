@echo OFF
@echo Publishing following pack:
@echo:
DIR /B *.nupkg
@echo:
SETLOCAL
SET VERSION=1.0.3
pause

nuget push Smartflow.%VERSION%.nupkg
nuget push Smartflow.RabbitMQ.%VERSION%.nupkg

pause
ENDLOCAL
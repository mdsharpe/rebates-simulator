@echo off
wt ^
-d "%~dp0\Server" powershell -NoExit -Command dotnet watch run

exit

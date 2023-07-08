@echo off
wt ^
-d "%~dp0\Client" powershell -NoExit -Command dotnet watch run --no-hot-reload; ^
new-tab -d "%~dp0\Server" powershell -NoExit -Command dotnet watch run

exit

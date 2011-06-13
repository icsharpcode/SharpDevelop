@echo off
setlocal
set SHARPDEVELOP_PATH="C:\Program Files (x86)\SharpDevelop\4.1"
echo Updating %SHARPDEVELOP_PATH%\bin
xcopy bin\*.* %SHARPDEVELOP_PATH%\bin /s /y /d /exclude:updateInstalledVersion.exceptions
echo Updating %SHARPDEVELOP_PATH%\Addins
xcopy AddIns\*.* %SHARPDEVELOP_PATH%\AddIns /s /y /d /exclude:updateInstalledVersion.exceptions
endlocal

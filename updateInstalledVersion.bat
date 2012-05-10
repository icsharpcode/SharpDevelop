@echo off
setlocal
set SHARPDEVELOP_PATH="C:\Program Files (x86)\SharpDevelop\4.2"

echo Updating %SHARPDEVELOP_PATH%\bin
if "%1"=="clean" del %SHARPDEVELOP_PATH%\bin\* /s /q
xcopy bin\*.* %SHARPDEVELOP_PATH%\bin /s /y /d /exclude:updateInstalledVersion.exceptions

echo Updating %SHARPDEVELOP_PATH%\Addins
if "%1"=="clean" del %SHARPDEVELOP_PATH%\Addins\* /s /q
xcopy AddIns\*.* %SHARPDEVELOP_PATH%\AddIns /s /y /d /exclude:updateInstalledVersion.exceptions

echo Updating %SHARPDEVELOP_PATH%\data
if "%1"=="clean" del %SHARPDEVELOP_PATH%\data\* /s /q
xcopy data\*.* %SHARPDEVELOP_PATH%\data /s /y /d /exclude:updateInstalledVersion.exceptions

endlocal

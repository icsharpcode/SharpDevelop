@set PROGFILES=%PROGRAMFILES%
@if exist "%PROGRAMFILES(x86)%" set PROGFILES=%PROGRAMFILES(x86)%
"%PROGFILES%\MSBuild\12.0\Bin\msbuild" /m SharpDevelop.sln /t:clean "/p:Platform=Any CPU" /p:Configuration=Debug
@IF %ERRORLEVEL% NEQ 0 PAUSE
"%PROGFILES%\MSBuild\12.0\Bin\msbuild" /m SharpDevelop.sln /t:clean "/p:Platform=Any CPU" /p:Configuration=Release
@IF %ERRORLEVEL% NEQ 0 PAUSE
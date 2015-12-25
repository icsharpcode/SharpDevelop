@set PROGFILES=%PROGRAMFILES%
@if exist "%PROGRAMFILES(x86)%" set PROGFILES=%PROGRAMFILES(x86)%
@if not exist "src\Libraries\AvalonEdit\ICSharpCode.AvalonEdit.sln" (
	git submodule update --init || exit /b 1
)
"%PROGFILES%\MSBuild\12.0\Bin\msbuild" /m SharpDevelop.sln /t:clean "/p:Platform=Any CPU" /p:Configuration=Debug
@IF %ERRORLEVEL% NEQ 0 PAUSE
"%PROGFILES%\MSBuild\12.0\Bin\msbuild" /m SharpDevelop.sln /t:clean "/p:Platform=Any CPU" /p:Configuration=Release
@IF %ERRORLEVEL% NEQ 0 PAUSE
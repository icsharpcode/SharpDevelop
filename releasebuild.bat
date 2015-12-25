@set PROGFILES=%PROGRAMFILES%
@if exist "%PROGRAMFILES(x86)%" set PROGFILES=%PROGRAMFILES(x86)%
@if not exist "src\Libraries\AvalonEdit\ICSharpCode.AvalonEdit.sln" (
	git submodule update --init || exit /b 1
)
"%PROGFILES%\MSBuild\12.0\Bin\msbuild" /m SharpDevelop.sln /p:Configuration=Release "/p:Platform=Any CPU" %*
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1
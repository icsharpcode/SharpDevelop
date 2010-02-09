%windir%\microsoft.net\framework\v4.0.30128\msbuild /m SharpDevelop.sln /t:clean "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 PAUSE
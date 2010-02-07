%windir%\microsoft.net\framework\v4.0.21006\msbuild /m SharpDevelop.sln /t:clean "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 PAUSE
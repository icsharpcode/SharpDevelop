%windir%\microsoft.net\framework\v3.5\msbuild SharpDevelop.sln "/p:BooBinPath=%CD%\AddIns\BackendBindings\Boo\RequiredLibraries"
@IF %ERRORLEVEL% NEQ 0 PAUSE
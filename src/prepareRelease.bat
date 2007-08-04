rem Prepares a release by updating the changelog
pushd Tools
%windir%\microsoft.net\framework\v3.5\msbuild /t:PrepareRelease /property:Configuration=Release Tools.build
@popd
@IF %ERRORLEVEL% NEQ 0 PAUSE & EXIT
@echo.
@echo.
@echo.
@echo PrepareRelease.bat completed successfully.
@echo The change log has been updated and a REVISION file containing the current revision number has been created.
@echo.
@pause
rem Prepares a release by updating the changelog
pushd Tools
%windir%\microsoft.net\framework\v2.0.50215\msbuild /t:PrepareRelease /property:Configuration=Release Tools.build
@popd
@IF %ERRORLEVEL% NEQ 0 PAUSE & EXIT
@echo.
@echo.
@echo.
@echo PrepareRelease.bat completed successfully.
@echo The change log has been updated and a VERSION file containing the current revision number has been created.
@echo.
@pause
del "Setup.exe"
"%PROGRAMFILES%\NSIS\makensis" /V2 "SetupScript.nsi"
@IF %ERRORLEVEL% NEQ 0 PAUSE
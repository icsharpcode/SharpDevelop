del "Setup.exe"
%PROGRAMFILES%\NSIS\makensis /V2 "Corsavy.nsi"
@IF %ERRORLEVEL% NEQ 0 PAUSE
@echo off

goto new

:new
echo Generating with #Coco
cd CSharp
..\Frames\SharpCoco -namespace ICSharpCode.NRefactory.Parser.CSharp -frames ..\Frames cs.ATG
del Parser.cs.old >NUL
cd ..

cd VBNet
..\Frames\SharpCoco -namespace ICSharpCode.NRefactory.Parser.VB -frames ..\Frames VBNET.ATG
del Parser.cs.old >NUL
goto exit

:exit
pause
cd ..
cd ..\..\..
%windir%\microsoft.net\framework\v2.0.50727\msbuild

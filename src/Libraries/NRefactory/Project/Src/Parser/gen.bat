@echo off

goto old

:new
echo Generating with new #Coco
copy Frames\Parser.frame.new Frames\Parser.frame
cd CSharp
..\Frames\SharpCoco -namespace ICSharpCode.NRefactory.Parser.CSharp -frames ..\Frames cs.ATG
del Parser.cs.old >NUL
cd ..

cd VBNet
..\Frames\SharpCoco -namespace ICSharpCode.NRefactory.Parser.VB -frames ..\Frames VBNET.ATG
del Parser.cs.old >NUL
goto exit

:old
echo Generating with old #Coco
copy Frames\Parser.frame.old Frames\Parser.frame

cd Frames

copy ..\CSharp\cs.ATG
OldSharpCoco -namespace ICSharpCode.NRefactory.Parser.CSharp cs.ATG
move Parser.cs ..\CSharp

copy ..\VBNet\VBNET.ATG
OldSharpCoco -trace GIPXA -namespace ICSharpCode.NRefactory.Parser.VB VBNET.ATG
move Parser.cs ..\VBNet

del Parser.cs.old >NUL
del cs.ATG VBNET.ATG >NUL

:exit
pause
cd ..

@echo off

echo Generating with #Coco

cd Frames

copy ..\VBNet\VBNET.ATG
SharpCoco -namespace ICSharpCode.NRefactory.VB.Parser VBNET.ATG
move Parser.cs ..\VBNet

del VBNET.ATG

pause
cd ..
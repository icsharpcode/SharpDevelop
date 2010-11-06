@echo off

echo Generating with #Coco

cd Frames

copy ..\vbnet.atg
SharpCoco -namespace ICSharpCode.NRefactory.VB.Parser vbnet.atg
move Parser.cs ..

del vbnet.atg

pause
cd ..
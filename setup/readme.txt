The setup program is maintained by Christoph Wille, christophw@alphasierrapapa.com

Information

Setup uses NSIS 2.0 Release - http://nsis.sourceforge.net


CHANGELOG

10/07/2004:

* Batch files replaced with WSH scripts

9/13/2004:

* bin\setup directory created, batch files added
* gacutil2 source code added
* setup changed to call post & pre batch files, disabled app data deletion

6/23/2004:

* http://abiword.pchasm.org/source/cvs/abiword-cvs/abi/src/pkg/win/setup/NSISv2/abi_util_fileassoc.nsh added
* abi_util_fileassoc.nsh modified (now named fileassoc.nsh) to allow icons per extension in CreateFileAssociation
* modified fileassoc.nsh to match file type registerer in Tools / Options
* Compressor set to lzma in Fidalgo.nsi

2/18/2004: changes for .99 and NSIS 2.0 Release

8/25/2003: added check to .onInit for installed previous version (protect us from 
           partially overwriting previous installations)

8/23/2003: added check to .onInit for installed .NET 1.1 Framework

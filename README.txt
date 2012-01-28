#develop (short for SharpDevelop) is a free IDE for .NET programming languages.
Website: http://www.icsharpcode.net/OpenSource/SD/Default.aspx
Forums: http://community.sharpdevelop.net/forums/
Nightly builds: http://build.sharpdevelop.net/BuildArtefacts

Copyright 2012 AlphaSierraPapa for the SharpDevelop team
License: SharpDevelop is distributed under the LGPL.

The #develop project started on September 11th, 2000. The project was initiated
by Mike Krüger. In the course of the project, several contributors joined in
(we always welcome new developers!), who have helped a great deal to make the
1.0 release a successful one – though it took us four years and a few
architectural changes along the way.

What you got on your machine is now version 4.0 – a vastly enhanced product, and
we encourage you to take the feature tour.

Overview
#develop (short for SharpDevelop) is a free Integrated Development Environment (IDE)
for C#, VB.NET, Boo, IronPython, IronRuby and F# projects on Microsoft's .NET platform.
It is written (almost) entirely in C#, and comes with features you would expect in an
IDE plus a few more.

Features
For detailed information on the features present in SharpDevelop, please take the feature tour:
http://www.icsharpcode.net/OpenSource/SD/Tour/

Support
Our primary means of support is via our Web-based forum (please do not email team
members directly unless they advise you to do so in the forum):
http://community.sharpdevelop.net/forums/ 

Before posting, we would like to encourage you to visit our Wiki pages:
http://wiki.sharpdevelop.net/

When reporting bugs, please use the Bug Reporting forum and be sure to revisit the sticky
topics on how to make good bug reports. Please provide us with steps to reproduce the error.  

Libraries and integrated tools:
	AvalonDock: New BSD License (BSD) (thanks to Adolfo Marinucci) 
	GraphSharp
	IQToolkit
	Irony
	ITextSharp
	log4net
	Mono T4
	Mono.Cecil: MIT License (thanks to Jb Evain) 
	SharpSvn
	SQLite
	WPFToolkit

Integrated Tools (shipping with SharpDevelop):
	Boo
	IronPython
	IronRuby
	NuGet
	NUnit
	PartCover
	Wix

Reusable Libraries (developed as part of SharpDevelop):
	AvalonEdit
	Debugger.Core
	ICSharpCode.Core
	ICSharpCode.Decompiler
	NRefactory
	SharpTreeView
	WPF Designer

SharpDevelop Contributors:
	Mike Krüger (Project Founder)
	Daniel Grunwald (Technical Lead)
	Matt Ward
	David Srbecky (Debugger)
	Siegfried Pammer
	Peter Forstmeier (SharpDevelop Reports)
	
	(for a full list see http://wiki.sharpdevelop.net/Contributors.ashx)
	
Some icons by Yusuke Kamiyamane. 
http://p.yusukekamiyamane.com/
All rights reserved. Licensed under a Creative Commons Attribution 3.0 License.
	
System Requirements (compile and run time)

- Windows XP SP2 or higher.
 - .NET 3.5 SP1
 - .NET 4 prerequisites (might be already installed, e.g. by Windows Update):
   - Windows Installer 3.1: http://www.microsoft.com/downloads/details.aspx?familyid=889482FC-5F56-4A38-B838-DE776FD4138C&displaylang=en
   - Windows Imaging Component: http://www.microsoft.com/downloads/details.aspx?FamilyId=8E011506-6307-445B-B950-215DEF45DDD8&displaylang=en#filelist
 - .NET 4.0 Full (Extended, the "Client" portion is not sufficient)
 - Visual C++ 2008 SP1 Runtime (http://www.microsoft.com/downloads/details.aspx?familyid=A5C84275-3B97-4AB7-A40D-3802B2AF5FC2&displaylang=en)
 
Extended Requirements to compile

- Windows SDK 7.0
- Windows SDK 6.1 (optional; C++ compiler needed for profiler)
- Windows PowerShell
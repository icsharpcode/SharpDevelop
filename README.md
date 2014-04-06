# #develop (short for SharpDevelop) 

##Overview

 #develop (short for SharpDevelop) is a free Integrated Development Environment
(IDE) for C#, VB.NET, Boo, IronPython, IronRuby and F# projects on Microsoft's
.NET platform. It is written (almost) entirely in C#, and comes with features 
you would expect in an IDE plus a few more.

The #develop project started on September 11th, 2000. The project was initiated
by Mike Krüger. In the course of the project, several contributors joined in.
If you want to contribute see [Joining the team][jointheteam].

1. Website: [Official Website][officialwebsite]
2. Forums: [Forums][officialforum]
3. Build Artefacts: [Night Builds][buildartefacts]

###How To Compile
SharpDevelop can be compiled using the supplied .bat files, or in SharpDevelop itself.

###System Requirements (running SharpDevelop)

 - Windows Vista or higher.
 - .NET 4.5
 - [Visual C++ 2008 SP1 Runtime] [vc08sp1runtime]

###Extended Requirements (building SharpDevelop)

 - .NET 3.5 SP1
 - .NET 4.5 SDK (part of Windows SDK 8.0)
 - Windows SDK 7.1 (?? not sure if this still is necessary...)
 - Windows SDK 7.0 (optional; C++ compiler needed for profiler)
 - Windows PowerShell
 - if you have cloned the SD git repository: git must be available on your PATH
 
###Libraries and Integrated tools:

* [Avalon Dock][avalondock] : New BSD License (BSD) (thanks to **Adolfo Marinucci**)
* [Graph Sharp][graphsharp]
* [IQToolkit][iqtoolkit]
* [Irony][irony]
* [ITextSharp][itextsharp]
* [log4Net][githublog4net]
* Mono T4
* [Mono.Cecil][mono.cecil]: MIT License (thanks to **Jb Evain**) - 
* [Sharp Svn][sharpsvn]
* [SQLite][sqlite]
* [WPFToolkit][wpftoolkit]

###Integrated Tools (shipping with SharpDevelop):

* [Iron Python][ironpython]
* [Iron Ruby][ironruby]
* [NuGet][nuget]
* [NUnit][nunit]
* [OpenCover][opencover]
* [WiX][wix]

###Reusable Libraries (developed as part of SharpDevelop):

* AvalonEdit
* Debugger.Core
* ICSharpCode.Core
* ICSharpCode.Decompiler
* NRefactory
* SharpTreeView
* WPF Designer

###SharpDevelop Contributors:

####Developers

* Mike Krüger (Project Founder)
* Daniel Grunwald (Technical Lead)
* Andreas Weizel
* Matt Ward
* David Srbecky (Debugger)
* Siegfried Pammer
* Peter Forstmeier (SharpDevelop Reports)	

####Non-Developers

* Christoph Wille (PM)
* Bernhard Spuida (Kalfaktor)

####Past Developers (Non-Exhuastive List)

* Mike Krüger (Project Founder)
* Alexandre Semenov
* Andrea Paatz
* Christian Hornung
* David Alpert
* Denis ERCHOFF
* Dickon Field
* Georg Brandl
* Ifko Kovacka
* Itai Bar-Haim
* Ivan Shumilin
* John Reilly
* John Simons
* Justin Dearing
* Markus Palme
* Mathias Simmack
* Matt Everson
* Nathan Allan
* Nikola Kavaldjiev
* Philipp Maihart
* Poul Staugaard
* Robert Pickering
* Robert Zaunere
* Roman Taranchenko
* Russell Wilkins
* Scott Ferrett
* Sergej Andrejev
* Shinsaku Nakagawa
* Tomasz Tretkowski
* Troy Simpson

Copyright 2014 AlphaSierraPapa for the SharpDevelop team.
SharpDevelop is distributed under the MIT license.

[officialwebsite]:http://www.icsharpcode.net/OpenSource/SD/Default.aspx
[officialforum]:http://community.sharpdevelop.net/forums/
[buildartefacts]:http://build.sharpdevelop.net/BuildArtefacts
[jointheteam]:https://github.com/icsharpcode/SharpDevelop/wiki/Joining-the-Team
[vc08sp1runtime]:http://www.microsoft.com/downloads/details.aspx?familyid=A5C84275-3B97-4AB7-A40D-3802B2AF5FC2&displaylang=en
[opencover]:https://github.com/OpenCover/opencover
[avalondock]: http://avalondock.codeplex.com/
[graphsharp]:https://graphsharp.codeplex.com/
[iqtoolkit]:https://iqtoolkit.codeplex.com/
[irony]:https://irony.codeplex.com/
[itextsharp]:http://sourceforge.net/projects/itextsharp/
[githublog4net]:https://github.com/apache/log4net
[mono.cecil]:https://github.com/jbevain/cecil
[sharpsvn]:https://sharpsvn.open.collab.net/
[sqlite]:https://sqlite.org/
[wpftoolkit]: https://wpf.codeplex.com/
[ironpython]: http://ironpython.net/
[ironruby]: https://ironruby.codeplex.com/
[nuget]: https://nuget.codeplex.com/
[nunit]: http://www.nunit.org/
[wix]: https://wix.codeplex.com/

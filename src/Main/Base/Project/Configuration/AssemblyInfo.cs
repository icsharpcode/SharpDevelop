// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(false)]
[assembly: StringFreezing()]

// Use hard-binding for ICSharpCode.SharpDevelop:
[assembly: Dependency("ICSharpCode.Core", LoadHint.Always)]
[assembly: Dependency("ICSharpCode.TextEditor", LoadHint.Always)]
[assembly: Dependency("ICSharpCode.NRefactory", LoadHint.Always)]
[assembly: Dependency("System.Drawing", LoadHint.Always)]
[assembly: Dependency("System.Xml", LoadHint.Always)]
[assembly: Dependency("System.Windows.Forms", LoadHint.Always)]
[assembly: Dependency("WeifenLuo.WinFormsUI.Docking", LoadHint.Always)]

[assembly: AssemblyTitle("SharpDevelopBase")]
[assembly: AssemblyDescription("The base add-in of SharpDevelop")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ic#code")]
[assembly: AssemblyProduct("SharpDevelop")]
[assembly: AssemblyCopyright("2000-2006 AlphaSierraPapa")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("2.0.0.1")]

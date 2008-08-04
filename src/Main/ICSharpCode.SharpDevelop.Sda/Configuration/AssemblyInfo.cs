// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

[assembly: CLSCompliant(true)]
[assembly: StringFreezing()]

// Use hard-binding:
[assembly: Dependency("log4net", LoadHint.Always)]

[assembly: AssemblyTitle("ICSharpCode.SharpDevelop.Sda")]
[assembly: AssemblyDescription("SharpDevelop for Applications")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Unrestricted=true)]

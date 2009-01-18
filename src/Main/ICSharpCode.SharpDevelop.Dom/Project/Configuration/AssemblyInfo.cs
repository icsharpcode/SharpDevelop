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

[assembly: Dependency("System.Core", LoadHint.Always)]

[assembly: AssemblyTitle("ICSharpCode.SharpDevelop.Dom")]
[assembly: AssemblyDescription("Code-completion library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Unrestricted=true)]

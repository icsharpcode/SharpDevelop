// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

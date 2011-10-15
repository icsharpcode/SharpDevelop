// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

[assembly: CLSCompliant(false)]
[assembly: StringFreezing()]

// Use hard-binding for ICSharpCode.SharpDevelop:
[assembly: Dependency("ICSharpCode.Core", LoadHint.Always)]
[assembly: Dependency("ICSharpCode.Core.WinForms", LoadHint.Always)]
[assembly: Dependency("ICSharpCode.Core.Presentation", LoadHint.Always)]
[assembly: Dependency("ICSharpCode.AvalonEdit", LoadHint.Always)]
[assembly: Dependency("ICSharpCode.NRefactory", LoadHint.Always)]
[assembly: Dependency("ICSharpCode.SharpDevelop.Dom", LoadHint.Always)]
[assembly: Dependency("ICSharpCode.SharpDevelop.Widgets", LoadHint.Always)]
[assembly: Dependency("System.Core", LoadHint.Always)]
[assembly: Dependency("System.Drawing", LoadHint.Always)]
[assembly: Dependency("System.Xml", LoadHint.Always)]
[assembly: Dependency("System.Windows.Forms", LoadHint.Always)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.SourceAssembly, //where theme specific resource dictionaries are located
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
)]

[assembly: AssemblyTitle("SharpDevelopBase")]
[assembly: AssemblyDescription("The base add-in of SharpDevelop")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

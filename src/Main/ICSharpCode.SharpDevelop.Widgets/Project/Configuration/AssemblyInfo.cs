// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

[assembly: CLSCompliant(true)]
[assembly: StringFreezing()]

[assembly: AssemblyTitle("ICSharpCode.SharpDevelop.Widgets")]
[assembly: AssemblyDescription("UI Widgets for SharpDevelop")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.SourceAssembly, //where theme specific resource dictionaries are located
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
)]

[assembly: XmlnsPrefix("http://icsharpcode.net/sharpdevelop/widgets", "widgets")]
[assembly: XmlnsDefinition("http://icsharpcode.net/sharpdevelop/widgets", "ICSharpCode.SharpDevelop.Widgets")]

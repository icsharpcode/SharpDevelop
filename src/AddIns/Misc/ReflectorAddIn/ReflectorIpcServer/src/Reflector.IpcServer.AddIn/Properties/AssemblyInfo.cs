// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision: 9 $</version>
// </file>

using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Reflector.IpcServer.AddIn")]
[assembly: AssemblyDescription("IPC server AddIn for Red Gate's .NET Reflector")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Christian Hornung")]
[assembly: AssemblyProduct("Reflector.IpcServer")]
[assembly: AssemblyCopyright("Christian Hornung 2008")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

[assembly: CLSCompliant(true)]

// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion("5.0.1.10")]

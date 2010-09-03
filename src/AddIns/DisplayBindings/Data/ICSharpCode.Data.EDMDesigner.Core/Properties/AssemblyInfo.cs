// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ICSharpCode.Data.EDMDesigner.Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("ICSharpCode.Data.EDMDesigner.Core")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("f58b6dc9-cfc8-46bd-8be5-4ba6e13dd92d")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner", "ICSharpCode.Data.EDMDesigner.Core")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Interfaces")]

[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/csdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/csdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/csdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Attributes")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/csdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/csdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Function")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/csdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/csdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type")]

[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/designer", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/designer", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/designer", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/designer", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Association")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/designer", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/designer", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/designer", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/designer", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.SSDL")]

[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/msl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/msl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/msl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Common")]

[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/msl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/msl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType")]

[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/ssdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/ssdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Association")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/ssdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/ssdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function")]
[assembly: XmlnsDefinition("http://icsharpcode.net/data/edmdesigner/ssdl", "ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property")]

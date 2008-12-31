using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SharpDevelop.XamlDesigner")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Organization1")]
[assembly: AssemblyProduct("SharpDevelop.XamlDesigner")]
[assembly: AssemblyCopyright("Copyright © Organization1 2008")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
	//(used if a resource is not found in the page, 
	// or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
	//(used if a resource is not found in the page, 
	// app, or any theme specific resource dictionaries)
)]


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

[assembly: XmlnsPrefix("http://sharpdevelop.net", "sd")]

[assembly: XmlnsDefinition("http://sharpdevelop.net", "SharpDevelop.XamlDesigner")]
[assembly: XmlnsDefinition("http://sharpdevelop.net", "SharpDevelop.XamlDesigner.Commanding")]
[assembly: XmlnsDefinition("http://sharpdevelop.net", "SharpDevelop.XamlDesigner.Controls")]
[assembly: XmlnsDefinition("http://sharpdevelop.net", "SharpDevelop.XamlDesigner.Converters")]
[assembly: XmlnsDefinition("http://sharpdevelop.net", "SharpDevelop.XamlDesigner.Palette")]
[assembly: XmlnsDefinition("http://sharpdevelop.net", "SharpDevelop.XamlDesigner.Outline")]
[assembly: XmlnsDefinition("http://sharpdevelop.net", "SharpDevelop.XamlDesigner.PropertyGrid")]
[assembly: XmlnsDefinition("http://sharpdevelop.net", "SharpDevelop.XamlDesigner.PropertyGrid.Editors")]
[assembly: XmlnsDefinition("http://sharpdevelop.net", "SharpDevelop.XamlDesigner.PropertyGrid.Editors.BrushEditor")]

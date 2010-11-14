#Region "Imports directives"

Imports System.Resources
Imports System.Windows

#End Region



' In order to begin building localizable applications, set 
' <UICulture>CultureYouAreCodingWith</UICulture> in your .vbproj file
' inside a <PropertyGroup>.  For example, if you are using US english
' in your source files, set the <UICulture> to en-US.  Then uncomment
' the NeutralResourceLanguage attribute below.  Update the "en-US" in
' the line below to match the UICulture setting in the project file.

'<Assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)>

' themeDictionaryLocation = where theme specific resource dictionaries are located
' (used if a resource is not found in the page, 
' or application resource dictionaries)
' genericDictionaryLocation = where the generic resource dictionary is located
' (used if a resource is not found in the page, 
' app, or any theme specific resource dictionaries)
<Assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)>

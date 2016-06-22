// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace NoGoop.ObjBrowser
{
	public class ComponentInspectorProperties
	{
		static Properties properties;

		ComponentInspectorProperties()
		{
		}
		
		static ComponentInspectorProperties()
		{
			properties = SD.PropertyService.NestedProperties("ComponentInspector.Properties");
		}

		public static void Update()
		{
			SD.PropertyService.SetNestedProperties("ComponentInspector.Properties", Properties);
		}
		
		static Properties Properties {
			get {
				return properties;
			}
		}
		
		public static bool ShowGettingStartedDialog {
			get {
				return Properties.Get<bool>("ShowGettingStartedDialog", true);
			}
			set {
				Properties.Set<bool>("ShowGettingStartedDialog", value);
			}
		}
		
		public static bool AutoInvokeProperties {
			get {
				return Properties.Get<bool>("AutoInvokeProperties", true);
			}
			set {
				Properties.Set<bool>("AutoInvokeProperties", value);
			}
		}
		
		public static bool ShowObjectAsBaseClass {
			get {
				return Properties.Get<bool>("ShowObjectAsBaseClass", true);
			}
			set {
				Properties.Set<bool>("ShowObjectAsBaseClass", value);
			}
		}
		
		/// <summary>
		/// Gets the running COM objects at startup and adds them to the object tree.
		/// </summary>
		public static bool AddRunningComObjects {
			get {
				return Properties.Get<bool>("AddRunningComObjects", true);
			}
			set {
				Properties.Set<bool>("AddRunningComObjects", value);
			}
		}
		
		public static bool AutoConvertTypeLibs {
			get {
				return Properties.Get<bool>("AutoConvertTypeLibs", true);
			}
			set {
				Properties.Set<bool>("AutoConvertTypeLibs", value);
			}
		}
		
		/// <summary>
		/// Folder that will be used to store generated typelibs.
		/// </summary>
		public static string ConvertedAssemblyDirectory {
			get {
				string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string defaultDirectory = Path.Combine(appDataDirectory, @"ComponentInspector\Converted");
				return Properties.Get<string>("ConvertedAssemblyDirectory", defaultDirectory);
			}
			set {
				Properties.Set<string>("ConvertedAssemblyDirectory", value);
			}
		}
		
		/// <summary>
		/// The working directory of the application being monitored not the
		/// Component Inspector.
		/// </summary>
		public static string ApplicationWorkingDirectory {
			get {
				return Properties.Get<string>("ApplicationWorkingDirectory", Application.StartupPath);
			}
			set {
				Properties.Set<string>("ApplicationWorkingDirectory", value);
			}
		}
		
		public static event EventHandler ShowAssemblyPanelChanged;

		public static bool ShowAssemblyPanel {
			get {
				return Properties.Get<bool>("ShowAssemblyPanel", true);
			}
			set {
				if (ShowAssemblyPanel != value) {
					Properties.Set<bool>("ShowAssemblyPanel", value);
					if (ShowAssemblyPanelChanged != null) {
						ShowAssemblyPanelChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowControlPanelChanged;

		public static bool ShowControlPanel {
			get {
				return Properties.Get<bool>("ShowControlPanel", false);
			}
			set {
				if (ShowControlPanel != value) {
					Properties.Set<bool>("ShowControlPanel", value);
					if (ShowControlPanelChanged != null) {
						ShowControlPanelChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowGacPanelChanged;

		public static bool ShowGacPanel {
			get {
				return Properties.Get<bool>("ShowGacPanel", false);
			}
			set {
				if (ShowGacPanel != value) {
					Properties.Set<bool>("ShowGacPanel", value);
					if (ShowGacPanelChanged != null) {
						ShowGacPanelChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowBaseClassNamesChanged;
		
		public static bool ShowBaseClassNames {
			get {
				return Properties.Get<bool>("ShowBaseClassNames", false);
			}
			set {
				if (ShowBaseClassNames != value) {
					Properties.Set<bool>("ShowBaseClassNames", value);
					if (ShowBaseClassNamesChanged != null) {
						ShowBaseClassNamesChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowFieldsChanged;
		
		public static bool ShowFields {
			get {
				return Properties.Get<bool>("ShowFields", true);
			}
			set {
				if (ShowFields != value) {
					Properties.Set<bool>("ShowFields", value);
					if (ShowFieldsChanged != null) {
						ShowFieldsChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowPropertiesChanged;
		
		public static bool ShowProperties {
			get {
				return Properties.Get<bool>("ShowProperties", true);
			}
			set {
				if (ShowProperties != value) {
					Properties.Set<bool>("ShowProperties", value);
					if (ShowPropertiesChanged != null) {
						ShowPropertiesChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowMethodsChanged;
		
		public static bool ShowMethods {
			get {
				return Properties.Get<bool>("ShowMethods", true);
			}
			set {
				if (ShowMethods != value) {
					Properties.Set<bool>("ShowMethods", value);
					if (ShowMethodsChanged != null) {
						ShowMethodsChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowEventsChanged;

		public static bool ShowEvents {
			get {
				return Properties.Get<bool>("ShowEvents", true);
			}
			set {
				if (ShowEvents != value) {
					Properties.Set<bool>("ShowEvents", value);
					if (ShowEventsChanged != null) {
						ShowEventsChanged(properties, new EventArgs());
					}
				}
			}
		}

		public static event EventHandler ShowBaseClassesChanged;

		public static bool ShowBaseClasses {
			get {
				return Properties.Get<bool>("ShowBaseClasses", true);
			}
			set {
				if (ShowBaseClasses != value) {
					Properties.Set<bool>("ShowBaseClasses", value);
					if (ShowBaseClassesChanged != null) {
						ShowBaseClassesChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowPublicMembersOnlyChanged;

		public static bool ShowPublicMembersOnly {
			get {
				return Properties.Get<bool>("ShowPublicMembersOnly", false);
			}
			set {
				if (ShowPublicMembersOnly != value) {
					Properties.Set<bool>("ShowPublicMembersOnly", value);
					if (ShowPublicMembersOnlyChanged != null) {
						ShowPublicMembersOnlyChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler CategoryThresholdChanged;
		
		public static int CategoryThreshold {
			get {
				return Properties.Get<int>("CategoryThreshold", 100);
			}
			set {
				if (CategoryThreshold != value) {
					Properties.Set<int>("CategoryThreshold", value);
					if (CategoryThresholdChanged != null) {
						CategoryThresholdChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		
		public static event EventHandler ShowMemberCategoriesChanged;

		public static bool ShowMemberCategories {
			get {
				return Properties.Get<bool>("ShowMemberCategories", true);
			}
			set {
				if (ShowMemberCategories != value) {
					Properties.Set<bool>("ShowMemberCategories", value);
					if (ShowMemberCategoriesChanged != null) {
						ShowMemberCategoriesChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowBaseCategoriesChanged;

		public static bool ShowBaseCategories {
			get {
				return Properties.Get<bool>("ShowBaseCategories", true);
			}
			set {
				if (ShowBaseCategories != value) {
					Properties.Set<bool>("ShowBaseCategories", value);
					if (ShowBaseCategoriesChanged != null) {
						ShowBaseCategoriesChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler ShowPropertyAccessorMethodsChanged;

		public static bool ShowPropertyAccessorMethods {
			get {
				return Properties.Get<bool>("ShowPropertyAccessorMethods", false);
			}
			set {
				if (ShowPropertyAccessorMethods != value) {
					Properties.Set<bool>("ShowPropertyAccessorMethods", value);
					if (ShowPropertyAccessorMethodsChanged != null) {
						ShowPropertyAccessorMethodsChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler DisplayHexChanged;

		public static bool DisplayHex {
			get {
				return Properties.Get<bool>("DisplayHex", false);
			}
			set {
				if (DisplayHex != value) {
					Properties.Set<bool>("DisplayHex", value);
					if (DisplayHexChanged != null) {
						DisplayHexChanged(properties, new EventArgs());
					}
				}
			}
		}
		
		public static event EventHandler TypeHandlerChanged;
		
		public static bool IsTypeHandlerEnabled(string name)
		{
			string propertyName = GetTypeHandlerPropertyName(name);
			return properties.Get<bool>(propertyName, true);
		}
		
		public static void EnableTypeHandler(string name, bool enabled)
		{
			if (IsTypeHandlerEnabled(name) != enabled) {
				string propertyName = GetTypeHandlerPropertyName(name);
				properties.Set<bool>(propertyName, enabled);
				if (TypeHandlerChanged != null) {
					TypeHandlerChanged(properties, new EventArgs());
				}
			}
		}
		
		static string GetTypeHandlerPropertyName(string name)
		{
			return String.Concat("TypeHandler.", name);
		}
		
		static PreviouslyOpenedAssemblyCollection previouslyOpenedAssemblies;
		
		public static PreviouslyOpenedAssemblyCollection PreviouslyOpenedAssemblies {
			get {
				if (previouslyOpenedAssemblies == null) {
					previouslyOpenedAssemblies = new PreviouslyOpenedAssemblyCollection(properties);
				}
				return previouslyOpenedAssemblies;
			}
		}
		
		static PreviouslyOpenedTypeLibraryCollection previouslyOpenedTypeLibraries;
		
		public static PreviouslyOpenedTypeLibraryCollection PreviouslyOpenedTypeLibraries {
			get {
				if (previouslyOpenedTypeLibraries == null) {
					previouslyOpenedTypeLibraries = new PreviouslyOpenedTypeLibraryCollection(properties);
				}
				return previouslyOpenedTypeLibraries;
			}
		}
		
		static SavedCastInfoCollection savedCasts;
		
		public static SavedCastInfoCollection SavedCasts {
			get {
				if (savedCasts == null) {
					savedCasts = new SavedCastInfoCollection(properties);
				}
				return savedCasts;
			}
		}
	}
}

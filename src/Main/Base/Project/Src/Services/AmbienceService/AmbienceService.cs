// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.Core
{
	public static class AmbienceService
	{
		static readonly string ambienceProperty       = "SharpDevelop.UI.CurrentAmbience";
		static readonly string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";
		
		public static Properties CodeGenerationProperties {
			get {
				return PropertyService.Get(codeGenerationProperty, new Properties());
			}
		}
		
		public static bool GenerateDocumentComments {
			get {
				return CodeGenerationProperties.Get("GenerateDocumentComments", true);
			}
		}
		
		public static bool GenerateAdditionalComments {
			get {
				return CodeGenerationProperties.Get("GenerateAdditionalComments", true);
			}
		}
		
		public static bool UseFullyQualifiedNames {
			get {
				return CodeGenerationProperties.Get("UseFullyQualifiedNames", true);
			}
		}
		
		static AmbienceReflectionDecorator defaultAmbience;
		
		public static AmbienceReflectionDecorator CurrentAmbience {
			get {
				ICSharpCode.SharpDevelop.Project.IProject p = ICSharpCode.SharpDevelop.Project.ProjectService.CurrentProject;
				if (p != null) {
					IAmbience ambience = p.Ambience;
					if (ambience != null) {
						return new AmbienceReflectionDecorator(ambience);
					}
				}
				if (defaultAmbience == null) {
					string language = PropertyService.Get(ambienceProperty, "C#");
					IAmbience ambience = (IAmbience)AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences").BuildChildItem(language, null);
					if (ambience == null) {
						MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Services.AmbienceService.AmbienceNotFoundError}");
						return null;
					}
					defaultAmbience = new AmbienceReflectionDecorator(ambience);
				}
				return defaultAmbience;
			}
		}
		
		static void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == ambienceProperty) {
				defaultAmbience = null;
				OnAmbienceChanged(EventArgs.Empty);
			}
		}
		
		static AmbienceService()
		{
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
		}
		
		
		static void OnAmbienceChanged(EventArgs e)
		{
			if (AmbienceChanged != null) {
				AmbienceChanged(null, e);
			}
		}
		
		public static event EventHandler AmbienceChanged;
	}
}

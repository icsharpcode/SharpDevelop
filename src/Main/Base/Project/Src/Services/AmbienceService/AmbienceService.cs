// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.SharpDevelop
{
	public static class AmbienceService
	{
		const string ambienceProperty       = "SharpDevelop.UI.CurrentAmbience";
		const string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";
		
		static AmbienceService()
		{
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
		}
		
		public static Properties CodeGenerationProperties {
			get {
				return PropertyService.Get(codeGenerationProperty, new Properties());
			}
		}
		
		static List<CodeGenerator> codeGenerators = new List<CodeGenerator>();
		
		static void ApplyCodeGenerationProperties(CodeGenerator generator)
		{
			CodeGeneratorOptions options = generator.Options;
			System.CodeDom.Compiler.CodeGeneratorOptions cdo = new CodeDOMGeneratorUtility().CreateCodeGeneratorOptions;
			
			options.EmptyLinesBetweenMembers = cdo.BlankLinesBetweenMembers;
			options.BracesOnSameLine = CodeGenerationProperties.Get("StartBlockOnSameLine", true);;
			options.IndentString = cdo.IndentString;
		}
		
		internal static void InitializeCodeGeneratorOptions(CodeGenerator generator)
		{
			codeGenerators.Add(generator);
			ApplyCodeGenerationProperties(generator);
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
		
		public static bool UseProjectAmbienceIfPossible {
			get {
				return PropertyService.Get("SharpDevelop.UI.UseProjectAmbience", true);
			}
			set {
				PropertyService.Set("SharpDevelop.UI.UseProjectAmbience", value);
			}
		}
		
		static IAmbience defaultAmbience;
		
		/// <summary>
		/// Gets the current ambience.
		/// Might return a new ambience object, or use an existing. Use this method only on the main thread.
		/// </summary>
		public static IAmbience GetCurrentAmbience()
		{
			Gui.WorkbenchSingleton.AssertMainThread();
			
			IAmbience ambience;
			if (UseProjectAmbienceIfPossible) {
				ICSharpCode.SharpDevelop.Project.IProject p = ICSharpCode.SharpDevelop.Project.ProjectService.CurrentProject;
				if (p != null) {
					ambience = p.GetAmbience();
					if (ambience != null)
						return ambience;
				}
			}
			if (defaultAmbience == null) {
				string language = DefaultAmbienceName;
				try {
					ambience = (IAmbience)AddInTree.BuildItem("/SharpDevelop/Workbench/Ambiences/" + language, null);
				} catch (TreePathNotFoundException) {
					ambience = null;
				}
				if (ambience == null && Gui.WorkbenchSingleton.MainForm != null) {
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Services.AmbienceService.AmbienceNotFoundError}");
				}
				defaultAmbience = ambience ?? new NetAmbience();
			}
			return defaultAmbience;
		}
		
		public static string DefaultAmbienceName {
			get {
				return PropertyService.Get(ambienceProperty, "C#");
			}
			set {
				PropertyService.Set(ambienceProperty, value);
			}
		}
		
		static void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == ambienceProperty) {
				OnAmbienceChanged(EventArgs.Empty);
			}
			if (e.Key == codeGenerationProperty) {
				codeGenerators.ForEach(ApplyCodeGenerationProperties);
			}
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

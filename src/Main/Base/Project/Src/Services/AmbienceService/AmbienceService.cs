// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		/// <summary>
		/// Gets the current ambience.
		/// This method is thread-safe.
		/// </summary>
		/// <returns>Returns a new ambience object (ambience objects are never reused to ensure their thread-safety).
		/// Never returns null, in case of errors the <see cref="NetAmbience"/> is used.</returns>
		public static IAmbience GetCurrentAmbience()
		{
			IAmbience ambience;
			if (UseProjectAmbienceIfPossible) {
				ICSharpCode.SharpDevelop.Project.IProject p = ICSharpCode.SharpDevelop.Project.ProjectService.CurrentProject;
				if (p != null) {
					ambience = p.GetAmbience();
					if (ambience != null)
						return ambience;
				}
			}
			string language = DefaultAmbienceName;
			try {
				ambience = (IAmbience)AddInTree.BuildItem("/SharpDevelop/Workbench/Ambiences/" + language, null);
			} catch (TreePathNotFoundException) {
				ambience = null;
			}
			if (ambience == null && Gui.WorkbenchSingleton.MainWin32Window != null) {
				MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Services.AmbienceService.AmbienceNotFoundError}");
			}
			return ambience ?? new NetAmbience();
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

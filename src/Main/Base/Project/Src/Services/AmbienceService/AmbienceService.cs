// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public static class AmbienceService
	{
		const string ambienceProperty       = "SharpDevelop.UI.CurrentAmbience";
		const string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";
		
		public static Properties CodeGenerationProperties {
			get {
				return PropertyService.NestedProperties(codeGenerationProperty);
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
		/// Never returns null, in case of errors the <see cref="DefaultAmbience"/> is used.</returns>
		public static IAmbience GetCurrentAmbience()
		{
			if (UseProjectAmbienceIfPossible) {
				IProject p = ProjectService.CurrentProject;
				if (p != null) {
					return p.GetAmbience();
				}
			}
			string language = DefaultAmbienceName;
			IAmbience ambience;
			try {
				ambience = (IAmbience)SD.AddInTree.BuildItem("/SharpDevelop/Workbench/Ambiences/" + language, null);
			} catch (TreePathNotFoundException) {
				ambience = null;
			}
			if (ambience == null && SD.WinForms.MainWin32Window != null) {
				MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Services.AmbienceService.AmbienceNotFoundError}");
			}
			return ambience ?? new DefaultAmbience();
		}
		
		public static string DefaultAmbienceName {
			get {
				return PropertyService.Get(ambienceProperty, "C#");
			}
			set {
				PropertyService.Set(ambienceProperty, value);
			}
		}
	}
}

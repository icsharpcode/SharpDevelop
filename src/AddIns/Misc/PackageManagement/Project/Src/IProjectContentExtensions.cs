// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public static class NRefactoryExtensionsForEnvDTE
	{
		public static string GetCodeModelLanguage(this IProject project)
		{
			if (project != null && project.Language == "VB") {
				return global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB;
			}
			return global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp;
		}
		
		public static global::EnvDTE.vsCMAccess ToAccess(this Accessibility accessiblity)
		{
			if (accessiblity == Accessibility.Public)
				return global::EnvDTE.vsCMAccess.vsCMAccessPublic;
			else
				return global::EnvDTE.vsCMAccess.vsCMAccessPrivate;
		}
		
		public static Accessibility ToAccessibility(this global::EnvDTE.vsCMAccess access)
		{
			switch (access) {
				case global::EnvDTE.vsCMAccess.vsCMAccessPublic:
					return Accessibility.Public;
				case global::EnvDTE.vsCMAccess.vsCMAccessPrivate:
					return Accessibility.Private;
				default:
					throw new Exception("Invalid value for vsCMAccess");
			}
		}
		
		public static CodeElementsList<T> AsCodeElements<T>(this IModelCollection<T> input)
			where T : global::EnvDTE.CodeElement
		{
			var list = new CodeElementsList<T>();
			// TODO: react to changes
			foreach (var element in input)
				list.Add(element);
			return list;
		}
	}
}

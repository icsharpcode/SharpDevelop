// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public static class IProjectContentExtensions
	{
		public static string GetCodeModelLanguage(this IProjectContent projectContent)
		{
			if (projectContent.Project != null) {
				var projectType = new ProjectType(projectContent.Project as MSBuildBasedProject);
				if (projectType.Type == ProjectType.VBNet) {
					return global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB;
				}
			}
			return global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp;
		}
	}
}

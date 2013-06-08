// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using Microsoft.Build.Construction;

namespace ICSharpCode.PackageManagement
{
	public static class ProjectRootElementExtensions
	{
		public static ProjectImportElement FindImport(this ProjectRootElement rootElement, string importedProjectFile)
		{
			return rootElement
				.Imports
				.FirstOrDefault(import => String.Equals(import.Project, importedProjectFile, StringComparison.OrdinalIgnoreCase));
		}
	}
}

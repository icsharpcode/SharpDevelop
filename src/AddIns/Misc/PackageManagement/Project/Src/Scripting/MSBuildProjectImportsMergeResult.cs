// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop;
using Microsoft.Build.Construction;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class MSBuildProjectImportsMergeResult
	{
		List<string> projectImportsAdded = new List<string>();
		List<string> projectImportsRemoved = new List<string>();
		
		public MSBuildProjectImportsMergeResult()
		{
		}
		
		public IEnumerable<string> ProjectImportsAdded {
			get { return projectImportsAdded; }
		}
		
		public IEnumerable<string> ProjectImportsRemoved {
			get { return projectImportsRemoved; }
		}
		
		public override string ToString()
		{
			return String.Format(
				"Imports added: {0}\r\nImports removed: {1}",
				ImportsToString(projectImportsAdded),
				ImportsToString(projectImportsRemoved));
		}
		
		static string ImportsToString(IEnumerable<string> imports)
		{
			if (!imports.Any()) {
				return String.Empty;
			}
			
			return String.Join(",\r\n", imports.Select(import => String.Format("'{0}'", import)));
		}
		
		public void AddProjectImportsRemoved(IEnumerable<ProjectImportElement> imports)
		{
			imports.ForEach(import => projectImportsRemoved.Add(import.Project));
		}
		
		public void AddProjectImportsAdded(IEnumerable<ProjectImportElement> imports)
		{
			imports.ForEach(import => projectImportsAdded.Add(import.Project));
		}
	}
}

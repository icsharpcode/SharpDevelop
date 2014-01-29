// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectKind
	{
		public ProjectKind(Project project)
		{
			this.Kind = GetProjectKind(project);
		}
		
		string GetProjectKind(Project project)
		{
			string type = ProjectType.GetProjectType(project);
			if (type == ProjectType.CSharp) {
				return SD.ProjectTypeGuids.CSharp.ToString();
			} else if (type == ProjectType.VB) {
				return SD.ProjectTypeGuids.VB.ToString();
			}
			return String.Empty;
		}
		
		public string Kind { get; private set; }
	}
}

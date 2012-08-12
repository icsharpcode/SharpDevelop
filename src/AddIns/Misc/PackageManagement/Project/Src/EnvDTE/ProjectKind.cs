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
			string type = new ProjectType(project).Type;
			if (type == ProjectType.CSharp) {
				return SD.ProjectTypeGuids.CSharp;
			} else if (type == ProjectType.VBNet) {
				return SD.ProjectTypeGuids.VBNet;
			}
			return String.Empty;
		}
		
		public string Kind { get; private set; }
	}
}

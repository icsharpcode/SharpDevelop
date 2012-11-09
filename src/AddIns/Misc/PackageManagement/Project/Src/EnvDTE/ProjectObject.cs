// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectObject : MarshalByRefObject, global::EnvDTE.ProjectObject
	{
		public ProjectObject(Project project)
		{
			this.Project = project;
			References = new References(project);
		}
		
		public global::EnvDTE.References References { get; private set; }
		public global::EnvDTE.Project Project { get; private set; }
		
		public global::EnvDTE.DTE DTE {
			get { return Project.DTE; }
		}
	}
}

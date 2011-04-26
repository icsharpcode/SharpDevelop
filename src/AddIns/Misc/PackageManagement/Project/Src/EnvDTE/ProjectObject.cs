// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectObject
	{
		MSBuildBasedProject project;
		
		public ProjectObject(MSBuildBasedProject project)
		{
			this.project = project;
			References = new References(project);
		}
		
		public References References { get; private set; }
	}
}

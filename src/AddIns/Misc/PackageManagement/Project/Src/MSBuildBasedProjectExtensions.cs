// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public static class MSBuildBasedProjectExtensions
	{
		public static bool IsWebProject(this MSBuildBasedProject project)
		{
			return project.HasProjectType(ProjectTypeGuids.WebApplication) || project.HasProjectType(ProjectTypeGuids.WebSite);
		}
	}
}

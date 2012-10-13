// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public static class MSBuildBasedProjectExtensions
	{
		public static readonly Guid WebApplication = Guid.Parse(ProjectTypeGuids.WebApplication);
		public static readonly Guid WebSite = Guid.Parse(ProjectTypeGuids.WebSite);
		
		public static bool IsWebProject(this MSBuildBasedProject project)
		{
			return project.HasProjectType(WebApplication) || project.HasProjectType(WebSite);
		}
	}
}

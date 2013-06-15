// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public static class ProjectType
	{
		public static readonly string VBNet = "VB.NET";
		public static readonly string CSharp = "C#";
		
		public static string GetProjectType(Project project)
		{
			return GetProjectType(project.MSBuildProject);
		}
		
		public static string GetProjectType(IProject project)
		{
			if (project.Language == "VB")
				return VBNet;
			else
				return project.Language;
		}
	}
}

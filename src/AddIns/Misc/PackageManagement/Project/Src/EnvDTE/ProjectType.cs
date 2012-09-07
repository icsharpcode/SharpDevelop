// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectType
	{
		public static readonly string VBNet = "VB.NET";
		public static readonly string CSharp = "C#";
		
		public ProjectType(Project project)
		{
			this.Type = GetProjectType(project);
		}
		
		public ProjectType(MSBuildBasedProject project)
			: this(new Project(project))
		{
		}
		
		string GetProjectType(Project project)
		{
			string extension = project.GetLowercaseFileExtension();
			if (extension == ".csproj") {
				return CSharp;
			} else if (extension == ".vbproj") {
				return VBNet;
			}
			return String.Empty;
		}
		
		public string Type { get; private set; }
	}
}

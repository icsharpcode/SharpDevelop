// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectType
	{
		Project project;
		
		public ProjectType(Project project)
		{
			this.project = project;
			Type = String.Empty;
			GetProjectType();
		}
		
		void GetProjectType()
		{
			string extension = GetProjectFileExtension();
			if (extension == ".csproj") {
				Type = "C#";
			} else if (extension == ".vbproj") {
				Type = "VB.NET";
			}
		}
		
		string GetProjectFileExtension()
		{
			return Path.GetExtension(project.FileName).ToLowerInvariant();
		}
		
		public string Type { get; private set; }
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Property
	{
		Project project;
		string name;
		
		public Property(Project project, string name)
		{
			this.project = project;
			this.name = name;
		}
		
		public object Value {
			get { return GetProperty(name); }
			set {
				SetProperty(name, value);
				project.Save();
			}
		}
		
		string GetProperty(string name)
		{
			string value = project.MSBuildProject.GetUnevalatedProperty(name);
			if (value != null) {
				return value;
			}
			
			if (IsTargetFrameworkMoniker(name)) {
				return GetTargetFrameworkMoniker();
			}
			return EmptyStringIfNull(value);
		}
		
		bool IsTargetFrameworkMoniker(string name)
		{
			return String.Equals(name, "TargetFrameworkMoniker", StringComparison.InvariantCultureIgnoreCase);
		}
		
		string GetTargetFrameworkMoniker()
		{
			var targetFramework = new ProjectTargetFramework(project.MSBuildProject);
			return targetFramework.TargetFrameworkName.ToString();
		}
		
		string EmptyStringIfNull(string value)
		{
			if (value != null) {
				return value;
			}
			return String.Empty;
		}
		
		void SetProperty(string name, object value)
		{
			bool escapeValue = false;
			project.MSBuildProject.SetProperty(name, value as string, escapeValue);
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectProperty : Property
	{
		Project project;
		
		public ProjectProperty(Project project, string name)
			: base(name)
		{
			this.project = project;
		}
		
		protected override object GetValue()
		{
			string value = project.MSBuildProject.GetUnevalatedProperty(Name);
			if (value != null) {
				return value;
			}
			
			if (IsTargetFrameworkMoniker(Name)) {
				return GetTargetFrameworkMoniker();
			} else if (IsFullPath(Name)) {
				return GetFullPath();
			}
			return EmptyStringIfNull(value);
		}
		
		bool IsTargetFrameworkMoniker(string name)
		{
			return IsCaseInsensitiveMatch(name, "TargetFrameworkMoniker");
		}
		
		bool IsFullPath(string name)
		{
			return IsCaseInsensitiveMatch(name, "FullPath");
		}
		
		bool IsCaseInsensitiveMatch(string a, string b)
		{
			return String.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}
		
		string GetTargetFrameworkMoniker()
		{
			var targetFramework = new ProjectTargetFramework(MSBuildProject);
			return targetFramework.TargetFrameworkName.ToString();
		}
		
		MSBuildBasedProject MSBuildProject {
			get { return project.MSBuildProject; }
		}
		
		string GetFullPath()
		{
			return MSBuildProject.Directory;
		}
		
		string EmptyStringIfNull(string value)
		{
			if (value != null) {
				return value;
			}
			return String.Empty;
		}
		
		protected override void SetValue(object value)
		{
			bool escapeValue = false;
			MSBuildProject.SetProperty(Name, value as string, escapeValue);
			project.Save();
		}
	}
}

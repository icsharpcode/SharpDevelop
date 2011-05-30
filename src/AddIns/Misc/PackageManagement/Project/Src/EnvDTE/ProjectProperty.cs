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
			}
			return EmptyStringIfNull(value);
		}
		
		bool IsTargetFrameworkMoniker(string name)
		{
			return String.Equals(name, "TargetFrameworkMoniker", StringComparison.InvariantCultureIgnoreCase);
		}
		
		string GetTargetFrameworkMoniker()
		{
			var targetFramework = new ProjectTargetFramework(MSBuildProject);
			return targetFramework.TargetFrameworkName.ToString();
		}
		
		MSBuildBasedProject MSBuildProject {
			get { return project.MSBuildProject; }
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

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectProperty : Property
	{
		public ProjectProperty(Project project, string name)
			: base(project, name)
		{
		}
		
		protected override object GetProperty()
		{
			string value = Project.MSBuildProject.GetUnevalatedProperty(Name);
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
			var targetFramework = new ProjectTargetFramework(Project.MSBuildProject);
			return targetFramework.TargetFrameworkName.ToString();
		}
		
		string EmptyStringIfNull(string value)
		{
			if (value != null) {
				return value;
			}
			return String.Empty;
		}
		
		protected override void SetProperty(object value)
		{
			bool escapeValue = false;
			Project.MSBuildProject.SetProperty(Name, value as string, escapeValue);
		}
	}
}

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
			return EmptyStringIfNull(value);
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

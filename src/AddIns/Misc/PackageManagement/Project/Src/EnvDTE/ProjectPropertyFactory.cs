// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectPropertyFactory : IPropertyFactory
	{
		Project project;
		
		public ProjectPropertyFactory(Project project)
		{
			this.project = project;
		}
		
		public Property CreateProperty(string name)
		{
			return new ProjectProperty(project, name);
		}
		
		public IEnumerator<Property> GetEnumerator()
		{
			List<Property> properties = GetProperties().ToList();
			return properties.GetEnumerator();
		}
		
		IEnumerable<Property> GetProperties()
		{
			foreach (string propertyName in project.GetAllPropertyNames()) {
				yield return CreateProperty(propertyName);
			}
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Configuration : MarshalByRefObject, global::EnvDTE.Configuration
	{
		Project project;
		
		public Configuration(Project project)
		{
			this.project = project;
		}
		
		public global::EnvDTE.Properties Properties {
			get { return project.Properties; }
		}
	}
}

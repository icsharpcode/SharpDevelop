// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop 
{
	public delegate void ProjectConfigurationEventHandler(object sender, ProjectConfigurationEventArgs e);
	
	public class ProjectConfigurationEventArgs : EventArgs
	{ 
		string   configuration;
		IProject project;
		
		public string Configuration {
			get {
				return configuration;
			}
		}
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public ProjectConfigurationEventArgs(IProject project, string configuration)
		{
			this.configuration = configuration;
			this.project = project;
		}
		
	}
}

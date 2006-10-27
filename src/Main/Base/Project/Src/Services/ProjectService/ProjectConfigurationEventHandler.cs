// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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

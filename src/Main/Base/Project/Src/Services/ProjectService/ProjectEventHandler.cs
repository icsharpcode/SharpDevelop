// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void ProjectEventHandler(object sender, ProjectEventArgs e);
	
	public class ProjectEventArgs : EventArgs
	{
		IProject project;
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public ProjectEventArgs(IProject project)
		{
			this.project = project;
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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

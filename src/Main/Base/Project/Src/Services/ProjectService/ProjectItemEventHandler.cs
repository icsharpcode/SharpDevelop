// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectItemEventArgs : ProjectEventArgs
	{
		ProjectItem projectItem;
		
		public ProjectItem ProjectItem {
			get {
				return projectItem;
			}
		}
		
		public ProjectItemEventArgs(IProject project, ProjectItem projectItem) : base(project)
		{
			this.projectItem = projectItem;
		}
	}
}

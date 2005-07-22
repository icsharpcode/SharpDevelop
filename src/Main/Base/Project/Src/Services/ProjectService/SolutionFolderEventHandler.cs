// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void SolutionFolderEventHandler(object sender, SolutionFolderEventArgs e);
	
	public class SolutionFolderEventArgs
	{
		ISolutionFolder solutionFolder;
		
		public ISolutionFolder SolutionFolder {
			get {
				return solutionFolder;
			}
		}
		
		public SolutionFolderEventArgs(ISolutionFolder solutionFolder)
		{
			this.solutionFolder = solutionFolder;
		}
		
	}
}

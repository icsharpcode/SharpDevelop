// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void SolutionFolderEventHandler(object sender, SolutionFolderEventArgs e);
	
	public class SolutionFolderEventArgs : EventArgs
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

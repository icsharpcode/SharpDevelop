// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void SolutionFolderEventHandler(object sender, SolutionFolderEventArgs e);
	
	public class SolutionFolderEventArgs : EventArgs
	{
		ISolutionItem solutionFolder;
		
		public ISolutionItem SolutionFolder {
			get {
				return solutionFolder;
			}
		}
		
		public SolutionFolderEventArgs(ISolutionItem solutionFolder)
		{
			this.solutionFolder = solutionFolder;
		}
		
	}
}

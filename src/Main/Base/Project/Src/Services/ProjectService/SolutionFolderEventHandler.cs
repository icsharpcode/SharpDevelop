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

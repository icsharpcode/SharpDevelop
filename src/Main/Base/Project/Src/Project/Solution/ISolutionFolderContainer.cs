using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ISolutionFolderContainer.
	/// </summary>
	public interface ISolutionFolderContainer
	{
		List<ProjectSection> Sections {
			get;
		}
		
		List<ISolutionFolder> Folders {
			get;
		}
		
		ProjectSection SolutionItems {
			get;
		}
		
		void AddFolder(ISolutionFolder folder);
		void RemoveFolder(ISolutionFolder folder);
		
		bool IsAncestorOf(ISolutionFolder folder);
	}
}

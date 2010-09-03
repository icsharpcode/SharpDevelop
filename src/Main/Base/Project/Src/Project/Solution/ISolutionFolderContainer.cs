// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ISolutionFolderContainer.
	/// </summary>
	public interface ISolutionFolderContainer
	{
		Solution ParentSolution {
			get;
		}
		
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

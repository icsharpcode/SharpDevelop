// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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

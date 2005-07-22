// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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

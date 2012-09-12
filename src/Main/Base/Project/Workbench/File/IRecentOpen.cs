// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <see cref="IFileService.RecentOpen"/>
	public interface IRecentOpen
	{
		IReadOnlyList<string> RecentFiles { get; }
		IReadOnlyList<string> RecentProjects { get; }
		
		void ClearRecentFiles();
		void ClearRecentProjects();
		void AddRecentFile(string fileName);
		void AddRecentProject(string fileName);
	}
}

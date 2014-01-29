// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <see cref="IFileService.RecentOpen"/>
	public interface IRecentOpen
	{
		IReadOnlyList<FileName> RecentFiles { get; }
		IReadOnlyList<FileName> RecentProjects { get; }
		
		void ClearRecentFiles();
		void ClearRecentProjects();
		void AddRecentFile(FileName fileName);
		void AddRecentProject(FileName fileName);
	}
}

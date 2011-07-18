// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class ManagePackagesViewTitle
	{
		public ManagePackagesViewTitle(IPackageManagementSolution solution)
		{
			GetTitle(solution);
		}
		
		void GetTitle(IPackageManagementSolution solution)
		{
			var selectedProjects = new PackageManagementSelectedProjects(solution);
			string selectionName = selectedProjects.SelectionName;
			Title = String.Format("{0} - Manage Packages", selectionName);
		}
		
		public string Title { get; private set; }
	}
}

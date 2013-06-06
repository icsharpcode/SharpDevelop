// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageViewModelFactory
	{
		PackageViewModel CreatePackageViewModel(IPackageViewModelParent parent, IPackageFromRepository package);
		
		IPackageManagementSolution Solution { get; }
		PackageManagementSelectedProjects SelectedProjects { get; }
		IPackageManagementEvents PackageManagementEvents { get; }
		IPackageActionRunner PackageActionRunner { get; }
		ILogger Logger { get; }
	}
}

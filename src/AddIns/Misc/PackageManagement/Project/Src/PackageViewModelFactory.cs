// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageViewModelFactory : IPackageViewModelFactory
	{
		public PackageViewModelFactory(IPackageViewModelFactory packageViewModelFactory)
			: this(
				packageViewModelFactory.Solution,
				packageViewModelFactory.PackageManagementEvents,
				packageViewModelFactory.PackageActionRunner)
		{
		}
		
		public PackageViewModelFactory(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents,
			IPackageActionRunner actionRunner)
		{
			this.Solution = solution;
			this.SelectedProjects = new PackageManagementSelectedProjects(solution);
			this.PackageManagementEvents = packageManagementEvents;
			this.PackageActionRunner = actionRunner;
			this.Logger = new PackageManagementLogger(packageManagementEvents);
		}
		
		public virtual PackageViewModel CreatePackageViewModel(IPackageFromRepository package)
		{
			return new PackageViewModel(
				package,
				SelectedProjects,
				PackageManagementEvents,
				PackageActionRunner,
				Logger);
		}
		
		public IPackageManagementSolution Solution { get; private set; }
		public PackageManagementSelectedProjects SelectedProjects { get; protected set; }
		public IPackageManagementEvents PackageManagementEvents { get; private set; }
		public ILogger Logger { get; private set; }
		public IPackageActionRunner PackageActionRunner { get; private set; }
	}
}

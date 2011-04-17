// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageViewModelFactory : IPackageViewModelFactory
	{
		public PackageViewModelFactory(IPackageViewModelFactory packageViewModelFactory)
			: this(
				packageViewModelFactory.Solution,
				packageViewModelFactory.PackageManagementEvents)
		{
		}
		
		public PackageViewModelFactory(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents)
		{
			this.Solution = solution;
			this.PackageManagementEvents = packageManagementEvents;
			this.Logger = new PackageManagementLogger(packageManagementEvents);
		}
		
		public virtual PackageViewModel CreatePackageViewModel(IPackageFromRepository package)
		{
			return new PackageViewModel(
				package,
				Solution,
				PackageManagementEvents,
				Logger);
		}
		
		public IPackageManagementSolution Solution { get; private set; }
		public IPackageManagementEvents PackageManagementEvents { get; private set; }
		public ILogger Logger { get; private set; }
	}
}

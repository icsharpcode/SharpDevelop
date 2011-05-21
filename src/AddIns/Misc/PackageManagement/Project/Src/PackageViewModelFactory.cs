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
				packageViewModelFactory.PackageScriptRunner)
		{
		}
		
		public PackageViewModelFactory(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents,
			IPackageScriptRunner scriptRunner)
		{
			this.Solution = solution;
			this.PackageManagementEvents = packageManagementEvents;
			this.PackageScriptRunner = scriptRunner;
			this.Logger = new PackageManagementLogger(packageManagementEvents);
		}
		
		public virtual PackageViewModel CreatePackageViewModel(IPackageFromRepository package)
		{
			return new PackageViewModel(
				package,
				Solution,
				PackageManagementEvents,
				PackageScriptRunner,
				Logger);
		}
		
		public IPackageManagementSolution Solution { get; private set; }
		public IPackageManagementEvents PackageManagementEvents { get; private set; }
		public ILogger Logger { get; private set; }
		public IPackageScriptRunner PackageScriptRunner { get; private set; }
	}
}

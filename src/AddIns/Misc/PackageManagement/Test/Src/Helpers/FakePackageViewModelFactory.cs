// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageViewModelFactory : IPackageViewModelFactory
	{
		public FakePackageManagementSolution FakeSolution = new FakePackageManagementSolution();
		public FakePackageManagementEvents FakePackageManagementEvents = new FakePackageManagementEvents();
		public FakeLogger FakeLogger = new FakeLogger();
		public FakePackageActionRunner FakeActionRunner = new FakePackageActionRunner();
		
		public PackageViewModel CreatePackageViewModel(IPackageViewModelParent parent, IPackageFromRepository package)
		{
			SelectedProjects = new PackageManagementSelectedProjects(FakeSolution);
			return new PackageViewModel(
				parent,
				package,
				SelectedProjects,
				FakePackageManagementEvents,
				FakeActionRunner,
				FakeLogger);
		}
		
		public IPackageManagementSolution Solution { 
			get { return FakeSolution; }
		}
		
		public PackageManagementSelectedProjects SelectedProjects { get; set; }
		
		public IPackageManagementEvents PackageManagementEvents {
			get { return FakePackageManagementEvents; }
		}
		
		public IPackageActionRunner PackageActionRunner {
			get { return FakeActionRunner; }
		}
		
		public ILogger Logger {
			get { return FakeLogger; }
		}
	}
}

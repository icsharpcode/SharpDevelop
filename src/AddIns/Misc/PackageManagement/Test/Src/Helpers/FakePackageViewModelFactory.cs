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
		public FakePackageScriptRunner FakeScriptRunner = new FakePackageScriptRunner();
		
		public PackageViewModel CreatePackageViewModel(IPackageFromRepository package)
		{
			return new PackageViewModel(
				package,
				FakeSolution,
				FakePackageManagementEvents,
				FakeScriptRunner,
				FakeLogger);
		}
		
		public IPackageManagementSolution Solution { 
			get { return FakeSolution; }
		}
		
		public IPackageManagementEvents PackageManagementEvents {
			get { return FakePackageManagementEvents; }
		}
		
		public IPackageScriptRunner PackageScriptRunner {
			get { return FakeScriptRunner; }
		}
	}
}

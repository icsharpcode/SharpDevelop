// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class TestableInstalledPackageViewModel : InstalledPackageViewModel
	{
		public FakePackageOperationResolver FakePackageOperationResolver = new FakePackageOperationResolver();
		public FakePackageManagementSolution FakeSolution;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackage FakePackage;
		public FakeLogger FakeLogger;
		public ILogger LoggerUsedWhenCreatingPackageResolver;
		public FakePackageActionRunner FakeActionRunner;
		
		public TestableInstalledPackageViewModel()
			: this(new FakePackageManagementSolution())
		{
		}
		
		public TestableInstalledPackageViewModel(FakePackageManagementSolution solution)
			: this(
				new FakePackage(),
				new SelectedProjectsForInstalledPackages(solution),
				new FakePackageManagementEvents(),
				new FakePackageActionRunner(),
				new FakeLogger())
		{
			this.FakeSolution = solution;
			solution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject("MyProject");
		}
		
		public TestableInstalledPackageViewModel(
			FakePackage package,
			SelectedProjectsForInstalledPackages selectedProjects,
			FakePackageManagementEvents packageManagementEvents,
			FakePackageActionRunner actionRunner,
			FakeLogger logger)
			: base(
				package,
				selectedProjects,
				packageManagementEvents,
				actionRunner,
				logger)
		{
			this.FakePackage = package;
			this.FakeActionRunner = actionRunner;
			this.FakeLogger = logger;
			this.FakePackageManagementEvents = packageManagementEvents;
		}
		
		public PackageViewModelOperationLogger OperationLoggerCreated;
		
		protected override PackageViewModelOperationLogger CreateLogger(ILogger logger)
		{
			OperationLoggerCreated = base.CreateLogger(logger);
			return OperationLoggerCreated;
		}
	}
}

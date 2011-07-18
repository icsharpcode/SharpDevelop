// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class TestableUpdatedPackageViewModel : UpdatedPackageViewModel
	{
		public FakePackageOperationResolver FakePackageOperationResolver = new FakePackageOperationResolver();
		public FakePackageManagementSolution FakeSolution;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackage FakePackage;
		public FakeLogger FakeLogger;
		public ILogger LoggerUsedWhenCreatingPackageResolver;
		public FakePackageActionRunner FakeActionRunner;
		
		public TestableUpdatedPackageViewModel()
			: this(new FakePackageManagementSolution())
		{
		}
		
		public TestableUpdatedPackageViewModel(FakePackageManagementSolution solution)
			: this(
				new FakePackage(),
				new PackageManagementSelectedProjects(solution),
				new FakePackageManagementEvents(),
				new FakePackageActionRunner(),
				new FakeLogger())
		{
			this.FakeSolution = solution;
			solution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject("MyProject");
		}
		
		public TestableUpdatedPackageViewModel(
			FakePackage package,
			PackageManagementSelectedProjects selectedProjects,
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
	}
}

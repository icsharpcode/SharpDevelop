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
		
		public TestableUpdatedPackageViewModel(
			IPackageViewModelParent parent,
			FakePackageManagementSolution solution)
			: this(
				parent,
				new FakePackage(),
				new SelectedProjectsForUpdatedPackages(solution),
				new FakePackageManagementEvents(),
				new FakePackageActionRunner(),
				new FakeLogger())
		{
			this.FakeSolution = solution;
		}
		
		public TestableUpdatedPackageViewModel(
			IPackageViewModelParent parent,
			FakePackage package,
			SelectedProjectsForUpdatedPackages selectedProjects,
			FakePackageManagementEvents packageManagementEvents,
			FakePackageActionRunner actionRunner,
			FakeLogger logger)
			: base(
				parent,
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

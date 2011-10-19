// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackageViewModel : PackageViewModel
	{
		public FakePackageManagementSolution FakeSolution;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackage FakePackage;
		public FakeLogger FakeLogger;
		public FakePackageActionRunner FakeActionRunner;
		
		public TestablePackageViewModel(FakePackageManagementSolution solution)
			: this(
				new FakePackage("Test"),
				new PackageManagementSelectedProjects(solution),
				new FakePackageManagementEvents(),
				new FakePackageActionRunner(),
				new FakeLogger())
		{
			this.FakeSolution = solution;
		}
		
		public TestablePackageViewModel(
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
			this.FakePackageManagementEvents = packageManagementEvents;
			this.FakeActionRunner = actionRunner;
			this.FakeLogger = logger;
		}
		
		protected override PackageViewModelOperationLogger CreateLogger(ILogger logger)
		{
			PackageViewModelOperationLogger operationLogger = base.CreateLogger(logger);
			operationLogger.AddingPackageMessageFormat = "Installing...{0}";
			operationLogger.RemovingPackageMessageFormat = "Uninstalling...{0}";
			operationLogger.ManagingPackageMessageFormat = "Managing...{0}";
			OperationLoggerCreated = operationLogger;
			return operationLogger;
		}
		
		public PackageViewModelOperationLogger OperationLoggerCreated;
		
		public PackageOperation AddOneFakeInstallPackageOperationForViewModelPackage()
		{
			var operation = new FakePackageOperation(FakePackage, PackageAction.Install);
			
			FakeSolution
				.FakeProjectToReturnFromGetProject
				.FakeInstallOperations
				.Add(operation);
			
			return operation;
		}
		
		public PackageOperation AddOneFakeUninstallPackageOperation()
		{
			var package = new FakePackage("PackageToUninstall");			
			var operation = new FakePackageOperation(package, PackageAction.Uninstall);
			FakeSolution.FakeProjectToReturnFromGetProject.FakeInstallOperations.Add(operation);
			return operation;
		}
	}
}

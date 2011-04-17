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
		public FakePackageManagementService	FakePackageManagementService;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackage FakePackage;
		public FakeLogger FakeLogger;
		
		public TestablePackageViewModel(FakePackageManagementService packageManagementService)
			: this(
				new FakePackage(),
				packageManagementService,
				new FakePackageManagementEvents(),
				new FakeLogger())
		{		
		}
		
		public TestablePackageViewModel(
			FakePackage package,
			FakePackageManagementService packageManagementService,
			FakePackageManagementEvents packageManagementEvents,
			FakeLogger logger)
			: base(
				package,
				packageManagementService,
				packageManagementEvents,
				logger)
		{
			this.FakePackage = package;
			this.FakePackageManagementService = packageManagementService;
			this.FakePackageManagementEvents = packageManagementEvents;
			this.FakeLogger = logger;
		}
		
		protected override PackageViewModelOperationLogger CreateLogger(ILogger logger)
		{
			PackageViewModelOperationLogger operationLogger = base.CreateLogger(logger);
			operationLogger.AddingPackageMessageFormat = "Installing...{0}";
			operationLogger.RemovingPackageMessageFormat = "Uninstalling...{0}";
			OperationLoggerCreated = operationLogger;
			return operationLogger;
		}
		
		public PackageViewModelOperationLogger OperationLoggerCreated;
		
		public PackageOperation AddOneFakeInstallPackageOperationForViewModelPackage()
		{
			var operation = new PackageOperation(FakePackage, PackageAction.Install);
			
			FakePackageManagementService
				.FakePackageManagerToReturnFromCreatePackageManager
				.PackageOperationsToReturnFromGetInstallPackageOperations
				.Add(operation);
			
			return operation;
		}
		
		public PackageOperation AddOneFakeUninstallPackageOperation()
		{
			var package = new FakePackage("PackageToUninstall");			
			var operation = new PackageOperation(package, PackageAction.Uninstall);
			
			FakePackageManagementService
				.FakePackageManagerToReturnFromCreatePackageManager
				.PackageOperationsToReturnFromGetInstallPackageOperations
				.Add(operation);
			
			return operation;
		}
	}
}

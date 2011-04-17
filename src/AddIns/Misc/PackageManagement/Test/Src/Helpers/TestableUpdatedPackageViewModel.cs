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
		public FakePackageManagementService	FakePackageManagementService;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackage FakePackage;
		public FakeLogger FakeLogger;
		public ILogger LoggerUsedWhenCreatingPackageResolver;
		
		public TestableUpdatedPackageViewModel()
			: this(new FakePackageManagementService())
		{
		}
		
		public TestableUpdatedPackageViewModel(FakePackageManagementService packageManagementService)
			: this(
				new FakePackage(),
				packageManagementService,
				new FakePackageManagementEvents(),
				new FakeLogger())
		{		
		}
		
		public TestableUpdatedPackageViewModel(
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
			this.FakeLogger = logger;
		}
	}
}

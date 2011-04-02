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
		public FakePackageRepository FakeSourcePackageRepository;
		public FakePackageManagementService	FakePackageManagementService;
		public FakeLicenseAcceptanceService	FakeLicenseAcceptanceService;
		public FakeMessageReporter FakeMessageReporter;
		public FakePackage FakePackage;
		public ILogger LoggerUsedWhenCreatingPackageResolver;
		public string PackageViewModelAddingPackageMessageFormat = String.Empty;
		public string PackageViewModelRemovingPackageMessageFormat = String.Empty;
		
		public TestableUpdatedPackageViewModel()
			: this(new FakePackageManagementService())
		{
		}
		
		public TestableUpdatedPackageViewModel(FakePackageManagementService packageManagementService)
			: this(
				new FakePackage(),
				packageManagementService,
				new FakeLicenseAcceptanceService(),
				new FakeMessageReporter())
		{		
		}
		
		public TestableUpdatedPackageViewModel(
			FakePackage package,
			FakePackageManagementService packageManagementService,
			FakeLicenseAcceptanceService licenseAcceptanceService,
			FakeMessageReporter messageReporter)
			: base(
				package,
				packageManagementService,
				licenseAcceptanceService,
				messageReporter)
		{
			this.FakePackage = package;
			this.FakePackageManagementService = packageManagementService;
			this.FakeLicenseAcceptanceService = licenseAcceptanceService;
			this.FakeMessageReporter = messageReporter;
			this.FakeSourcePackageRepository = FakePackageManagementService.FakeActivePackageRepository;
		}
		
		protected override IPackageOperationResolver CreatePackageOperationResolver(ILogger logger)
		{
			return FakePackageOperationResolver;
		}
		
		protected override string AddingPackageMessageFormat {
			get { return PackageViewModelAddingPackageMessageFormat; }
		}
		
		protected override string RemovingPackageMessageFormat {
			get { return PackageViewModelRemovingPackageMessageFormat; }
		}
	}
}

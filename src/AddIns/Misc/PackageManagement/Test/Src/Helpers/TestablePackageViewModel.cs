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
		public FakePackageOperationResolver FakePackageOperationResolver;
		public FakePackageRepository FakeSourcePackageRepository;
		public FakePackageManagementService	FakePackageManagementService;
		public FakeLicenseAcceptanceService	FakeLicenseAcceptanceService;
		public FakePackage FakePackage;
		
		public TestablePackageViewModel()
			: this(
				new FakePackage(),
				new FakePackageManagementService(),
				new FakeLicenseAcceptanceService())
		{
		}
		
		public TestablePackageViewModel(
			FakePackage package,
			FakePackageManagementService packageManagementService,
			FakeLicenseAcceptanceService licenseAcceptanceService)
			: base(
				package,
				packageManagementService,
				licenseAcceptanceService)
		{
			this.FakePackage = package;
			this.FakePackageManagementService = packageManagementService;
			this.FakeLicenseAcceptanceService = licenseAcceptanceService;
			this.FakeSourcePackageRepository = FakePackageManagementService.FakeActivePackageRepository;
		}
		
		protected override IPackageOperationResolver CreatePackageOperationResolver()
		{
			if (FakePackageOperationResolver != null) {
				return FakePackageOperationResolver;
			}
			return base.CreatePackageOperationResolver();
		}
	}
}

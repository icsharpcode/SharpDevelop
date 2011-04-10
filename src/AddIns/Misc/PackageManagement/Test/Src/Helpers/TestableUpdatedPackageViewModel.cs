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
		public FakePackageManagementEvents FakePackageManagementEvents;
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
				new FakePackageRepository(),
				packageManagementService,
				new FakePackageManagementEvents())
		{		
		}
		
		public TestableUpdatedPackageViewModel(
			FakePackage package,
			FakePackageRepository sourceRepository,
			FakePackageManagementService packageManagementService,
			FakePackageManagementEvents packageManagementEvents)
			: base(
				package,
				sourceRepository,
				packageManagementService,
				packageManagementEvents)
		{
			this.FakePackage = package;
			this.FakePackageManagementService = packageManagementService;
			this.FakeSourcePackageRepository = sourceRepository;
		}
		
		protected override string AddingPackageMessageFormat {
			get { return PackageViewModelAddingPackageMessageFormat; }
		}
		
		protected override string RemovingPackageMessageFormat {
			get { return PackageViewModelRemovingPackageMessageFormat; }
		}
	}
}

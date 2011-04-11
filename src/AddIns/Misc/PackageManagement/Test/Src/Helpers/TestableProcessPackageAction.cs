// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;

namespace PackageManagement.Tests.Helpers
{
	public class TestableProcessPackageAction : ProcessPackageAction
	{
		public FakePackageManagementService FakePackageManagementService;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackageRepository FakePackageRepository = new FakePackageRepository();
		public FakePackage FakePackage = new FakePackage("Test");
		
		public TestableProcessPackageAction()
			: this(new FakePackageManagementService(), new FakePackageManagementEvents())
		{
		}
		
		public TestableProcessPackageAction(
			FakePackageManagementService packageManagementService,
			FakePackageManagementEvents packageManagementEvents)
			: base(packageManagementService, packageManagementEvents)
		{
			FakePackageManagementService = packageManagementService;
			FakePackageManagementEvents = packageManagementEvents;
			this.PackageRepository = FakePackageRepository;
			this.Package = FakePackage;
		}
		
		public void CallBeforeExecute()
		{
			base.BeforeExecute();
		}
	}
}

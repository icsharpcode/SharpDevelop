// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;

namespace PackageManagement.Tests.Helpers
{
	public class TestableProcessPackageAction : ProcessPackageAction
	{
		public FakePackageManagementSolution FakeSolution;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackageRepository FakePackageRepository = new FakePackageRepository();
		public FakePackage FakePackage = new FakePackage("Test");
		
		public TestableProcessPackageAction()
			: this(new FakePackageManagementSolution(), new FakePackageManagementEvents())
		{
		}
		
		public TestableProcessPackageAction(
			FakePackageManagementSolution solution,
			FakePackageManagementEvents packageManagementEvents)
			: base(solution, packageManagementEvents)
		{
			FakeSolution = solution;
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

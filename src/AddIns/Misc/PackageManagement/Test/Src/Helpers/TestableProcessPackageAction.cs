// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;

namespace PackageManagement.Tests.Helpers
{
	public class TestableProcessPackageAction : ProcessPackageAction
	{
		public FakePackageManagementProject FakeProject;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackage FakePackage = new FakePackage("Test");
		
		public TestableProcessPackageAction()
			: this(new FakePackageManagementProject(), new FakePackageManagementEvents())
		{
		}
		
		public TestableProcessPackageAction(
			FakePackageManagementProject project,
			FakePackageManagementEvents packageManagementEvents)
			: base(project, packageManagementEvents)
		{
			FakeProject = project;
			FakePackageManagementEvents = packageManagementEvents;
			this.Package = FakePackage;
		}
		
		public void CallBeforeExecute()
		{
			base.BeforeExecute();
		}
	}
}

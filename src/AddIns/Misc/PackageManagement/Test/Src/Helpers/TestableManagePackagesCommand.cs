// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;

namespace PackageManagement.Tests.Helpers
{
	public class TestableManagePackagesCommand : ManagePackagesCommand
	{
		public FakePackageManagementOutputMessagesView FakeOutputMessagesView;
		public FakeManagePackagesView FakeManagePackagesView = new FakeManagePackagesView();
		
		public TestableManagePackagesCommand()
			: this(new FakePackageManagementOutputMessagesView())
		{
		}
		
		public TestableManagePackagesCommand(FakePackageManagementOutputMessagesView view)
			: base(view)
		{
			this.FakeOutputMessagesView = view;
		}
		
		protected override IManagePackagesView CreateManagePackagesView()
		{
			return FakeManagePackagesView;
		}
	}
}

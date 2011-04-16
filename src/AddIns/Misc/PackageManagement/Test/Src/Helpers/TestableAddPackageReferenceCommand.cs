// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;

namespace PackageManagement.Tests.Helpers
{
	public class TestableAddPackageReferenceCommand : AddPackageReferenceCommand
	{
		public FakePackageManagementOutputMessagesView FakeOutputMessagesView;
		public FakeAddPackageReferenceView FakeAddPackageReferenceView = new FakeAddPackageReferenceView();
		
		public TestableAddPackageReferenceCommand()
			: this(new FakePackageManagementOutputMessagesView())
		{
		}
		
		public TestableAddPackageReferenceCommand(FakePackageManagementOutputMessagesView view)
			: base(view)
		{
			this.FakeOutputMessagesView = view;
		}
		
		protected override IAddPackageReferenceView CreateAddPackageReferenceView()
		{
			return FakeAddPackageReferenceView;
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class PackageFilesViewIsActiveTests
	{
		MockWorkbench workbench;
		PackageFilesView packageFilesView;
		WixProject project;
		
		[SetUp]
		public void Init()
		{
			workbench = new MockWorkbench();
			project = WixBindingTestsHelper.CreateEmptyWixProject();
			packageFilesView = new PackageFilesView(project, workbench);
			
			workbench.ActiveViewContent = packageFilesView;
		}
		
		[Test]
		public void PackageFilesViewIsActive()
		{
			Assert.IsTrue(packageFilesView.IsActiveWindow);
		}
		
		[Test]
		public void PackageFilesViewIsNotActiveWhenWorkbenchActiveViewContentIsAnotherView()
		{
			workbench.ActiveViewContent = new PackageFilesView(project, workbench);
			Assert.IsFalse(packageFilesView.IsActiveWindow);
		}
	}
}

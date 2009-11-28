// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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

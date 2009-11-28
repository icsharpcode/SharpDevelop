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
	public class ActivePackageFilesViewTests
	{
		ActivePackageFilesView activePackageFilesView;
		MockWorkbench workbench;
		
		[SetUp]
		public void Init()
		{
			workbench = new MockWorkbench();
			activePackageFilesView = new ActivePackageFilesView(workbench);
		}
		
		[Test]
		public void GetActiveViewReturnsNullWhenActiveContentIsNotPackageFilesView()
		{
			workbench.ActiveContent = new MockViewContent();
			Assert.IsNull(activePackageFilesView.GetActiveView());
		}
		
		[Test]
		public void GetActiveViewReturnsPackageFilesViewWhenActiveContentIsPackageFilesView()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			using (PackageFilesView packageFilesView = new PackageFilesView(project, workbench)) {
				workbench.ActiveContent = packageFilesView;
				Assert.AreSame(packageFilesView, activePackageFilesView.GetActiveView());
			}
		}
	}
}

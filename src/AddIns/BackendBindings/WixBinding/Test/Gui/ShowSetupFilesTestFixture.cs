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

namespace WixBinding.Tests.Gui
{
	[TestFixture]
	public class ShowSetupFilesTestFixture
	{
		MockWorkbench workbench;
		MockPackageFilesViewFactory factory;
		WixProject project;
		
		[SetUp]
		public void Init()
		{
			workbench = new MockWorkbench();
			factory = new MockPackageFilesViewFactory();
			project = WixBindingTestsHelper.CreateEmptyWixProject();
			
			ViewSetupFilesCommand viewCommand = new ViewSetupFilesCommand(factory, workbench);
			viewCommand.Run(project);
		}
		
		[Test]
		public void NewPackageFilesViewObjectCreated()
		{
			Assert.IsNotNull(factory.PackageFilesViewCreated);
		}
		
		[Test]
		public void WorkbenchPassedToPackageFilesViewFactory()
		{
			Assert.AreSame(workbench, factory.CreateMethodWorkbenchParameter);
		}
		
		[Test]
		public void PackageFilesViewShowFilesMethodCalled()
		{
			Assert.AreSame(project, factory.PackageFilesControlCreated.ShowFilesMethodProjectParameter);
		}
		
		[Test]
		public void PackageFilesViewDisplayedInWorkbench()
		{
			Assert.IsTrue(workbench.ViewContentCollection.Contains(factory.PackageFilesViewCreated));
		}
	}
}

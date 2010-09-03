// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

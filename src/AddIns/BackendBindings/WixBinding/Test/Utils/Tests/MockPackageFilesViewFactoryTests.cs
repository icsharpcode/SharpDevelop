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

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockPackageFilesViewFactoryTests
	{
		PackageFilesView view;
		MockPackageFilesViewFactory factory;
		WixProject project;
		MockWorkbench workbench;
		
		[SetUp]
		public void Init()
		{
			project = WixBindingTestsHelper.CreateEmptyWixProject();
			factory = new MockPackageFilesViewFactory();
			workbench = new MockWorkbench();
			
			view = factory.Create(project, workbench);
		}
		
		[Test]
		public void ViewCreatedIsSaved()
		{
			Assert.AreSame(view, factory.PackageFilesViewCreated);
		}
		
		[Test]
		public void ViewCreatedIsNotNull()
		{
			Assert.IsNotNull(view);
		}
		
		[Test]
		public void ProjectParameterUsedInCreateMethodCallIsSaved()
		{
			Assert.AreSame(project, factory.CreateMethodProjectParameter);
		}
		
		[Test]
		public void WorkbenchParameterUsedInCreateMethodCallIsSaved()
		{
			Assert.AreSame(workbench, factory.CreateMethodWorkbenchParameter);
		}
		
		[Test]
		public void PackageFilesCreatedWithMockWixPackageFilesControl()
		{
			view.AddFiles();
			Assert.IsTrue(factory.PackageFilesControlCreated.AddFilesMethodCalled);
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	[TestFixture]
	public class ShowExistingPackageFilesViewTestFixture
	{
		MockWorkbench workbench;
		MockPackageFilesViewFactory factory;
		WixProject project;
		PackageFilesView existingView;
		MockWixPackageFilesControl packageFilesControl;
		
		[SetUp]
		public void Init()
		{
			workbench = new MockWorkbench();
			
			CreatePackageFilesViewWithDifferentWixProject();
			CreatePackageFilesViewWithProjectToBeUsedWithViewSetupFilesCommandRunMethod();
			
			factory = new MockPackageFilesViewFactory();
			ViewSetupFilesCommand viewCommand = new ViewSetupFilesCommand(factory, workbench);
			viewCommand.Run(project);
		}
		
		void CreatePackageFilesViewWithDifferentWixProject()
		{
			WixProject differentProject = WixBindingTestsHelper.CreateEmptyWixProject();
			PackageFilesView viewWithDifferentProject = new PackageFilesView(differentProject, workbench, new MockWixPackageFilesControl());
			
			workbench.ShowView(viewWithDifferentProject);
			
			CreateWorkbenchWindowForPackageFilesView(viewWithDifferentProject);
		}
		
		void CreatePackageFilesViewWithProjectToBeUsedWithViewSetupFilesCommandRunMethod()
		{
			project = WixBindingTestsHelper.CreateEmptyWixProject();
			packageFilesControl = new MockWixPackageFilesControl();
			existingView = new PackageFilesView(project, workbench, packageFilesControl);
			
			workbench.ShowView(existingView);

			CreateWorkbenchWindowForPackageFilesView(existingView);
		}
		
		void CreateWorkbenchWindowForPackageFilesView(PackageFilesView view)
		{
			MockWorkbenchWindow window = new MockWorkbenchWindow();
			IViewContent viewContent = (IViewContent)view;
			viewContent.WorkbenchWindow = window;
		}
		
		[Test]
		public void ExistingPackageFilesViewWorkbenchWindowIsSelected()
		{
			MockWorkbenchWindow window = existingView.WorkbenchWindow as MockWorkbenchWindow;
			Assert.IsTrue(window.SelectWindowMethodCalled);
		}
	}
}

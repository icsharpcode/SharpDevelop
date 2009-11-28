// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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

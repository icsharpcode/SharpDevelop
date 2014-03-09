// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
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
			SD.InitializeForUnitTests();
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

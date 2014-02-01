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
			SD.InitializeForUnitTests();
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

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
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.VisualStudio;
using NuGet.VisualStudio;
using NuGetConsole;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.VisualStudio
{
	[TestFixture]
	public class ComponentModelTests
	{
		ComponentModel model;
		FakePackageManagementConsoleHost fakeConsoleHost;
		FakePackageManagementSolution fakeSolution;
		
		void CreateComponentModel()
		{
			fakeConsoleHost = new FakePackageManagementConsoleHost();
			fakeSolution = new FakePackageManagementSolution();
			model = new ComponentModel(fakeConsoleHost, fakeSolution);
		}
		
		[Test]
		public void GetService_GetIConsoleInitializer_ReturnsConsoleInitializer()
		{
			CreateComponentModel();
			
			IConsoleInitializer initializer = model.GetService<IConsoleInitializer>();
			
			Assert.IsNotNull(initializer);
		}
		
		[Test]
		public void GetService_GetIVsPackageInstallerServices_ReturnsVsPackageInstallerServices()
		{
			CreateComponentModel();
			
			IVsPackageInstallerServices installer = model.GetService<IVsPackageInstallerServices>();
			
			Assert.IsNotNull(installer);
		}
	}
}

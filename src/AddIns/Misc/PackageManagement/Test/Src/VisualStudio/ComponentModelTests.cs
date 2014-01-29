// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

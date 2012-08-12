// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.VisualStudio;
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
		
		void CreateComponentModel()
		{
			fakeConsoleHost = new FakePackageManagementConsoleHost();
			model = new ComponentModel(fakeConsoleHost);
		}
		
		[Test]
		public void GetService_GetIConsoleInitializer_ReturnsConsoleInitializer()
		{
			CreateComponentModel();
			
			IConsoleInitializer initializer = model.GetService<IConsoleInitializer>();
			
			Assert.IsNotNull(initializer);
		}
	}
}

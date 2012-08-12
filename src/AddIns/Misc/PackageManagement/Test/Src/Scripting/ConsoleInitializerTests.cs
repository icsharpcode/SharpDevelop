// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class ConsoleInitializerTests
	{
		ConsoleInitializer initializer;
		FakePackageManagementConsoleHost fakeConsoleHost;
		
		void CreateConsoleInitializer()
		{
			fakeConsoleHost = new FakePackageManagementConsoleHost();
			initializer = new ConsoleInitializer(fakeConsoleHost);
		}
		
		[Test]
		public void Initialize_RunInitializeTask_SetsDefaultRunspaceForConsoleHost()
		{
			CreateConsoleInitializer();
			
			initializer.Initialize().Result();
			
			Assert.IsTrue(fakeConsoleHost.IsSetDefaultRunspaceCalled);
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageManagementConsoleHostProviderTests
	{
		PackageManagementConsoleHostProvider provider;
		FakePackageManagementSolution fakeSolution;
		FakeRegisteredPackageRepositories fakeRegisteredRepositories;
		FakePowerShellDetection fakePowerShellDetection;
		
		void CreateProvider()
		{
			fakeSolution = new FakePackageManagementSolution();
			fakeRegisteredRepositories = new FakeRegisteredPackageRepositories();
			fakePowerShellDetection = new FakePowerShellDetection();
			provider = new PackageManagementConsoleHostProvider(
				fakeSolution,
				fakeRegisteredRepositories,
				fakePowerShellDetection);
		}
		
		[Test]
		public void ConsoleHost_PowerShellIsInstalled_ReturnsPackageManagementConsoleHost()
		{
			CreateProvider();
			fakePowerShellDetection.IsPowerShell2InstalledReturnValue = true;
			
			IPackageManagementConsoleHost consoleHost = provider.ConsoleHost;
			
			Assert.IsInstanceOf(typeof(PackageManagementConsoleHost), consoleHost);
		}
		
		[Test]
		public void ConsoleHost_PowerShellIsNotInstalled_ReturnsPowerShellMissingConsoleHost()
		{
			CreateProvider();
			fakePowerShellDetection.IsPowerShell2InstalledReturnValue = false;
			
			IPackageManagementConsoleHost consoleHost = provider.ConsoleHost;
			
			Assert.IsInstanceOf(typeof(PowerShellMissingConsoleHost), consoleHost);
		}
	}
}

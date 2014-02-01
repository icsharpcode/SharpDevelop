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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.VisualStudio;
using NuGet;
using NuGet.VisualStudio;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests.VisualStudio
{
	[TestFixture]
	public class VsPackageInstallerServicesTests
	{
		VsPackageInstallerServices installerServices;
		IPackageManagementSolution fakeSolution;
		List<IPackage> installedPackages;
		
		void CreatePackageInstallerServices()
		{
			fakeSolution = MockRepository.GenerateStub<IPackageManagementSolution>();
			installedPackages = new List<IPackage>();
			fakeSolution.Stub(s => s.GetPackages()).Return(installedPackages.AsQueryable());
			installerServices = new VsPackageInstallerServices(fakeSolution);
		}
		
		[Test]
		public void GetInstalledPackages_NoInstalledPackages_ReturnsNoPackages()
		{
			CreatePackageInstallerServices();
			
			List<IVsPackageMetadata> packages = installerServices.GetInstalledPackages().ToList();
			
			Assert.AreEqual(0, packages.Count);
		}
		
		[Test]
		public void GetInstalledPackages_OneInstalledPackages_ReturnsOnePackage()
		{
			CreatePackageInstallerServices();
			string installPath = @"d:\projects\MyProject\packages\TestPackage";
			IPackage installedPackage = AddPackage("Id", "1.1", installPath);
			
			List<IVsPackageMetadata> packages = installerServices.GetInstalledPackages().ToList();
			
			IVsPackageMetadata package = packages[0];
			Assert.AreEqual(1, packages.Count);
			Assert.AreEqual("Id", package.Id);
			Assert.AreEqual("1.1", package.Version.ToString());
			Assert.AreEqual(@"d:\projects\MyProject\packages\TestPackage", package.InstallPath);
		}
		
		IPackage AddPackage(string id, string version, string installPath)
		{
			var package = new FakePackage(id, version);
			installedPackages.Add(package);
			fakeSolution.Stub(s => s.GetInstallPath(package)).Return(installPath);
			return package;
		}
	}
}

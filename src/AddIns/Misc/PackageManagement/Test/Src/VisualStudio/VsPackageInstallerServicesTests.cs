// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

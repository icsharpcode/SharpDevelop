// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class DesignTimePackageManagementService : FakePackageManagementService
	{
		public DesignTimePackageManagementService()
		{
			AddDesignTimePackages();
		}
		
		void AddDesignTimePackages()
		{
			for (int i = 0; i < 5; ++i) {
				FakePackage package = CreatePackage(i);
				FakeActiveProjectManager.FakeLocalRepository.FakePackages.Add(package);
				FakeActivePackageRepository.FakePackages.Add(package);
			}
		}
		
		FakePackage CreatePackage(int i)
		{
			var package = new FakePackage();
			package.Id = "Package ID: " + i;
			package.Description = "Package description.";
			package.Summary = "Package summary. Package summary. Package summary. " +
				"Package summary. Package summary. Package summary. Package summary.";
			package.DownloadCount = i;
			package.Rating = 4.5;
			package.RatingsCount = 344;
			package.RequireLicenseAcceptance = true;
			package.LicenseUrl = new Uri("http://www.google.com/license");
			package.ProjectUrl = new Uri("http://www.codeplex.com");
			package.ReportAbuseUrl = new Uri("http://www.google.com");
			package.Version = Version.Parse("1.0.4.5");
			package.AddAuthor("A User");
			package.AddAuthor("B User");
			package.AddDependency("NuGet.Package." + i, Version.Parse("1.0.0.1"), Version.Parse("1.2.0.2"));
			package.AddDependency("NuGet.Package." + i + 1, Version.Parse("1.2.0.2"), Version.Parse("2.2.0.0"));
			return package;
		}
	}
}

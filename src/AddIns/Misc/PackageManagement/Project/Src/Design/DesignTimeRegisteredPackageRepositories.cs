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
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class DesignTimeRegisteredPackageRepositories : FakeRegisteredPackageRepositories
	{
		public DesignTimeRegisteredPackageRepositories()
		{
			AddDesignTimePackages();
			ActivePackageSource = new PackageSource("http://nuget.org", "NuGet Official Package Source");
		}
		
		void AddDesignTimePackages()
		{
			for (int i = 0; i < 5; ++i) {
				FakePackage package = CreatePackage(i);
				FakeActiveRepository.FakePackages.Add(package);
			}
		}
		
		FakePackage CreatePackage(int i)
		{
			var package = new FakePackage();
			package.Id = "Package ID: " + i;
			package.Description = "Package description.";
			package.Summary =
				"Package summary. Package summary. Package summary. " +
				"Package summary. Package summary. Package summary. Package summary.";
			package.DownloadCount = i;
			package.Rating = 4.5;
			package.RatingsCount = 344;
			package.RequireLicenseAcceptance = true;
			package.LicenseUrl = new Uri("http://www.google.com/license");
			package.ProjectUrl = new Uri("http://www.codeplex.com");
			package.ReportAbuseUrl = new Uri("http://www.google.com");
			package.Version = SemanticVersion.Parse("1.0.4.5");
			package.LastUpdated = new DateTime(2011, 1, 2);
			package.AddAuthor("A User");
			package.AddAuthor("B User");
			package.AddDependency("NuGet.Package." + i, SemanticVersion.Parse("1.0.0.1"), SemanticVersion.Parse("1.2.0.2"));
			package.AddDependency("NuGet.Package." + i + 1, SemanticVersion.Parse("1.2.0.2"), SemanticVersion.Parse("2.2.0.0"));
			return package;
		}
	}
}

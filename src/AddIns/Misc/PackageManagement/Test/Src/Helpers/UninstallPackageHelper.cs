// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class UninstallPackageHelper
	{
		UninstallPackageAction action;
		
		public UninstallPackageHelper(UninstallPackageAction action)
		{
			this.action = action;
		}
		
		public FakePackage TestPackage = new FakePackage() {
			Id = "Test"
		};
		
		public void UninstallTestPackage()
		{
			action.Package = TestPackage;
			action.Execute();
		}
		
		public SemanticVersion Version;
		public PackageSource PackageSource = new PackageSource("http://sharpdevelop.net");
		public bool ForceRemove;
		public bool RemoveDependencies;
		
		public void UninstallPackageById(string packageId)
		{
			action.PackageId = packageId;
			action.PackageVersion = Version;
			action.ForceRemove = ForceRemove;
			action.RemoveDependencies = RemoveDependencies;
			action.Execute();
		}
	}
}

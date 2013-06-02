// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class TestPackageHelper
	{
		public TestPackageHelper()
		{
			Package = MockRepository.GenerateStub<IPackage>();
		}
		
		public TestPackageHelper(string id, string version)
			: this()
		{
			SetId(id);
			SetVersion(version);
		}
		
		public IPackage Package { get; set; }
		
		public void SetId(string id)
		{
			Package.Stub(p => p.Id).Return(id);
		}
		
		public void SetVersion(string version)
		{
			Package.Stub(p => p.Version).Return(new SemanticVersion(version));
		}
		
		public void IsLatestVersion()
		{
			Package.Stub(p => p.IsLatestVersion).Return(true);
		}
		
		public void Listed()
		{
			Package.Stub(p => p.Listed).Return(true);
		}
	}
}

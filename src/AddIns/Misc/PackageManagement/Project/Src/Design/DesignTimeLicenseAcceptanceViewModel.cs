// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class DesignTimeLicenseAcceptanceViewModel : LicenseAcceptanceViewModel
	{
		public DesignTimeLicenseAcceptanceViewModel()
			: base(CreatePackages())
		{
		}
		
		static IEnumerable<IPackage> CreatePackages()
		{
			var repositories = new DesignTimeRegisteredPackageRepositories();
			return repositories.FakeActiveRepository.FakePackages;
		}
	}
}

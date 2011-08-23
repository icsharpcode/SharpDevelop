// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class DesignTimePackagesViewModel : PackagesViewModel
	{
		public DesignTimePackagesViewModel()
			: this(new DesignTimeRegisteredPackageRepositories(), new FakePackageManagementSolution())
		{
		}
		
		public DesignTimePackagesViewModel(
			DesignTimeRegisteredPackageRepositories registeredPackageRepositories,
			FakePackageManagementSolution solution)
			: base(
				registeredPackageRepositories,
				new PackageViewModelFactory(solution, null, null),
				new PackageManagementTaskFactory())
		{
			PageSize = 3;
			AddPackageViewModels();
		}
		
		void AddPackageViewModels()
		{
			IQueryable<IPackage> packages = RegisteredPackageRepositories.ActiveRepository.GetPackages();
			PackageViewModels.AddRange(ConvertToPackageViewModels(packages));
		}
	}
}

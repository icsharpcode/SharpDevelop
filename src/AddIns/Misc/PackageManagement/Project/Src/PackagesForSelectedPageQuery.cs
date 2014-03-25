// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackagesForSelectedPageQuery
	{
		public PackagesForSelectedPageQuery (
			PackagesViewModel viewModel,
			IEnumerable<IPackage> allPackages,
			string searchCriteria)
		{
			Skip = viewModel.ItemsBeforeFirstPage;
			Take = viewModel.PageSize;
			AllPackages = allPackages;
			SearchCriteria = searchCriteria;
			TotalPackages = viewModel.TotalItems;
		}

		public int Skip { get; private set; }
		public int Take { get; private set; }
		public string SearchCriteria { get; private set; }

		public int TotalPackages { get; set; }
		public IEnumerable<IPackage> AllPackages { get; set; }
	}
}

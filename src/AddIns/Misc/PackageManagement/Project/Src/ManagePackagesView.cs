// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.PackageManagement
{
	public partial class ManagePackagesView : Window, IManagePackagesView
	{
		public ManagePackagesView()
		{
			InitializeComponent();
		}
		
		public void Dispose()
		{
			var viewModel = DataContext as ManagePackagesViewModel;
			viewModel.Dispose();
		}
	}
}
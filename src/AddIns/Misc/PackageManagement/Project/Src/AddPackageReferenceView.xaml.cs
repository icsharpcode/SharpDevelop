// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.PackageManagement
{
	public partial class AddPackageReferenceView : Window, IAddPackageReferenceView
	{
		public AddPackageReferenceView()
		{
			InitializeComponent();
		}
		
		public void Dispose()
		{
			var viewModel = MainPanel.DataContext as AddPackageReferenceViewModel;
			viewModel.Dispose();
		}
	}
}
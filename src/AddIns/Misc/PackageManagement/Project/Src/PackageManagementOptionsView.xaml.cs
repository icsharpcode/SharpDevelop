// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public partial class PackageManagementOptionsView : OptionPanel
	{
		const string ViewModelResourceName = "PackageManagementOptionsViewModel";
		PackageManagementOptionsViewModel viewModel;
			
		public PackageManagementOptionsView()
		{
			InitializeComponent();
		}
		
		PackageManagementOptionsViewModel ViewModel { 
			get {
				if (viewModel == null) {
					viewModel = MainGrid.DataContext as PackageManagementOptionsViewModel;
				}
				return viewModel;
			}
		}
		
		public override void LoadOptions()
		{
			ViewModel.Load();
		}
		
		public override bool SaveOptions()
		{
			ViewModel.Save();
			return true;
		}
	}
}
// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AddInManager2.ViewModel;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AddInManager2.View
{
	/// <summary>
	/// Interaction logic for PackageRepositoriesView.xaml
	/// </summary>
	public partial class PackageRepositoriesView : OptionPanel
	{
		PackageRepositoriesViewModel viewModel;
		
		public PackageRepositoriesView()
		{
			InitializeComponent();
		}
		
		private PackageRepositoriesViewModel ViewModel
		{
			get
			{
				if (viewModel == null)
				{
					viewModel = DataContext as PackageRepositoriesViewModel;
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
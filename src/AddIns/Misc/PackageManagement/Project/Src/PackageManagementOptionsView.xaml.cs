// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public partial class PackageManagementOptionsView : OptionPanel
	{
		public PackageManagementOptionsView()
		{
			InitializeComponent();
		}
		
		public override bool SaveOptions()
		{
			var viewModel = DataContext as PackageManagementOptionsViewModel;
			viewModel.SaveOptions();
			return true;
		}
	}
}
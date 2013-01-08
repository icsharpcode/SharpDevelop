// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.AddInManager2.ViewModel;

namespace ICSharpCode.AddInManager2.View
{
	public partial class AddInManagerView : Window, IDisposable
	{
		public AddInManagerView()
		{
			InitializeComponent();
			
			ICSharpCode.SharpDevelop.Gui.FormLocationHelper.ApplyWindow(this, "AddInManager2.WindowBounds", true);
		}
		
		public void Dispose()
		{
			var viewModel = DataContext as AddInManagerViewModel;
			if (viewModel != null)
			{
				viewModel.Dispose();
			}
		}
	}
}
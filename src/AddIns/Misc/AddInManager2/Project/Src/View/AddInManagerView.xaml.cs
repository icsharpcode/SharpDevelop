// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.SharpDevelop;
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
		
		/// <summary>
		/// Creates a new <see cref="ICSharpCode.AddInManager2.View.AddInManagerView"/> instance.
		/// </summary>
		/// <returns>New <see cref="ICSharpCode.AddInManager2.View.AddInManagerView"/> instance.</returns>
		public static AddInManagerView Create()
		{
			return new AddInManagerView()
			{
				Owner = SD.Workbench.MainWindow
			};
		}
		
		public AddInManagerViewModel ViewModel
		{
			get
			{
				return DataContext as AddInManagerViewModel;
			}
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
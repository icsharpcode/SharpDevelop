// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.PackageManagement
{
	public partial class FileConflictView : Window
	{
		FileConflictViewModel viewModel;
		
		public FileConflictView()
		{
			InitializeComponent();
		}
		
		public FileConflictViewModel ViewModel {
			get { return viewModel; }
			set {
				viewModel = value;
				viewModel.Close += CloseView;
				DataContext = viewModel;
			}
		}
		
		void CloseView(object sender, EventArgs e)
		{
			DialogResult = true;
		}
	}
}
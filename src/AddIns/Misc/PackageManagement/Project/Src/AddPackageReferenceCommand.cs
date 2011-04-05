// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class AddPackageReferenceCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			var view = new AddPackageReferenceView();
			view.Owner = WorkbenchSingleton.MainWindow;
			view.ShowDialog();
			
			var viewModel = view.MainPanel.DataContext as AddPackageReferenceViewModel;
			viewModel.Dispose();
		}
	}
}

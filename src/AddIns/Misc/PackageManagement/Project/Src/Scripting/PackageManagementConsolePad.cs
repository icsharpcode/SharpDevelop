// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementConsolePad : AbstractPadContent
	{
		PackageManagementConsoleView view;
		PackageManagementConsoleViewModel viewModel;
		
		public override object Control {
			get {
				if (view == null) {
					view = new PackageManagementConsoleView();
					viewModel = view.DataContext as PackageManagementConsoleViewModel;
				}
				return view;
			}
		}
		
		public override void Dispose()
		{
			if (viewModel != null) {
				viewModel.Dispose();
				viewModel = null;
			}
		}
	}
}

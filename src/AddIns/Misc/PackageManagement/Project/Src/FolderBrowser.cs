// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class FolderBrowser : IFolderBrowser
	{
		public string SelectFolder()
		{
			using (var dialog = new FolderBrowserDialog()) {
				IWin32Window owner = WorkbenchSingleton.MainWin32Window;
				if (dialog.ShowDialog(owner) == DialogResult.OK) {
					return dialog.SelectedPath;
				}
			}
			return null;
		}
	}
}

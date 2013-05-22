// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.WinForms
{
	[SDService("SD.WinForms.ToolbarService")]
	public interface IWinFormsToolbarService
	{
		ToolStripItem[] CreateToolStripItems(string path, object parameter, bool throwOnNotFound);
		ToolStrip CreateToolStrip(object parameter, string addInTreePath);
		
		void UpdateToolbar(ToolStrip toolStrip);
		void UpdateToolbarText(ToolStrip toolStrip);
	}
}

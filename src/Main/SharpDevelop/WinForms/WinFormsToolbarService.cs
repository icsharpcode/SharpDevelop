// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.WinForms
{
	sealed class WinFormsToolbarService : IWinFormsToolbarService
	{
		public ToolStripItem[] CreateToolStripItems(string path, object parameter, bool throwOnNotFound)
		{
			return ToolbarService.CreateToolStripItems(path, parameter, throwOnNotFound);
		}
		
		public ToolStrip CreateToolStrip(object parameter, string addInTreePath)
		{
			return ToolbarService.CreateToolStrip(parameter, addInTreePath);
		}
		
		public void UpdateToolbar(ToolStrip toolStrip)
		{
			ToolbarService.UpdateToolbar(toolStrip);
		}
		
		public void UpdateToolbarText(ToolStrip toolStrip)
		{
			ToolbarService.UpdateToolbarText(toolStrip);
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.WinForms
{
	sealed class WinFormsMenuService : IWinFormsMenuService
	{
		public System.Windows.Forms.ContextMenuStrip CreateContextMenu(object parameter, string addInTreePath)
		{
			return MenuService.CreateContextMenu(parameter, addInTreePath);
		}
		
		public void ShowContextMenu(object parameter, string addInTreePath, Control parent, int x, int y)
		{
			MenuService.ShowContextMenu(parameter, addInTreePath, parent, x, y);
		}
		
		public void CreateQuickInsertMenu(TextBoxBase targetControl, Control popupControl, string[,] quickInsertMenuItems)
		{
			MenuService.CreateQuickInsertMenu(targetControl, popupControl, quickInsertMenuItems);
		}
	}
}

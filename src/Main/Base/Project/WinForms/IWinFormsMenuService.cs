// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.WinForms
{
	[SDService("SD.WinForms.MenuService")]
	public interface IWinFormsMenuService
	{
		/// <summary>
		/// Creates a new context menu from the AddIn Tree.
		/// </summary>
		/// <param name="parameter">The parameter that will be passed to the commands.</param>
		/// <param name="addInTreePath">The AddIn tree path containing the &lt;MenuItem&gt;s</param>
		ContextMenuStrip CreateContextMenu(object parameter, string addInTreePath);
		
		void ShowContextMenu(object parameter, string addInTreePath, Control parent, int x, int y);
		
		void CreateQuickInsertMenu(TextBoxBase targetControl, Control popupControl, string[,] quickInsertMenuItems);
	}
}

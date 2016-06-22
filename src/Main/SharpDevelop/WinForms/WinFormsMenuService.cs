// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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

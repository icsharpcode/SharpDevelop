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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin
{
	public class UIService : System.Windows.Forms.Design.IUIService
	{
		IDictionary styles = new Hashtable();
		
		public IDictionary Styles {
			get {
				return styles;
			}
		}
		
		public UIService()
		{
			styles["DialogFont"]     = Control.DefaultFont;
			styles["HighlightColor"] = Color.LightYellow;
		}
		
		public void SetUIDirty()
		{
			
		}
		
		#region ComponentEditor functions
		public bool CanShowComponentEditor(object component)
		{
			return false;
		}
		
		public bool ShowComponentEditor(object component, IWin32Window parent)
		{
			throw new System.NotImplementedException("Cannot display component editor for " + component);
		}
		#endregion
		
		#region Dialog functions
		public IWin32Window GetDialogOwnerWindow()
		{
			return SD.WinForms.MainWin32Window;
		}
		
		public DialogResult ShowDialog(Form form)
		{
			return form.ShowDialog(GetDialogOwnerWindow());
		}
		#endregion
		
		#region Show error functions
		public void ShowError(Exception ex)
		{
			MessageService.ShowError(ex.ToString());
		}
		
		public void ShowError(string message)
		{
			MessageService.ShowError(message);
		}
		
		public void ShowError(Exception ex, string message)
		{
			MessageService.ShowError(message + Environment.NewLine + ex.ToString());
		}
		#endregion
		
		#region Show Message functions
		public void ShowMessage(string message)
		{
			ShowMessage(message, "", MessageBoxButtons.OK);
		}
		
		public void ShowMessage(string message, string caption)
		{
			ShowMessage(message, caption, MessageBoxButtons.OK);
		}
		
		public DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons)
		{
			return MessageBox.Show(GetDialogOwnerWindow(), message, caption, buttons);
		}
		#endregion
		
		public bool ShowToolWindow(Guid toolWindow)
		{
			return false;
		}
	}
}

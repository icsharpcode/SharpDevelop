// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.FormsDesigner.Services
{
	public class UIService : MarshalByRefObject, IUIService
	{
		IFormsDesigner designer;
		IServiceProvider provider;
		ISharpDevelopIDEService messenger;
		IDictionary styles = new Hashtable();
		
		public IDictionary Styles {
			get {
				return styles;
			}
		}
		
		public UIService(IFormsDesigner designer, IServiceProvider provider)
		{
			this.designer = designer;
			this.provider = provider;
			messenger = (ISharpDevelopIDEService)provider.GetService(typeof(ISharpDevelopIDEService));
			styles["DialogFont"]     = Control.DefaultFont;
			styles["HighlightColor"] = Color.LightYellow;
		}
		
		public void SetUIDirty()
		{
			// TODO: Fixme!
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
			return new Win32Window { Handle = designer.GetDialogOwnerWindowHandle() };
		}
		
		[Serializable]
		sealed class Win32Window : IWin32Window
		{
			public IntPtr Handle { get; set; }
		}
		
		public DialogResult ShowDialog(Form form)
		{
			return form.ShowDialog(GetDialogOwnerWindow());
		}
		#endregion
		
		#region Show error functions
		public void ShowError(Exception ex)
		{
			messenger.ShowError(ex.ToString());
		}
		
		public void ShowError(string message)
		{
			messenger.ShowError(message);
		}
		
		public void ShowError(Exception ex, string message)
		{
			messenger.ShowError(message + Environment.NewLine + ex.ToString());
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

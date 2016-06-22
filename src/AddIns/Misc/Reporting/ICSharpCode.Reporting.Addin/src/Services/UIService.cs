/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.02.2014
 * Time: 18:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reporting.Addin.Services
{
	/// <summary>
	/// Description of UIService.
	/// </summary>
	class UIService : System.Windows.Forms.Design.IUIService
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

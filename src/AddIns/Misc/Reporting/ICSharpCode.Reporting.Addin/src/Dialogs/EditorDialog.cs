// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

using ICSharpCode.Reporting.Addin.Dialogs;

namespace ICSharpCode.Reports.Core.Dialogs
{
	/// <summary>
	/// Description of EditorDialog.
	/// </summary>
	/// 
	
	
	public partial class TextEditorDialog : Form,IStringBasedEditorDialog
	{
		string textValue;
	
		
		private TextEditorDialog()
		{
			InitializeComponent();
		}
		
		
		public TextEditorDialog(string text,string header):this()
		{
			this.textValue = text;
			this.textBox1.Text = this.textValue;
			this.Text = String.IsNullOrEmpty(header) ? this.Name : header;
		}
		
		
		public string TextValue {
			get { 
				return textValue.Trim();
			}
		}
		
		
		void OkButtonClick(object sender, EventArgs e)
		{
			textValue = this.textBox1.Text;
		}
		
	}
}

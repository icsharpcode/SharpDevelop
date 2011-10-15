// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Reports.Core.Interfaces;

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
			if (String.IsNullOrEmpty (header)) {
				this.Text = this.Name;
			} else {
				this.Text = header;
			}
		}
		
		
		public string TextValue {
			get { 
				return textValue.Trim();
			}
		}
		
		
		void OkButtonClick(object sender, EventArgs e)
		{
			this.textValue = this.textBox1.Text;
		}
		
	}
}

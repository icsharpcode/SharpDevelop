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

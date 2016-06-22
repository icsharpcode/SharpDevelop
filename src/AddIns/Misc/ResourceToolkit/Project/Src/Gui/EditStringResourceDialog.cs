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
using System.ComponentModel;
using System.Windows.Forms;

using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace Hornung.ResourceToolkit.Gui
{
	/// <summary>
	/// A dialog where the user can edit a string resource key and value.
	/// </summary>
	public class EditStringResourceDialog : BaseSharpDevelopForm
	{
		
		readonly IResourceFileContent content;
		
		public EditStringResourceDialog(IResourceFileContent content, string key, string value, bool allowEditKey) : base()
		{
			this.content = content;
			key = key ?? String.Empty;
			value = value ?? String.Empty;
			
			InitializeComponent();
			
			if (allowEditKey) {
				this.Get<TextBox>("key").Validating += this.KeyValidating;
			} else {
				this.Get<TextBox>("key").ReadOnly = true;
			}
			
			this.Get<TextBox>("key").Text = key;
			this.Get<TextBox>("value").Text = value;
			
			if (allowEditKey) {
				this.Get<TextBox>("key").Select();
				this.Get<TextBox>("key").Select(key.Length, 0);
			} else {
				this.Get<TextBox>("value").Select();
				this.Get<TextBox>("value").Select(value.Length, 0);
			}
			
		}
		
		void InitializeComponent()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Hornung.ResourceToolkit.Resources.EditStringResourceDialog.xfrm"));
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "ICSharpCode.Core.MessageService.ShowWarning(System.String)")]
		void KeyValidating(object sender, CancelEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (textBox.Text.Trim().Length == 0) {
				e.Cancel = true;
				MessageService.ShowWarning("${res:Hornung.ResourceToolkit.EditStringResourceDialog.KeyIsEmpty}");
			} else if (this.content.ContainsKey(textBox.Text.Trim())) {
				e.Cancel = true;
				MessageService.ShowWarning("${res:Hornung.ResourceToolkit.EditStringResourceDialog.DuplicateKey}");
			}
		}
		
		public string Key {
			get {
				return this.Get<TextBox>("key").Text.Trim();
			}
		}
		
		public string Value {
			get {
				return this.Get<TextBox>("value").Text;
			}
		}
		
	}
}

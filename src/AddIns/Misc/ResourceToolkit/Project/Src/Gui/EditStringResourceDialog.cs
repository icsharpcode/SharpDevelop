// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

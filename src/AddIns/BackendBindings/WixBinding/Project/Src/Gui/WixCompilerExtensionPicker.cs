// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Allows the user to add/remove Wix compiler, linker or library extensions.
	/// </summary>
	public class WixCompilerExtensionPicker : System.Windows.Forms.UserControl
	{
		public event EventHandler ExtensionsChanged;
		
		public WixCompilerExtensionPicker()
		{
			InitializeComponent();
			InitStrings();
		}
		
		/// <summary>
		/// Adds an extension to the list.
		/// </summary>
		public void AddExtension(string extension)
		{
			extensionsTextBox.AppendText(String.Concat(extension, "\r\n"));
		}
		
		/// <summary>
		/// Clears the extensions list.
		/// </summary>
		public void Clear()
		{
			extensionsTextBox.Text = String.Empty;
		}
		
		/// <summary>
		/// Adds a set of extensions to the picker.
		/// </summary>
		public void AddExtensions(WixCompilerExtensionName[] extensions)
		{
			foreach (WixCompilerExtensionName extension in extensions) {
				AddExtension(extension.QualifiedName);
			}
		}
		
		/// <summary>
		/// Gets the extensions in the list.
		/// </summary>
		public WixCompilerExtensionName[] GetExtensions()
		{
			List<WixCompilerExtensionName> extensions = new List<WixCompilerExtensionName>();
			foreach (string line in extensionsTextBox.Lines) {
				if (line.Length > 0) {
					extensions.Add(new WixCompilerExtensionName(line));
				}
			}
			return extensions.ToArray();
		}
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		#region Windows Forms Designer generated code
		
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		System.ComponentModel.IContainer components = null;

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		void InitializeComponent()
		{
			this.extensionsTextBox = new System.Windows.Forms.RichTextBox();
			this.addButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// extensionsTextBox
			// 
			this.extensionsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.extensionsTextBox.Location = new System.Drawing.Point(0, 0);
			this.extensionsTextBox.Name = "extensionsTextBox";
			this.extensionsTextBox.Size = new System.Drawing.Size(370, 109);
			this.extensionsTextBox.TabIndex = 0;
			this.extensionsTextBox.Text = "";
			this.extensionsTextBox.TextChanged += new System.EventHandler(this.ExtensionsTextBoxTextChanged);
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.Location = new System.Drawing.Point(301, 115);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(66, 24);
			this.addButton.TabIndex = 1;
			this.addButton.Text = "Add...";
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.AddButtonClick);
			// 
			// WixCompilerExtensionPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.extensionsTextBox);
			this.MinimumSize = new System.Drawing.Size(370, 134);
			this.Name = "WixCompilerExtensionPicker";
			this.Size = new System.Drawing.Size(370, 142);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.RichTextBox extensionsTextBox;
		
		#endregion

		void OnExtensionsChanged()
		{
			if (ExtensionsChanged != null) {
				ExtensionsChanged(this, new EventArgs());
			}
		}
		
		void AddButtonClick(object sender, EventArgs e)
		{
			using (AddCompilerExtensionsDialog dialog = new AddCompilerExtensionsDialog(GetExtensions(), GetStandardExtensions())) {
				if (DialogResult.OK == dialog.ShowDialog() && dialog.IsChanged) {
					Clear();
					AddExtensions(dialog.GetExtensions());
					OnExtensionsChanged();
				}
			}
		}
		
		/// <summary>
		/// Returns a list of the standard Wix Compiler extensions.
		/// These are defined in the WixBinding.addin file.
		/// </summary>
		WixCompilerExtensionName[] GetStandardExtensions()
		{
			List<WixCompilerExtensionName> extensionNames = new List<WixCompilerExtensionName>();
			List<string> extensions = AddInTree.BuildItems<string>("/AddIns/WixBinding/CompilerExtensions", this);
			
			foreach (string extension in extensions) {
				extensionNames.Add(WixCompilerExtensionName.CreateFromString(extension));
			}
			
			return extensionNames.ToArray();
		}
		
		void ExtensionsTextBoxTextChanged(object sender, EventArgs e)
		{
			OnExtensionsChanged();
		}
		
		void InitStrings()
		{
			addButton.Text = StringParser.Parse("${res:ICSharpCode.WixBinding.WixCompilerExtensionPicker.AddButton}");
		}
	}
}

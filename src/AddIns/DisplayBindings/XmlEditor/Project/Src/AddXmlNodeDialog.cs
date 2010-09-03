// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Base class for the AddElementDialog and AddAttributeDialog. This
	/// dialog presents a list of names and an extra text box for entering
	/// a custom name. It is used to add a new node to the XML tree. It
	/// contains all the core logic for the AddElementDialog and
	/// AddAttributeDialog classes.
	/// </summary>
	public class AddXmlNodeDialog : System.Windows.Forms.Form, IAddXmlNodeDialog
	{
		public AddXmlNodeDialog() : this(new string[0])
		{
		}
		
		/// <summary>
		/// Creates the dialog and adds the specified names to the
		/// list box.
		/// </summary>
		public AddXmlNodeDialog(string[] names)
		{
			InitializeComponent();
			InitStrings();
			if (names.Length > 0) {
				AddNames(names);
			} else {
				RemoveNamesListBox();
			}
			RightToLeftConverter.ConvertRecursive(this);
		}
		
		/// <summary>
		/// Gets the selected names in the list box together with the
		/// custom name entered in the text box.
		/// </summary>
		public string[] GetNames()
		{
			// Add items selected in list box.
			List<string> names = new List<string>();
			foreach (string name in namesListBox.SelectedItems) {
				names.Add(name);
			}
			
			// Add the custom name if entered.
			string customName = customNameTextBox.Text.Trim();
			if (customName.Length > 0) {
				names.Add(customName);
			}
			return names.ToArray();
		}
		
		/// <summary>
		/// Gets the text from the error provider.
		/// </summary>
		public string ErrorText {
			get { return errorProvider.GetError(customNameTextBox); }
		}
		
		/// <summary>
		/// Gets or sets the custom name label's text.
		/// </summary>
		public string CustomNameLabelText {
			get {
				return customNameTextBoxLabel.Text;
			}
			set {
				customNameTextBoxLabel.Text = value;
			}
		}
		
		/// <summary>
		/// Disposes resources used by the form.
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
		
		protected void NamesListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateOkButtonState();
		}
		
		protected void CustomNameTextBoxTextChanged(object sender, EventArgs e)
		{
			UpdateOkButtonState();
		}
		
		#region Windows Forms Designer generated code

		void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.namesListBox = new System.Windows.Forms.ListBox();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.customNameTextBoxLabel = new System.Windows.Forms.Label();
			this.customNameTextBox = new System.Windows.Forms.TextBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.bottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// namesListBox
			// 
			this.namesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                                  | System.Windows.Forms.AnchorStyles.Left)
			                                                                 | System.Windows.Forms.AnchorStyles.Right)));
			this.namesListBox.FormattingEnabled = true;
			this.namesListBox.Location = new System.Drawing.Point(0, 0);
			this.namesListBox.Name = "namesListBox";
			this.namesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.namesListBox.Size = new System.Drawing.Size(289, 173);
			this.namesListBox.Sorted = true;
			this.namesListBox.TabIndex = 1;
			this.namesListBox.SelectedIndexChanged += new System.EventHandler(this.NamesListBoxSelectedIndexChanged);
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// bottomPanel
			// 
			this.bottomPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			                                                                | System.Windows.Forms.AnchorStyles.Right)));
			this.bottomPanel.Controls.Add(this.customNameTextBoxLabel);
			this.bottomPanel.Controls.Add(this.customNameTextBox);
			this.bottomPanel.Controls.Add(this.cancelButton);
			this.bottomPanel.Controls.Add(this.okButton);
			this.bottomPanel.Location = new System.Drawing.Point(0, 173);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(289, 73);
			this.bottomPanel.TabIndex = 2;
			// 
			// customNameTextBoxLabel
			// 
			this.customNameTextBoxLabel.Location = new System.Drawing.Point(3, 10);
			this.customNameTextBoxLabel.Name = "customNameTextBoxLabel";
			this.customNameTextBoxLabel.Size = new System.Drawing.Size(82, 23);
			this.customNameTextBoxLabel.TabIndex = 3;
			this.customNameTextBoxLabel.Text = "Custom:";
			// 
			// customNameTextBox
			// 
			this.customNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                                      | System.Windows.Forms.AnchorStyles.Right)));
			this.customNameTextBox.Location = new System.Drawing.Point(107, 10);
			this.customNameTextBox.Name = "customNameTextBox";
			this.customNameTextBox.Size = new System.Drawing.Size(167, 20);
			this.customNameTextBox.TabIndex = 4;
			this.customNameTextBox.TextChanged += new System.EventHandler(this.CustomNameTextBoxTextChanged);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(199, 40);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Enabled = false;
			this.okButton.Location = new System.Drawing.Point(118, 40);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// AddXmlNodeDialog
			// 
			this.AcceptButton = this.okButton;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(289, 244);
			this.Controls.Add(this.bottomPanel);
			this.Controls.Add(this.namesListBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(289, 143);
			this.Name = "AddXmlNodeDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.bottomPanel.ResumeLayout(false);
			this.bottomPanel.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Panel bottomPanel;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.TextBox customNameTextBox;
		private System.Windows.Forms.Label customNameTextBoxLabel;
		private System.Windows.Forms.ListBox namesListBox;
		
		#endregion
		
		/// <summary>
		/// Adds the names to the list box.
		/// </summary>
		void AddNames(string[] names)
		{
			foreach (string name in names) {
				namesListBox.Items.Add(name);
			}
		}
		
		/// <summary>
		/// Enables or disables the ok button depending on whether any list
		/// item is selected or a custom name has been entered.
		/// </summary>
		void UpdateOkButtonState()
		{
			okButton.Enabled = IsOkButtonEnabled;
		}
		
		/// <summary>
		/// Returns whether any items are selected in the list box.
		/// </summary>
		bool IsItemSelected {
			get {
				return namesListBox.SelectedIndex >= 0;
			}
		}
		
		bool IsOkButtonEnabled {
			get {
				return IsItemSelected || ValidateCustomName();
			}
		}
		
		/// <summary>
		/// Returns whether there is a valid string in the custom
		/// name text box. The string must be a name that can be used to
		/// create an xml element or attribute.
		/// </summary>
		bool ValidateCustomName()
		{
			string name = customNameTextBox.Text.Trim();
			if (name.Length > 0) {
				try {
					VerifyName(name);
					errorProvider.Clear();
					return true;
				} catch (XmlException ex) {
					errorProvider.SetError(customNameTextBox, ex.Message);
				}
			}
			return false;
		}
		
		/// <summary>
		/// Checks that the name would make a valid element name or
		/// attribute name. Trying to use XmlConvert and its Verify methods
		/// so the validation is not done ourselves. XmlDocument has a
		/// CheckName method but this is not public.
		/// </summary>
		static void VerifyName(string name)
		{
			// Check the qualification is valid.
			string[] parts = name.Split(new char[] {':'}, 2);
			if (parts.Length == 1) {
				// No colons.
				XmlConvert.VerifyName(name);
				return;
			}
			
			string firstPart = parts[0].Trim();
			string secondPart = parts[1].Trim();
			if (firstPart.Length > 0 && secondPart.Length > 0) {
				XmlConvert.VerifyNCName(firstPart);
				XmlConvert.VerifyNCName(secondPart);
			} else {
				// Throw an error using VerifyNCName since the
				// qualified name parts have no strings.
				XmlConvert.VerifyNCName(name);
			}
		}
		
		/// <summary>
		/// Sets the control's text using string resources.
		/// </summary>
		void InitStrings()
		{
			okButton.Text = StringParser.Parse("${res:Global.OKButtonText}");
			cancelButton.Text = StringParser.Parse("${res:Global.CancelButtonText}");
		}
		
		/// <summary>
		/// Removes the names list box from the dialog, re-positions the
		/// remaining controls and resizes the dialog to fit.
		/// </summary>
		void RemoveNamesListBox()
		{
			using (namesListBox) {
				Controls.Remove(namesListBox);
				
				// Reset the dialog's minimum size first so setting the
				// ClientSize to something smaller works as expected.
				MinimumSize = Size.Empty;
				ClientSize = bottomPanel.Size;
				MinimumSize = Size;
				
				// Make sure bottom panel fills the dialog when it is resized.
				bottomPanel.Dock = DockStyle.Fill;
			}
		}
	}
}

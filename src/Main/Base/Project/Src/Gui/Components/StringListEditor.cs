// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Control to edit a list of strings.
	/// </summary>
	public class StringListEditor : System.Windows.Forms.UserControl
	{
		public StringListEditor()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.ManualOrder = true;
			this.BrowseForDirectory = false;
			// init enabled states:
			ListBoxSelectedIndexChanged(null, null);
			EditTextBoxTextChanged(null, null);
			addButton.Text = StringParser.Parse(addButton.Text);
			updateButton.Text = StringParser.Parse(updateButton.Text);
			removeButton.Text = StringParser.Parse(removeButton.Text);
			moveUpButton.Image   = WinFormsResourceService.GetBitmap("Icons.16x16.ArrowUp");
			moveDownButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.ArrowDown");
			deleteButton.Image   = WinFormsResourceService.GetBitmap("Icons.16x16.DeleteIcon");
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.Size = new System.Drawing.Size(380, 272);
			// 
			// StringListEditor
			// 
			removeButton = new System.Windows.Forms.Button();
			// 
			// removeButton
			// 
			removeButton.Location = new System.Drawing.Point(165, 53);
			removeButton.Name = "removeButton";
			removeButton.Size = new System.Drawing.Size(75, 23);
			removeButton.TabIndex = 5;
			removeButton.Text = "${res:Global.DeleteButtonText}";
			removeButton.Click += new System.EventHandler(this.RemoveButtonClick);
			deleteButton = new System.Windows.Forms.Button();
			// 
			// deleteButton
			// 
			deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			deleteButton.Location = new System.Drawing.Point(329, 164);
			deleteButton.Name = "deleteButton";
			deleteButton.Size = new System.Drawing.Size(24, 24);
			deleteButton.TabIndex = 10;
			deleteButton.Click += new System.EventHandler(this.RemoveButtonClick);
			moveDownButton = new System.Windows.Forms.Button();
			// 
			// moveDownButton
			// 
			moveDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			moveDownButton.Location = new System.Drawing.Point(329, 134);
			moveDownButton.Name = "moveDownButton";
			moveDownButton.Size = new System.Drawing.Size(24, 24);
			moveDownButton.TabIndex = 9;
			moveDownButton.Click += new System.EventHandler(this.MoveDownButtonClick);
			moveUpButton = new System.Windows.Forms.Button();
			// 
			// moveUpButton
			// 
			moveUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			moveUpButton.Location = new System.Drawing.Point(329, 104);
			moveUpButton.Name = "moveUpButton";
			moveUpButton.Size = new System.Drawing.Size(24, 24);
			moveUpButton.TabIndex = 8;
			moveUpButton.Click += new System.EventHandler(this.MoveUpButtonClick);
			listBox = new System.Windows.Forms.ListBox();
			// 
			// listBox
			// 
			listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                        | System.Windows.Forms.AnchorStyles.Left)
			                                                       | System.Windows.Forms.AnchorStyles.Right)));
			listBox.FormattingEnabled = true;
			listBox.Location = new System.Drawing.Point(3, 104);
			listBox.Name = "listBox";
			listBox.Size = new System.Drawing.Size(320, 160);
			listBox.TabIndex = 7;
			listBox.SelectedIndexChanged += new System.EventHandler(this.ListBoxSelectedIndexChanged);
			listLabel = new System.Windows.Forms.Label();
			// 
			// listLabel
			// 
			listLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                         | System.Windows.Forms.AnchorStyles.Right)));
			listLabel.Location = new System.Drawing.Point(3, 89);
			listLabel.Name = "listLabel";
			listLabel.Size = new System.Drawing.Size(350, 23);
			listLabel.TabIndex = 6;
			listLabel.Text = "List:";
			updateButton = new System.Windows.Forms.Button();
			// 
			// updateButton
			// 
			updateButton.Location = new System.Drawing.Point(84, 53);
			updateButton.Name = "updateButton";
			updateButton.Size = new System.Drawing.Size(75, 23);
			updateButton.TabIndex = 4;
			updateButton.Text = "${res:Global.UpdateButtonText}";
			updateButton.Click += new System.EventHandler(this.UpdateButtonClick);
			addButton = new System.Windows.Forms.Button();
			// 
			// addButton
			// 
			addButton.Location = new System.Drawing.Point(3, 53);
			addButton.Name = "addButton";
			addButton.Size = new System.Drawing.Size(75, 23);
			addButton.TabIndex = 3;
			addButton.Text = "${res:Global.AddButtonText}";
			addButton.Click += new System.EventHandler(this.AddButtonClick);
			editTextBox = new System.Windows.Forms.TextBox();
			// 
			// editTextBox
			// 
			editTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                           | System.Windows.Forms.AnchorStyles.Right)));
			editTextBox.Location = new System.Drawing.Point(3, 26);
			editTextBox.Name = "editTextBox";
			editTextBox.Size = new System.Drawing.Size(316, 21);
			editTextBox.TabIndex = 1;
			editTextBox.TextChanged += new System.EventHandler(this.EditTextBoxTextChanged);
			browseButton = new System.Windows.Forms.Button();
			// 
			// browseButton
			// 
			browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			browseButton.Location = new System.Drawing.Point(325, 24);
			browseButton.Name = "browseButton";
			browseButton.Size = new System.Drawing.Size(28, 23);
			browseButton.TabIndex = 2;
			browseButton.Text = "...";
			browseButton.Click += new System.EventHandler(this.BrowseButtonClick);
			TitleLabel = new System.Windows.Forms.Label();
			// 
			// TitleLabel
			// 
			TitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                          | System.Windows.Forms.AnchorStyles.Right)));
			TitleLabel.Location = new System.Drawing.Point(3, 10);
			TitleLabel.Name = "TitleLabel";
			TitleLabel.Size = new System.Drawing.Size(350, 23);
			TitleLabel.TabIndex = 0;
			TitleLabel.Text = "Title:";
			this.Controls.Add(removeButton);
			this.Controls.Add(deleteButton);
			this.Controls.Add(moveDownButton);
			this.Controls.Add(moveUpButton);
			this.Controls.Add(listBox);
			this.Controls.Add(listLabel);
			this.Controls.Add(updateButton);
			this.Controls.Add(addButton);
			this.Controls.Add(editTextBox);
			this.Controls.Add(browseButton);
			this.Controls.Add(TitleLabel);
			this.Name = "StringListEditor";
		}
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button moveDownButton;
		private System.Windows.Forms.Button moveUpButton;
		private System.Windows.Forms.ListBox listBox;
		private System.Windows.Forms.Label listLabel;
		private System.Windows.Forms.Button updateButton;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.TextBox editTextBox;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.Label TitleLabel;
		#endregion
		
		public event EventHandler ListChanged;
		
		protected virtual void OnListChanged(EventArgs e)
		{
			if (ListChanged != null) {
				ListChanged(this, e);
			}
		}
		
		public bool ManualOrder {
			get {
				return !listBox.Sorted;
			}
			set {
				moveUpButton.Visible = moveDownButton.Visible = deleteButton.Visible = value;
				removeButton.Visible = !value;
				listBox.Sorted = !value;
			}
		}
		
		bool browseForDirectory;
		
		public bool BrowseForDirectory {
			get {
				return browseForDirectory;
			}
			set {
				browseForDirectory = value;
				browseButton.Visible = browseForDirectory; // || browseForFile;
			}
		}
		
		bool autoAddAfterBrowse;
		
		public bool AutoAddAfterBrowse {
			get {
				return autoAddAfterBrowse;
			}
			set {
				autoAddAfterBrowse = value;
			}
		}
		
		public string TitleText {
			get {
				return TitleLabel.Text;
			}
			set {
				TitleLabel.Text = value;
			}
		}
		
		public string AddButtonText {
			get {
				return addButton.Text;
			}
			set {
				addButton.Text = value;
			}
		}
		
		public string ListCaption {
			get {
				return listLabel.Text;
			}
			set {
				listLabel.Text = value;
			}
		}
		
		public void LoadList(IEnumerable<string> list)
		{
			listBox.Items.Clear();
			foreach (string str in list) {
				listBox.Items.Add(str);
			}
		}
		
		public string[] GetList()
		{
			string[] list = new string[listBox.Items.Count];
			for (int i = 0; i < list.Length; i++) {
				list[i] = listBox.Items[i].ToString();
			}
			return list;
		}
		
		void BrowseButtonClick(object sender, EventArgs e)
		{
			using (FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog("${res:Dialog.ProjectOptions.SelectFolderTitle}")) {
				if (fdiag.ShowDialog() == DialogResult.OK) {
					string path = fdiag.SelectedPath;
					if (!path.EndsWith("\\") && !path.EndsWith("/"))
						path += "\\";
					editTextBox.Text = path;
					if (autoAddAfterBrowse) {
						AddButtonClick(null, null);
					}
				}
			}
		}
		
		void AddButtonClick(object sender, EventArgs e)
		{
			editTextBox.Text = editTextBox.Text.Trim();
			if (editTextBox.TextLength > 0) {
				int index = listBox.Items.IndexOf(editTextBox.Text);
				if (index < 0) {
					index = listBox.Items.Add(editTextBox.Text);
					OnListChanged(EventArgs.Empty);
				}
				listBox.SelectedIndex = index;
			}
		}
		
		void UpdateButtonClick(object sender, EventArgs e)
		{
			editTextBox.Text = editTextBox.Text.Trim();
			if (editTextBox.TextLength > 0) {
				listBox.Items[listBox.SelectedIndex] = editTextBox.Text;
				OnListChanged(EventArgs.Empty);
			}
		}
		
		void RemoveButtonClick(object sender, EventArgs e)
		{
			listBox.Items.RemoveAt(listBox.SelectedIndex);
			OnListChanged(EventArgs.Empty);
		}
		
		void MoveUpButtonClick(object sender, EventArgs e)
		{
			int index = listBox.SelectedIndex;
			object tmp = listBox.Items[index];
			listBox.Items[index] = listBox.Items[index - 1];
			listBox.Items[index - 1] = tmp;
			listBox.SelectedIndex = index - 1;
			OnListChanged(EventArgs.Empty);
		}
		
		void MoveDownButtonClick(object sender, EventArgs e)
		{
			int index = listBox.SelectedIndex;
			object tmp = listBox.Items[index];
			listBox.Items[index] = listBox.Items[index + 1];
			listBox.Items[index + 1] = tmp;
			listBox.SelectedIndex = index + 1;
			OnListChanged(EventArgs.Empty);
		}
		
		void ListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex >= 0) {
				editTextBox.Text = listBox.Items[listBox.SelectedIndex].ToString();
			}
			moveUpButton.Enabled = listBox.SelectedIndex > 0;
			moveDownButton.Enabled = listBox.SelectedIndex >= 0 && listBox.SelectedIndex < listBox.Items.Count - 1;
			removeButton.Enabled = deleteButton.Enabled = listBox.SelectedIndex >= 0;
			updateButton.Enabled = listBox.SelectedIndex >= 0 && editTextBox.TextLength > 0;
		}
		
		void EditTextBoxTextChanged(object sender, System.EventArgs e)
		{
			addButton.Enabled = editTextBox.TextLength > 0;
			updateButton.Enabled = listBox.SelectedIndex >= 0 && editTextBox.TextLength > 0;
		}
	}
}

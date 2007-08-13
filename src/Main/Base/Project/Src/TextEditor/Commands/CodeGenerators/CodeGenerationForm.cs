// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class CodeGenerationForm : Form
	{
		private System.Windows.Forms.ListView       categoryListView;
		private System.Windows.Forms.Label          statusLabel;
		private System.Windows.Forms.CheckedListBox selectionListBox;
		
		TextEditorControl textEditorControl;
		
		CodeGeneratorBase SelectedCodeGenerator {
			get {
				if (categoryListView.SelectedItems.Count <= 0) {
					return null;
				}
				return (CodeGeneratorBase)categoryListView.SelectedItems[0].Tag;
			}
		}
		
		public CodeGenerationForm(TextEditorControl textEditorControl, CodeGeneratorBase[] codeGenerators, IClass currentClass)
		{
			this.textEditorControl = textEditorControl;
			
			foreach (CodeGeneratorBase generator in codeGenerators) {
				generator.Initialize(currentClass);
			}
			
			//  Must be called for initialization
			this.InitializeComponents();
			
			okButton.Text = ResourceService.GetString("Global.OKButtonText");
			cancelButton.Text = ResourceService.GetString("Global.CancelButtonText");
			
//			selectionListBox.Sorted = true;
			TextLocation caretPos  = textEditorControl.ActiveTextAreaControl.Caret.Position;
			TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
			TextView textView = textArea.TextView;
			Point visualPos;
			int physicalline = textView.Document.GetVisibleLine(caretPos.Y);
			visualPos = new Point(textView.GetDrawingXPos(caretPos.Y, caretPos.X) +
			                      textView.DrawingPosition.X,
			                      (int)((1 + physicalline) * textView.FontHeight) -
			                      textArea.VirtualTop.Y - 1 + textView.DrawingPosition.Y);
			
			Point tempLocation = textEditorControl.ActiveTextAreaControl.TextArea.PointToScreen(visualPos);
			tempLocation.Y = (tempLocation.Y + Height) > Screen.FromPoint(tempLocation).WorkingArea.Bottom ?
				Screen.FromPoint(tempLocation).WorkingArea.Bottom - Height : tempLocation.Y;
			tempLocation.X = (tempLocation.X + Width) > Screen.FromPoint(tempLocation).WorkingArea.Right ?
				Screen.FromPoint(tempLocation).WorkingArea.Right - Width : tempLocation.X;
			Location = tempLocation;
			
			StartPosition   = FormStartPosition.Manual;
			
			
			categoryListView.SmallImageList = categoryListView.LargeImageList = ClassBrowserIconService.ImageList;
			
			foreach (CodeGeneratorBase codeGenerator in codeGenerators) {
				if (codeGenerator.IsActive) {
					ListViewItem newItem = new ListViewItem(StringParser.Parse(codeGenerator.CategoryName));
					newItem.ImageIndex = codeGenerator.ImageIndex;
					newItem.Tag        = codeGenerator;
					categoryListView.Items.Add(newItem);
				}
			}
			
			categoryListView.SelectedIndexChanged += new EventHandler(CategoryListViewItemChanged);
		}
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			if (categoryListView.Items.Count > 0) {
				categoryListView.Select();
				categoryListView.Focus();
				categoryListView.Items[0].Focused = categoryListView.Items[0].Selected = true;
			} else {
				Close();
			}
		}
		
		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.Escape:
					Close();
					return true;
				case Keys.Back:
					categoryListView.Focus();
					return true;
				case Keys.Return:
					if (SelectedCodeGenerator != null) {
						if (categoryListView.Focused && SelectedCodeGenerator.Content.Count > 0) {
							selectionListBox.Focus();
						} else {
							Close();
							SelectedCodeGenerator.GenerateCode(textEditorControl.ActiveTextAreaControl.TextArea, selectionListBox.CheckedItems.Count > 0 ? (IList)selectionListBox.CheckedItems : (IList)selectionListBox.SelectedItems);
						}
						return true;
					}  else {
						return false;
					}
			}
			return base.ProcessDialogKey(keyData);
		}
		
		void CategoryListViewItemChanged(object sender, EventArgs e)
		{
			CodeGeneratorBase codeGenerator = SelectedCodeGenerator;
			if (codeGenerator == null) {
				return;
			}
			
			statusLabel.Text = StringParser.Parse(codeGenerator.Hint);
			selectionListBox.BeginUpdate();
			selectionListBox.Items.Clear();
			if (codeGenerator.Content.Count > 0) {
				Hashtable objs = new Hashtable();
//				selectionListBox.Sorted = codeGenerator.Content.Count > 1;
				foreach (object o in codeGenerator.Content) {
					if (!objs.Contains(o.ToString())) {
						selectionListBox.Items.Add(o);
						objs.Add(o.ToString(), "");
					}
				}
				selectionListBox.SelectedIndex = 0;
			}
			selectionListBox.EndUpdate();
			selectionListBox.Refresh();
		}
		
		/// <summary>
		///   This method was autogenerated - do not change the contents manually
		/// </summary>
		private void InitializeComponents()
		{
			System.Windows.Forms.ColumnHeader columnHeader1;
			System.Windows.Forms.Panel panel1;
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.selectionListBox = new System.Windows.Forms.CheckedListBox();
			this.statusLabel = new System.Windows.Forms.Label();
			this.categoryListView = new System.Windows.Forms.ListView();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			panel1 = new System.Windows.Forms.Panel();
			panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// columnHeader1
			// 
			columnHeader1.Width = 258;
			// 
			// panel1
			// 
			panel1.BackColor = System.Drawing.SystemColors.Control;
			panel1.Controls.Add(this.okButton);
			panel1.Controls.Add(this.cancelButton);
			panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			panel1.Location = new System.Drawing.Point(1, 309);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(262, 29);
			panel1.TabIndex = 3;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(94, 3);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseCompatibleTextRendering = true;
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(175, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseCompatibleTextRendering = true;
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// selectionListBox
			// 
			this.selectionListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.selectionListBox.IntegralHeight = false;
			this.selectionListBox.Location = new System.Drawing.Point(1, 129);
			this.selectionListBox.Name = "selectionListBox";
			this.selectionListBox.Size = new System.Drawing.Size(262, 180);
			this.selectionListBox.TabIndex = 2;
			this.selectionListBox.UseCompatibleTextRendering = true;
			// 
			// statusLabel
			// 
			this.statusLabel.BackColor = System.Drawing.SystemColors.Control;
			this.statusLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.statusLabel.Location = new System.Drawing.Point(1, 113);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(262, 16);
			this.statusLabel.TabIndex = 1;
			this.statusLabel.Text = "statusLabel";
			this.statusLabel.UseCompatibleTextRendering = true;
			// 
			// categoryListView
			// 
			this.categoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									columnHeader1});
			this.categoryListView.Dock = System.Windows.Forms.DockStyle.Top;
			this.categoryListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.categoryListView.Location = new System.Drawing.Point(1, 1);
			this.categoryListView.MultiSelect = false;
			this.categoryListView.Name = "categoryListView";
			this.categoryListView.Size = new System.Drawing.Size(262, 112);
			this.categoryListView.TabIndex = 0;
			this.categoryListView.UseCompatibleStateImageBehavior = false;
			this.categoryListView.View = System.Windows.Forms.View.Details;
			// 
			// CodeGenerationForm
			// 
			this.AcceptButton = this.okButton;
			this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(264, 339);
			this.Controls.Add(this.selectionListBox);
			this.Controls.Add(this.statusLabel);
			this.Controls.Add(this.categoryListView);
			this.Controls.Add(panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "CodeGenerationForm";
			this.Padding = new System.Windows.Forms.Padding(1);
			this.ShowInTaskbar = false;
			panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		
		void CancelButtonClick(object sender, EventArgs e)
		{
			ProcessDialogKey(Keys.Escape);
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			ProcessDialogKey(Keys.Return);
		}
	}
}

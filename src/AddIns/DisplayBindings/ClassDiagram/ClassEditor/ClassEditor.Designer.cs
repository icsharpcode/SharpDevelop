/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 20/10/2006
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ClassDiagram
{
	partial class ClassEditor : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
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
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.membersList = new Aga.Controls.Tree.TreeViewAdv();
			this.nameCol = new Aga.Controls.Tree.TreeColumn();
			this.typeCol = new Aga.Controls.Tree.TreeColumn();
			this.modifierCol = new Aga.Controls.Tree.TreeColumn();
			this.summaryCol = new Aga.Controls.Tree.TreeColumn();
			this._icon = new Aga.Controls.Tree.NodeControls.NodeIcon();
			this._name = new Aga.Controls.Tree.NodeControls.NodeTextBox();
			this._type = new Aga.Controls.Tree.NodeControls.NodeComboBox();
			this._modifiers = new Aga.Controls.Tree.NodeControls.NodeComboBox();
			this._summary = new Aga.Controls.Tree.NodeControls.NodeTextBox();
			this._paramModifiers = new Aga.Controls.Tree.NodeControls.NodeComboBox();
			this.SuspendLayout();
			// 
			// membersList
			// 
			this.membersList.BackColor = System.Drawing.SystemColors.Window;
			this.membersList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.membersList.Columns.Add(this.nameCol);
			this.membersList.Columns.Add(this.typeCol);
			this.membersList.Columns.Add(this.modifierCol);
			this.membersList.Columns.Add(this.summaryCol);
			this.membersList.Cursor = System.Windows.Forms.Cursors.Default;
			this.membersList.DefaultToolTipProvider = null;
			this.membersList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.membersList.DragDropMarkColor = System.Drawing.Color.Black;
			this.membersList.FullRowSelect = true;
			this.membersList.LineColor = System.Drawing.SystemColors.ControlDark;
			this.membersList.Location = new System.Drawing.Point(0, 0);
			this.membersList.Model = null;
			this.membersList.Name = "membersList";
			this.membersList.NodeControls.Add(this._icon);
			this.membersList.NodeControls.Add(this._name);
			this.membersList.NodeControls.Add(this._type);
			this.membersList.NodeControls.Add(this._modifiers);
			this.membersList.NodeControls.Add(this._summary);
			this.membersList.NodeControls.Add(this._paramModifiers);
			this.membersList.Search.BackColor = System.Drawing.Color.Pink;
			this.membersList.Search.FontColor = System.Drawing.Color.Black;
			this.membersList.SelectedNode = null;
			this.membersList.ShowLines = false;
			this.membersList.Size = new System.Drawing.Size(603, 299);
			this.membersList.TabIndex = 0;
			this.membersList.Text = "treeViewAdv1";
			this.membersList.UseColumns = true;
			this.membersList.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.MembersListNodeMouseDoubleClick);
			// 
			// nameCol
			// 
			this.nameCol.Header = "Name";
			this.nameCol.SortOrder = System.Windows.Forms.SortOrder.None;
			this.nameCol.Width = 80;
			// 
			// typeCol
			// 
			this.typeCol.Header = "Type";
			this.typeCol.SortOrder = System.Windows.Forms.SortOrder.None;
			this.typeCol.Width = 80;
			// 
			// modifierCol
			// 
			this.modifierCol.Header = "Modifier";
			this.modifierCol.SortOrder = System.Windows.Forms.SortOrder.None;
			this.modifierCol.Width = 80;
			// 
			// summaryCol
			// 
			this.summaryCol.Header = "Summary";
			this.summaryCol.SortOrder = System.Windows.Forms.SortOrder.None;
			this.summaryCol.Width = 320;
			// 
			// _icon
			// 
			this._icon.DataPropertyName = "MemberIcon";
			this._icon.IncrementalSearchEnabled = false;
			this._icon.ParentColumn = this.nameCol;
			// 
			// _name
			// 
			this._name.DataPropertyName = "MemberName";
			this._name.EditEnabled = true;
			this._name.ParentColumn = this.nameCol;
			this._name.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
			this._name.DrawText += new System.EventHandler<Aga.Controls.Tree.NodeControls.DrawEventArgs>(this._nameDrawText);
			// 
			// _type
			// 
			this._type.DataPropertyName = "MemberType";
			this._type.EditEnabled = true;
			this._type.ParentColumn = this.typeCol;
			this._type.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
			// 
			// _modifiers
			// 
			this._modifiers.DataPropertyName = "MemberModifier";
			this._modifiers.DropDownItems.Add("Public");
			this._modifiers.DropDownItems.Add("Private");
			this._modifiers.DropDownItems.Add("Protected");
			this._modifiers.DropDownItems.Add("Internal");
			this._modifiers.EditEnabled = true;
			this._modifiers.ParentColumn = this.modifierCol;
			this._modifiers.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
			// 
			// _summary
			// 
			this._summary.DataPropertyName = "MemberSummary";
			this._summary.EditEnabled = true;
			this._summary.ParentColumn = this.summaryCol;
			this._summary.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
			// 
			// _paramModifiers
			// 
			this._paramModifiers.DataPropertyName = "MemberModifier";
			this._paramModifiers.DropDownItems.Add("In");
			this._paramModifiers.DropDownItems.Add("Out");
			this._paramModifiers.DropDownItems.Add("Ref");
			this._paramModifiers.DropDownItems.Add("Params");
			this._paramModifiers.DropDownItems.Add("Optional");
			this._paramModifiers.ParentColumn = this.modifierCol;
			// 
			// ClassEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.membersList);
			this.Name = "ClassEditor";
			this.Size = new System.Drawing.Size(603, 299);
			this.ResumeLayout(false);
		}
		private Aga.Controls.Tree.NodeControls.NodeComboBox _paramModifiers;
		private Aga.Controls.Tree.NodeControls.NodeIcon _icon;
		private Aga.Controls.Tree.NodeControls.NodeComboBox _modifiers;
		private Aga.Controls.Tree.NodeControls.NodeComboBox _type;
		private Aga.Controls.Tree.NodeControls.NodeTextBox _summary;
		private Aga.Controls.Tree.NodeControls.NodeTextBox _name;
		private Aga.Controls.Tree.TreeViewAdv membersList;
		private Aga.Controls.Tree.TreeColumn nameCol;
		private Aga.Controls.Tree.TreeColumn typeCol;
		private Aga.Controls.Tree.TreeColumn modifierCol;
		private Aga.Controls.Tree.TreeColumn summaryCol;
	}
}

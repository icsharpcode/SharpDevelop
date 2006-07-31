/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 7/31/2006
 * Time: 7:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace IconEditor
{
	partial class EditorPanel : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
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
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel2 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.colorComboBox = new System.Windows.Forms.ComboBox();
			this.table = new System.Windows.Forms.TableLayoutPanel();
			this.tableLabel = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2.SuspendLayout();
			this.table.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.colorComboBox);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(297, 30);
			this.panel2.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "View on color:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// colorComboBox
			// 
			this.colorComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.colorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.colorComboBox.FormattingEnabled = true;
			this.colorComboBox.Location = new System.Drawing.Point(108, 3);
			this.colorComboBox.Name = "colorComboBox";
			this.colorComboBox.Size = new System.Drawing.Size(46, 24);
			this.colorComboBox.TabIndex = 0;
			this.colorComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ColorComboBoxDrawItem);
			this.colorComboBox.SelectedIndexChanged += new System.EventHandler(this.ColorComboBoxSelectedIndexChanged);
			// 
			// table
			// 
			this.table.AutoScroll = true;
			this.table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.table.ColumnCount = 1;
			this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.table.Controls.Add(this.tableLabel, 0, 0);
			this.table.Dock = System.Windows.Forms.DockStyle.Fill;
			this.table.Location = new System.Drawing.Point(0, 30);
			this.table.Name = "table";
			this.table.RowCount = 1;
			this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.table.Size = new System.Drawing.Size(297, 200);
			this.table.TabIndex = 5;
			// 
			// tableLabel
			// 
			this.tableLabel.Location = new System.Drawing.Point(3, 0);
			this.tableLabel.Name = "tableLabel";
			this.tableLabel.Size = new System.Drawing.Size(68, 23);
			this.tableLabel.TabIndex = 0;
			this.tableLabel.Text = "Icon Editor";
			this.tableLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 230);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(297, 32);
			this.panel1.TabIndex = 6;
			this.panel1.Visible = false;
			// 
			// EditorPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.table);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.Name = "EditorPanel";
			this.Size = new System.Drawing.Size(297, 262);
			this.panel2.ResumeLayout(false);
			this.table.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label tableLabel;
		private System.Windows.Forms.TableLayoutPanel table;
		private System.Windows.Forms.ComboBox colorComboBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel2;
	}
}

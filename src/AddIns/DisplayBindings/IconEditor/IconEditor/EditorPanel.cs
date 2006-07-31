// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IconEditor
{
	/// <summary>
	/// Description of EditorPanel.
	/// </summary>
	public partial class EditorPanel
	{
		IList<Size> availableSizes;
		IList<int> availableColorDepths;
		IconPanel[,] iconPanels;
		
		public EditorPanel()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			ShowFile(new IconFile());
			
			for (int i = 0; i < backgroundColors.Length; i++) {
				colorComboBox.Items.Add("");
			}
			colorComboBox.SelectedIndex = 0;
		}
		
		IconFile activeIconFile;
		
		public void SaveIcon(string fileName)
		{
			activeIconFile.Save(fileName);
		}
		
		public void ShowFile(IconFile f)
		{
			this.activeIconFile = f;
			table.Visible = false;
			table.SuspendLayout();
			foreach (Control ctl in table.Controls) {
				if (ctl != this.tableLabel) {
					ctl.Dispose();
				}
			}
			table.Controls.Clear();
			table.ColumnCount = 1;
			table.RowCount = 1;
			table.Controls.Add(tableLabel, 0, 0);
			availableSizes = f.AvailableSizes;
			foreach (Size size in availableSizes) {
				table.RowCount += 1;
				table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				Label lbl = new Label();
				lbl.Text = size.Width + "x" + size.Height;
				lbl.AutoSize = true;
				lbl.Anchor = AnchorStyles.Right;
				table.Controls.Add(lbl, 0, table.RowCount - 1);
			}
			availableColorDepths = f.AvailableColorDepths;
			foreach (int colorDepth in availableColorDepths) {
				table.ColumnCount += 1;
				table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
				Label lbl = new Label();
				lbl.TextAlign = ContentAlignment.MiddleRight;
				lbl.Text = colorDepth + "bit";
				lbl.Anchor = AnchorStyles.Bottom;
				lbl.AutoSize = true;
				table.Controls.Add(lbl, table.ColumnCount - 1, 0);
			}
			table.ColumnCount += 1;
			table.RowCount += 1;
			iconPanels = new IconPanel[table.ColumnCount - 2, table.RowCount - 2];
			for (int column = 1; column < table.ColumnCount - 1; column++) {
				for (int row = 1; row < table.RowCount - 1; row++) {
					iconPanels[column-1,row-1] = new IconPanel(availableSizes[row-1], availableColorDepths[column-1]);
					iconPanels[column-1,row-1].Anchor = AnchorStyles.None;
					table.Controls.Add(iconPanels[column-1,row-1], column, row);
				}
			}
			foreach (IconEntry e in f.Icons) {
				int row = availableSizes.IndexOf(e.Size);
				int column = availableColorDepths.IndexOf(e.ColorDepth);
				iconPanels[column, row].Entry = e;
				iconPanels[column, row].BackColor = backgroundColors[colorComboBox.SelectedIndex];
			}
			// Work around Windows.Forms bug (scrollbars don't update correctly):
			table.Size = new Size(3000, 3000);
			table.ResumeLayout(true);
			table.Visible = true;
		}
		
		Color[] backgroundColors = {SystemColors.Control, SystemColors.Window, SystemColors.Desktop, SystemColors.ControlText};
		
		void ColorComboBoxDrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index >= 0) {
				using (SolidBrush b = new SolidBrush(backgroundColors[e.Index])) {
					e.Graphics.FillRectangle(b, e.Bounds);
				}
			}
		}
		
		void ColorComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < table.ColumnCount - 2; i++) {
				for (int j = 0; j < table.RowCount - 2; j++) {
					iconPanels[i, j].BackColor = backgroundColors[colorComboBox.SelectedIndex];
				}
			}
		}
	}
}

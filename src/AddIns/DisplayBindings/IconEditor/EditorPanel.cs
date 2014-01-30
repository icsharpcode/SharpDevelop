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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.IconEditor
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
		
		public void SaveIcon(Stream stream)
		{
			activeIconFile.Save(stream);
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
			availableSizes = f.AvailableSizes.ToList();
			foreach (Size size in availableSizes) {
				table.RowCount += 1;
				table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				Label lbl = new Label();
				lbl.Text = size.Width + "x" + size.Height;
				lbl.AutoSize = true;
				lbl.Anchor = AnchorStyles.Right;
				table.Controls.Add(lbl, 0, table.RowCount - 1);
			}
			availableColorDepths = f.AvailableColorDepths.ToList();
			foreach (int colorDepth in availableColorDepths) {
				table.ColumnCount += 1;
				table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
				Label lbl = new Label();
				lbl.TextAlign = ContentAlignment.MiddleRight;
				lbl.Text = colorDepth + " bit";
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
					iconPanels[column-1,row-1].BackColor = backgroundColors[colorComboBox.SelectedIndex];
					table.Controls.Add(iconPanels[column-1,row-1], column, row);
				}
			}
			foreach (IconEntry e in f.Icons) {
				int row = availableSizes.IndexOf(e.Size);
				int column = availableColorDepths.IndexOf(e.ColorDepth);
				iconPanels[column, row].Entry = e;
			}
			for (int column = 1; column < table.ColumnCount - 1; column++) {
				for (int row = 1; row < table.RowCount - 1; row++) {
					iconPanels[column-1, row-1].EntryChanged += EditorPanel_EntryChanged;
				}
			}
			// Work around Windows.Forms bug (scrollbars don't update correctly):
			table.Size = new Size(3000, 3000);
			table.ResumeLayout(true);
			table.Visible = true;
		}
		
		void EditorPanel_EntryChanged(object sender, EventArgs e)
		{
			var panel = (IconPanel)sender;
			if (panel.Entry == null) {
				activeIconFile.RemoveEntry(panel.IconSize.Width, panel.IconSize.Height, panel.ColorDepth);
				// recreate UI in case we removed the last icon of a format
				ShowFile(activeIconFile);
			} else {
				activeIconFile.AddEntry(panel.Entry);
			}
			if (IconWasEdited != null)
				IconWasEdited(this, e);
		}
		
		public event EventHandler IconWasEdited;
		
		Color[] backgroundColors = { SystemColors.Control, Color.Black, Color.White, Color.Teal, Color.DeepSkyBlue, Color.Red, Color.Magenta };
		
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
		
		void AddFormatButtonClick(object sender, EventArgs e)
		{
			if (activeIconFile == null)
				return;
			using (PickFormatDialog dlg = new PickFormatDialog()) {
				if (dlg.ShowDialog() == DialogResult.OK) {
					int width = dlg.IconWidth;
					int height = dlg.IconHeight;
					int colorDepth = dlg.ColorDepth;
					var sameSizeEntries = activeIconFile.Icons.Where(entry => entry.Width == width && entry.Height == height);
					if (sameSizeEntries.Any(entry => entry.ColorDepth == colorDepth)) {
						MessageService.ShowMessage("This icon already contains an image with the specified format.", "Icon Editor");
						return;
					}
					IconEntry sourceEntry = sameSizeEntries.OrderByDescending(entry => entry.ColorDepth).FirstOrDefault();
					if (sourceEntry == null) {
						sourceEntry = (from entry in activeIconFile.Icons
						               orderby entry.Width descending, entry.Height descending, entry.ColorDepth descending
						               select entry).FirstOrDefault();
					}
					// sourceEntry can still be null if the icon is completely empty
					Bitmap sourceBitmap = sourceEntry != null ? sourceEntry.ExportArgbBitmap() : new Bitmap(width, height);
					activeIconFile.AddEntry(new IconEntry(width, height, colorDepth, sourceBitmap));
					if (IconWasEdited != null)
						IconWasEdited(this, e);
					ShowFile(activeIconFile);
				}
			}
		}
	}
}

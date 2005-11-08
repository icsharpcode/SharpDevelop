// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui.TreeGrid;

namespace ICSharpCode.Core
{
	public class DebuggerGridControl : DynamicList
	{
		// Columns:
		// 0 = plus sign
		// 1 = icon
		// 2 = text
		// 3 = value
		
		DynamicTreeRow row;
		
		public static void AddColumns(IList<DynamicListColumn> columns)
		{
			columns.Add(new DynamicListColumn());
			columns.Add(new DynamicListColumn());
			columns.Add(new DynamicListColumn());
			columns.Add(new DynamicListColumn());
			columns[0].BackgroundBrush = Brushes.White;
			columns[0].BackgroundBrushInactive = Brushes.White;
			columns[0].RowHighlightBrush = null;
			
			// default is allowgrow = true and autosize = false
			columns[0].AllowGrow = false;
			columns[1].AllowGrow = false;
			columns[1].Width = 18;
			columns[1].ColumnSeperatorColor = Color.Transparent;
			columns[1].ColumnSeperatorColorInactive = Color.Transparent;
			columns[2].AutoSize = true;
			columns[2].MinimumWidth = 75;
			columns[2].ColumnSeperatorColor = Color.White;
			columns[2].ColumnSeperatorColorInactive = Color.FromArgb(172, 168, 153);
			columns[3].AutoSize = true;
			columns[3].MinimumWidth = 75;
		}
		
		public DebuggerGridControl(DynamicTreeRow row)
		{
			this.row = row;
			
			BeginUpdate();
			
			AddColumns(Columns);
			
			Rows.Add(row);
			
			AddColumns(row.ChildColumns);
			
			row.Expanded  += delegate { isExpanded = true; };
			row.Collapsed += delegate { isExpanded = false; };
			
			CreateControl();
			using (Graphics g = CreateGraphics()) {
				this.Width = GetRequiredWidth(g);
			}
			this.Height = row.Height;
			EndUpdate();
		}
		
		DynamicTreeRow.ChildForm frm;
		
		public void ShowForm(ICSharpCode.TextEditor.TextArea textArea, Point logicTextPos)
		{
			frm = new DynamicTreeRow.ChildForm();
			frm.AllowResizing = false;
			frm.Owner = textArea.FindForm();
			int ypos = (textArea.Document.GetVisibleLine(logicTextPos.Y) + 1) * textArea.TextView.FontHeight - textArea.VirtualTop.Y;
			Point p = new Point(0, ypos);
			p = textArea.PointToScreen(p);
			p.X = Control.MousePosition.X - 16;
			p.Y -= 1;
			frm.StartPosition = FormStartPosition.Manual;
			frm.ShowInTaskbar = false;
			frm.Location = p;
			frm.ClientSize = new Size(Width + 2, row.Height + 2);
			Dock = DockStyle.Fill;
			frm.Controls.Add(this);
			frm.ShowWindowWithoutActivation = true;
			frm.Show();
			textArea.Click   += OnTextAreaClick;
			textArea.KeyDown += OnTextAreaClick;
			frm.ClientSize = new Size(frm.ClientSize.Width, row.Height + 2);
		}
		
		void OnTextAreaClick(object sender, EventArgs e)
		{
			((ICSharpCode.TextEditor.TextArea)sender).KeyDown -= OnTextAreaClick;
			((ICSharpCode.TextEditor.TextArea)sender).Click   -= OnTextAreaClick;
			frm.Close();
		}
		
		bool isExpanded;
		
		public bool AllowClose {
			get {
				return !isExpanded;
			}
		}
	}
}

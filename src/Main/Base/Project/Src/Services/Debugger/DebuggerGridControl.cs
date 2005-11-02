/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 27.10.2005
 * Time: 14:18
 */

using System;
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
		
		public DebuggerGridControl(DynamicTreeRow row)
		{
			this.row = row;
			
			BeginUpdate();
			Columns.Add(new DynamicListColumn());
			Columns.Add(new DynamicListColumn());
			Columns.Add(new DynamicListColumn());
			Columns.Add(new DynamicListColumn());
			Columns[0].BackgroundBrush = SystemBrushes.ControlLightLight;
			// default is allowgrow = true and autosize = false
			Columns[0].AllowGrow = false;
			Columns[1].AllowGrow = false;
			Columns[1].Width = 18;
			Columns[2].AutoSize = true;
			Columns[3].AutoSize = true;
			Rows.Add(row);
			
			foreach (DynamicListColumn col in Columns) {
				row.ChildColumns.Add(col.Clone());
			}
			row.Expanded  += delegate { isExpanded = true; };
			row.Collapsed += delegate { isExpanded = false; };
			
			CreateControl();
			using (Graphics g = CreateGraphics()) {
				this.Width = GetRequiredWidth(g);
			}
			this.Height = row.Height;
			EndUpdate();
		}
		
		Form frm;
		
		public void ShowForm(ICSharpCode.TextEditor.TextArea textArea, Point logicTextPos)
		{
			frm = new DynamicTreeRow.ChildForm();
			frm.FormBorderStyle = FormBorderStyle.None;
			frm.Owner = textArea.FindForm();
			int ypos = (textArea.Document.GetVisibleLine(logicTextPos.Y) + 1) * textArea.TextView.FontHeight - textArea.VirtualTop.Y;
			Point p = new Point(0, ypos);
			p = textArea.PointToScreen(p);
			p.X = Control.MousePosition.X;
			frm.StartPosition = FormStartPosition.Manual;
			frm.ShowInTaskbar = false;
			frm.Location = p;
			frm.Size = new Size(Width, row.Height);
			Dock = DockStyle.Fill;
			frm.Controls.Add(this);
			ICSharpCode.TextEditor.Gui.CompletionWindow.AbstractCompletionWindow.ShowWindowWithoutFocus(frm);
			textArea.Click   += OnTextAreaClick;
			textArea.KeyDown += OnTextAreaClick;
			frm.Height = row.Height;
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

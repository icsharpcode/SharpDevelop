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

using ICSharpCode.SharpDevelop.Widgets.TreeGrid;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.Debugging
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
			
			row.Expanded  += delegate { isExpanded = true; };
			row.Collapsed += delegate { isExpanded = false; };
			
			CreateControl();
			using (Graphics g = CreateGraphics()) {
				this.Width = GetRequiredWidth(g);
			}
			this.Height = row.Height;
			EndUpdate();
		}
		
		class TooltipForm : DynamicTreeRow.ChildForm
		{
			protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
			{
				if (base.ProcessCmdKey(ref msg, keyData)) {
					return true;
				} else {
					Console.WriteLine("Handling " + keyData);
					var i = FindItemByShortcut(Gui.WorkbenchSingleton.MainForm.MainMenuStrip.Items, keyData);
					if (i != null)
						i.PerformClick();
					return false;
				}
			}
			
			static ToolStripMenuItem FindItemByShortcut(ToolStripItemCollection c, Keys shortcut)
			{
				foreach (ToolStripItem i in c) {
					ToolStripMenuItem mi = i as ToolStripMenuItem;
					if (mi != null) {
						if (mi.ShortcutKeys == shortcut && mi.Enabled)
							return mi;
						mi = FindItemByShortcut(mi.DropDownItems, shortcut);
						if (mi != null)
							return mi;
					}
				}
				return null;
			}
		}
		
		TooltipForm frm;
		
		public void ShowForm(ICSharpCode.TextEditor.TextArea textArea, TextLocation logicTextPos)
		{
			frm = new TooltipForm();
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
		
		public bool IsMouseOver {
			get {
				if (frm != null && !frm.IsDisposed) {
					return frm.ClientRectangle.Contains(frm.PointToClient(Control.MousePosition));
				}
				return false;
			}
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

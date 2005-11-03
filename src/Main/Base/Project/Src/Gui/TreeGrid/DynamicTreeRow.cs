// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.TreeGrid
{
	public class DynamicTreeRow : DynamicListRow
	{
		CollectionWithEvents<DynamicListColumn> childColumns = new CollectionWithEvents<DynamicListColumn>();
		CollectionWithEvents<DynamicListRow> childRows = new CollectionWithEvents<DynamicListRow>();
		
		DynamicListItem plus;
		
		public DynamicTreeRow()
		{
			plus = this[0];
			plus.HighlightBrush = Brushes.AliceBlue;
			plus.Cursor = Cursors.Hand;
			plus.Click += OnPlusClick;
			plus.Paint += OnPlusPaint;
			plus.Text = "+";
			plus.TextFormat = new StringFormat(plus.TextFormat);
			plus.TextFormat.Alignment = StringAlignment.Center;
			plus.TextFormat.LineAlignment = StringAlignment.Center;
		}
		
		protected virtual void OnPlusPaint(object sender, PaintEventArgs e)
		{
			Rectangle r = e.ClipRectangle;
			r.Inflate(-4, -4);
			e.Graphics.DrawRectangle(SystemPens.ControlDarkDark, r);
		}
		
		public class ChildForm : Form
		{
			bool isActive = true;
			
			protected override void OnActivated(EventArgs e)
			{
				base.OnActivated(e);
				isActive = true;
			}
			
			protected override void OnDeactivate(EventArgs e)
			{
				base.OnDeactivate(e);
				isActive = false;
				if (isOpeningChild)
					return;
				BeginInvoke(new MethodInvoker(CloseOnDeactivate));
			}
			
			void CloseOnDeactivate()
			{
				ChildForm owner = Owner as ChildForm;
				if (owner != null) {
					if (owner.isActive)
						Close();
					else
						owner.CloseOnDeactivate();
				} else {
					Close();
				}
			}
		}
		
		static bool isOpeningChild;
		
		protected virtual void OnPlusClick(object sender, DynamicListEventArgs e)
		{
			OnExpanding(e);
			ChildForm frm = new ChildForm();
			frm.Closed += delegate {
				frm = null;
				OnCollapsed(EventArgs.Empty);
			};
			frm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			Point p = e.List.PointToScreen(e.List.GetPositionFromRow(this));
			p.Offset(e.List.Columns[0].Width, Height);
			frm.StartPosition = FormStartPosition.Manual;
			frm.Location = p;
			frm.ShowInTaskbar = false;
			frm.Text = childWindowCaption;
			frm.Owner = e.List.FindForm();
			
			VerticalScrollContainer scrollContainer = new VerticalScrollContainer();
			scrollContainer.Dock = DockStyle.Fill;
			
			DynamicList childList = new DynamicList(childColumns, childRows);
			childList.Dock = DockStyle.Fill;
			childList.KeyDown += delegate(object sender2, KeyEventArgs e2) {
				if (e2.KeyData == Keys.Escape) {
					frm.Close();
					// workaround focus problem: sometimes the mainform gets focus after this
					e.List.FindForm().Focus();
				}
			};
			scrollContainer.Controls.Add(childList);
			
			frm.DockPadding.All = 2;
			frm.Controls.Add(scrollContainer);
			
			int screenHeight = Screen.FromPoint(p).WorkingArea.Bottom - p.Y;
			screenHeight -= frm.Size.Height - frm.ClientSize.Height;
			int requiredHeight = childList.TotalRowHeight + 4;
			int formHeight = Math.Min(requiredHeight, screenHeight);
			if (formHeight < requiredHeight) {
				int missingHeight = Math.Min(100, requiredHeight - formHeight);
				formHeight += missingHeight;
				frm.Top -= missingHeight;
			}
			// Autosize child window
			int formWidth;
			using (Graphics g = childList.CreateGraphics()) {
				formWidth = 8 + childList.GetRequiredWidth(g);
			}
			int screenWidth = Screen.FromPoint(p).WorkingArea.Right - p.X;
			if (formWidth > screenWidth) {
				int missingWidth = Math.Min(100, formWidth - screenWidth);
				formWidth = screenWidth + missingWidth;
				frm.Left -= missingWidth;
			}
			frm.ClientSize = new Size(formWidth, formHeight);
			isOpeningChild = true;
			frm.Show();
			isOpeningChild = false;
			childList.Focus();
			OnExpanded(e);
		}
		
		public event EventHandler Expanding;
		public event EventHandler Expanded;
		public event EventHandler Collapsed;
		
		protected virtual void OnExpanding(EventArgs e)
		{
			if (Expanding != null) {
				Expanding(this, e);
			}
		}
		protected virtual void OnExpanded(EventArgs e)
		{
			if (Expanded != null) {
				Expanded(this, e);
			}
		}
		protected virtual void OnCollapsed(EventArgs e)
		{
			if (Collapsed != null) {
				Collapsed(this, e);
			}
		}
		
		string childWindowCaption = "Child window";
		
		public string ChildWindowCaption {
			get {
				return childWindowCaption;
			}
			set {
				if (value == null)
					throw new ArgumentNullException();
				childWindowCaption = value;
			}
		}
		
		public CollectionWithEvents<DynamicListColumn> ChildColumns {
			get {
				return childColumns;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				childColumns = value;
			}
		}
		
		public CollectionWithEvents<DynamicListRow> ChildRows {
			get {
				return childRows;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				childRows = value;
			}
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
			ShowPlus = true;
		}
		
		bool showPlus;
		
		public bool ShowPlus {
			get {
				return showPlus;
			}
			set {
				if (showPlus == value)
					return;
				showPlus = value;
				plus.HighlightBrush = showPlus ? Brushes.AliceBlue : null;
				plus.Cursor = showPlus ? Cursors.Hand : null;
				if (showPlus) {
					plus.Click += OnPlusClick;
					plus.Paint += OnPlusPaint;
				} else {
					plus.Click -= OnPlusClick;
					plus.Paint -= OnPlusPaint;
				}
				OnShowPlusChanged(EventArgs.Empty);
			}
		}
		
		public event EventHandler ShowPlusChanged;
		
		public virtual void OnShowPlusChanged(EventArgs e)
		{
			if (ShowPlusChanged != null) {
				ShowPlusChanged(this, e);
			}
		}
		
		static readonly Color PlusBorder = Color.FromArgb(120, 152, 181);
		static readonly Color LightPlusBorder = Color.FromArgb(176, 194, 221);
		
		protected virtual void OnPlusPaint(object sender, PaintEventArgs e)
		{
			Rectangle r = e.ClipRectangle;
			r.Inflate(-4, -4);
			using (Brush b = new LinearGradientBrush(r, Color.White, Color.FromArgb(221, 218, 203), 66f)) {
				e.Graphics.FillRectangle(b, r);
			}
			using (Pen p = new Pen(PlusBorder)) {
				e.Graphics.DrawRectangle(p, r);
			}
			using (Brush b = new SolidBrush(LightPlusBorder)) {
				e.Graphics.FillRectangle(b, new Rectangle(r.X, r.Y, 1, 1));
				e.Graphics.FillRectangle(b, new Rectangle(r.Right, r.Y, 1, 1));
				e.Graphics.FillRectangle(b, new Rectangle(r.X, r.Bottom, 1, 1));
				e.Graphics.FillRectangle(b, new Rectangle(r.Right, r.Bottom, 1, 1));
			}
			e.Graphics.DrawLine(Pens.Black, r.Left + 2, r.Top + r.Height / 2, r.Right - 2, r.Top + r.Height / 2);
			e.Graphics.DrawLine(Pens.Black, r.Left + r.Width / 2, r.Top + 2, r.Left + r.Width / 2, r.Bottom - 2);
		}
		
		public class ChildForm : Form
		{
			bool isActive = true;
			
			bool allowResizing = true;
			
			public bool AllowResizing {
				get {
					return allowResizing;
				}
				set {
					if (allowResizing == value)
						return;
					allowResizing = value;
					this.DockPadding.All = value ? 2 : 1;
				}
			}
			
			public ChildForm()
			{
				this.FormBorderStyle = FormBorderStyle.None;
				this.DockPadding.All = 2;
				this.BackColor = Color.FromArgb(195, 192, 175);
			}
			
			#region Resizing the form
			private enum MousePositionCodes
			{
				HTERROR             = (-2),
				HTTRANSPARENT       = (-1),
				HTNOWHERE           = 0,
				HTCLIENT            = 1,
				HTCAPTION           = 2,
				HTSYSMENU           = 3,
				HTGROWBOX           = 4,
				HTSIZE              = HTGROWBOX,
				HTMENU              = 5,
				HTHSCROLL           = 6,
				HTVSCROLL           = 7,
				HTMINBUTTON         = 8,
				HTMAXBUTTON         = 9,
				HTLEFT              = 10,
				HTRIGHT             = 11,
				HTTOP               = 12,
				HTTOPLEFT           = 13,
				HTTOPRIGHT          = 14,
				HTBOTTOM            = 15,
				HTBOTTOMLEFT        = 16,
				HTBOTTOMRIGHT       = 17,
				HTBORDER            = 18,
				HTREDUCE            = HTMINBUTTON,
				HTZOOM              = HTMAXBUTTON,
				HTSIZEFIRST         = HTLEFT,
				HTSIZELAST          = HTBOTTOMRIGHT,
				HTOBJECT            = 19,
				HTCLOSE             = 20,
				HTHELP              = 21
			}
			
			protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
				if (m.Msg == 0x0084) // WM_NCHITTEST
					HitTest(ref m);
			}
			
			void HitTest(ref Message m)
			{
				if (!allowResizing)
					return;
				int mousePos = m.LParam.ToInt32();
				int mouseX = mousePos & 0xffff;
				int mouseY = mousePos >> 16;
				//System.Diagnostics.Debug.WriteLine(mouseX + " / " + mouseY);
				Rectangle bounds = Bounds;
				bool isLeft = mouseX == bounds.Left || mouseX + 1 == bounds.Left;
				bool isTop = mouseY == bounds.Top || mouseY + 1 == bounds.Top;
				bool isRight = mouseX == bounds.Right - 1 || mouseX == bounds.Right - 2;
				bool isBottom = mouseY == bounds.Bottom - 1 || mouseY == bounds.Bottom - 2;
				if (isLeft) {
					if (isTop)
						m.Result = new IntPtr((int)MousePositionCodes.HTTOPLEFT);
					else if (isBottom)
						m.Result = new IntPtr((int)MousePositionCodes.HTBOTTOMLEFT);
					else
						m.Result = new IntPtr((int)MousePositionCodes.HTLEFT);
				} else if (isRight) {
					if (isTop)
						m.Result = new IntPtr((int)MousePositionCodes.HTTOPRIGHT);
					else if (isBottom)
						m.Result = new IntPtr((int)MousePositionCodes.HTBOTTOMRIGHT);
					else
						m.Result = new IntPtr((int)MousePositionCodes.HTRIGHT);
				} else if (isTop) {
					m.Result = new IntPtr((int)MousePositionCodes.HTTOP);
				} else if (isBottom) {
					m.Result = new IntPtr((int)MousePositionCodes.HTBOTTOM);
				}
			}
			#endregion
			
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
			frm.MinimumSize = new Size(100, Math.Min(50, formHeight));
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

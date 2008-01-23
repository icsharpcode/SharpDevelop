// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.TreeGrid
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
		
		#region Plus painting
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
		
		static readonly Color PlusBorder = Color.FromArgb(120, 152, 181);
		static readonly Color LightPlusBorder = Color.FromArgb(176, 194, 221);
		public static readonly Color DefaultExpandedRowColor = Color.FromArgb(235, 229, 209);
		
		public static void DrawPlusSign(Graphics graphics, Rectangle r, bool drawMinus)
		{
			using (Brush b = new LinearGradientBrush(r, Color.White, DynamicListColumn.DefaultRowHighlightBackColor, 66f)) {
				graphics.FillRectangle(b, r);
			}
			using (Pen p = new Pen(PlusBorder)) {
				graphics.DrawRectangle(p, r);
			}
			using (Brush b = new SolidBrush(LightPlusBorder)) {
				graphics.FillRectangle(b, new Rectangle(r.X, r.Y, 1, 1));
				graphics.FillRectangle(b, new Rectangle(r.Right, r.Y, 1, 1));
				graphics.FillRectangle(b, new Rectangle(r.X, r.Bottom, 1, 1));
				graphics.FillRectangle(b, new Rectangle(r.Right, r.Bottom, 1, 1));
			}
			graphics.DrawLine(Pens.Black, r.Left + 2, r.Top + r.Height / 2, r.Right - 2, r.Top + r.Height / 2);
			if (!drawMinus) {
				graphics.DrawLine(Pens.Black, r.Left + r.Width / 2, r.Top + 2, r.Left + r.Width / 2, r.Bottom - 2);
			}
		}
		
		protected virtual void OnPlusPaint(object sender, ItemPaintEventArgs e)
		{
			Rectangle r = e.ClipRectangle;
			r.Inflate(-4, -4);
			DrawPlusSign(e.Graphics, r, expandedIn != null && expandedIn.Contains(e.List));
		}
		
		protected override DynamicListItem CreateItem()
		{
			DynamicListItem item = base.CreateItem();
			item.Paint += delegate(object sender, ItemPaintEventArgs e) {
				if (e.Item != plus && expandedIn != null && !expandedRowColor.IsEmpty && expandedIn.Contains(e.List)) {
					using (Brush b = new SolidBrush(expandedRowColor)) {
						e.Graphics.FillRectangle(b, e.FillRectangle);
					}
				}
			};
			return item;
		}
		
		List<DynamicList> expandedIn;
		Color expandedRowColor = DefaultExpandedRowColor;
		
		public bool ShowMinusWhileExpanded {
			get {
				return expandedIn != null;
			}
			set {
				if (this.ShowMinusWhileExpanded == value)
					return;
				expandedIn = value ? new List<DynamicList>() : null;
			}
		}
		
		/// <summary>
		/// Gets/Sets the row color used when the row is expanded. Only works together with ShowMinusWhileExpanded.
		/// </summary>
		public Color ExpandedRowColor {
			get {
				return expandedRowColor;
			}
			set {
				expandedRowColor = value;
			}
		}
		#endregion
		
		#region Events
		public event EventHandler<DynamicListEventArgs> Expanding;
		public event EventHandler<DynamicListEventArgs> Expanded;
		public event EventHandler<DynamicListEventArgs> Collapsed;
		public event EventHandler ShowPlusChanged;
		
		protected virtual void OnExpanding(DynamicListEventArgs e)
		{
			if (Expanding != null) {
				Expanding(this, e);
			}
		}
		protected virtual void OnExpanded(DynamicListEventArgs e)
		{
			if (Expanded != null) {
				Expanded(this, e);
			}
		}
		protected virtual void OnCollapsed(DynamicListEventArgs e)
		{
			if (Collapsed != null) {
				Collapsed(this, e);
			}
		}
		public virtual void OnShowPlusChanged(EventArgs e)
		{
			if (ShowPlusChanged != null) {
				ShowPlusChanged(this, e);
			}
		}
		#endregion
		
		#region Properties
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
		
		public static readonly Color DefaultBorderColor = Color.FromArgb(195, 192, 175);
		
		Color childBorderColor = DefaultBorderColor;
		
		public Color ChildBorderColor {
			get {
				return childBorderColor;
			}
			set {
				childBorderColor = value;
			}
		}
		#endregion
		
		#region Child form
		static bool isOpeningChild;
		
		/// <summary>
		/// Block the next click event - used to ensure that a click on the "-" sign
		/// does not cause the submenu to open again when the "-" sign becomes a "+" sign
		/// before the click event is handled.
		/// </summary>
		bool blockClickEvent;
		
		protected virtual void OnPlusClick(object sender, DynamicListEventArgs e)
		{
			if (blockClickEvent) { blockClickEvent = false; return; }
			OnExpanding(e);
			// If OnExpanding displaies an error message, focus is lost, form is closed and we can not access the handle anymore
			if (e.List.IsDisposed) return;
			ChildForm frm = new ChildForm();
			frm.Closed += delegate {
				blockClickEvent = true;
				if (expandedIn != null)
					expandedIn.Remove(e.List);
				OnCollapsed(e);
				plus.RaiseItemChanged();
				Timer timer = new Timer();
				timer.Interval = 85;
				timer.Tick += delegate(object sender2, EventArgs e2) {
					((Timer)sender2).Stop();
					((Timer)sender2).Dispose();
					blockClickEvent = false;
				};
				timer.Start();
			};
			Point p = e.List.PointToScreen(e.List.GetPositionFromRow(this));
			p.Offset(e.List.Columns[0].Width, Height);
			frm.StartPosition = FormStartPosition.Manual;
			frm.BackColor = childBorderColor;
			frm.Location = p;
			frm.ShowInTaskbar = false;
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
			if (expandedIn != null)
				expandedIn.Add(e.List);
			OnExpanded(e);
			plus.RaiseItemChanged();
		}
		
		public class ChildForm : Form, IActivatable
		{
			bool isActivated = true;
			
			public bool IsActivated {
				get {
					return isActivated;
				}
			}
			
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
				this.BackColor = DefaultBorderColor;
			}
			
			bool showWindowWithoutActivation;
			
			/// <summary>
			/// Gets/Sets whether the window will receive focus when it is shown.
			/// </summary>
			public bool ShowWindowWithoutActivation {
				get {
					return showWindowWithoutActivation;
				}
				set {
					showWindowWithoutActivation = value;
				}
			}
			
			protected override bool ShowWithoutActivation {
				get {
					return showWindowWithoutActivation;
				}
			}
			
			protected override CreateParams CreateParams {
				get {
					CreateParams p = base.CreateParams;
					DesignHelper.AddShadowToWindow(p);
					return p;
				}
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
				isActivated = true;
				base.OnActivated(e);
				Refresh();
			}
			
			protected override void OnDeactivate(EventArgs e)
			{
				isActivated = false;
				base.OnDeactivate(e);
				if (isOpeningChild) {
					Refresh();
					return;
				}
				BeginInvoke(new MethodInvoker(CloseOnDeactivate));
			}
			
			void CloseOnDeactivate()
			{
				ChildForm owner = Owner as ChildForm;
				if (owner != null) {
					if (owner.isActivated)
						Close();
					else
						owner.CloseOnDeactivate();
				} else {
					Close();
				}
			}
			
			protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
			{
				if (base.ProcessCmdKey(ref msg, keyData)) {
					return true;
				} else {
					ChildForm owner = Owner as ChildForm;
					if (owner != null)
						return owner.ProcessCmdKey(ref msg, keyData);
					return false;
				}
			}
		}
		#endregion
	}
}

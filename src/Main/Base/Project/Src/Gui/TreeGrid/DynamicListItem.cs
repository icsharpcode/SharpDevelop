// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.TreeGrid
{
	public sealed class DynamicListItem
	{
		DynamicListRow row;
		
		internal DynamicListItem(DynamicListRow row)
		{
			this.row = row;
		}
		
		void OnItemChanged()
		{
			row.RaiseItemChanged(this);
		}
		
		Cursor cursor;
		
		public Cursor Cursor {
			get {
				return cursor;
			}
			set {
				cursor = value;
			}
		}
		
		#region BackgroundBrush / Control
		Brush backgroundBrush;
		
		public Brush BackgroundBrush {
			get {
				return backgroundBrush;
			}
			set {
				if (backgroundBrush != value) {
					backgroundBrush = value;
					OnItemChanged();
				}
			}
		}
		
		Brush highlightBrush;
		
		public Brush HighlightBrush {
			get {
				return highlightBrush;
			}
			set {
				if (highlightBrush != value) {
					highlightBrush = value;
					OnItemChanged();
				}
			}
		}
		
		Control control;
		
		public Control Control {
			get {
				return control;
			}
			set {
				if (control != value) {
					control = value;
					OnItemChanged();
				}
			}
		}
		#endregion
		
		#region MeasureWidth / Paint
		public event EventHandler<MeasureWidthEventArgs> MeasureWidth;
		
		internal void MeasureMinimumWidth(Graphics graphics, ref int minimumWidth)
		{
			if (MeasureWidth != null) {
				MeasureWidthEventArgs e = new MeasureWidthEventArgs(graphics);
				MeasureWidth(this, e);
				minimumWidth = Math.Max(minimumWidth, e.ItemWidth);
			}
			if (text.Length > 0) {
				int width = 2 + (int)graphics.MeasureString(text, font, new PointF(0, 0), textFormat).Width;
				minimumWidth = Math.Max(minimumWidth, width);
			}
		}
		
		public event PaintEventHandler Paint;
		
		internal void PaintTo(Graphics g, Rectangle rectangle, DynamicListColumn column, bool isMouseEntered)
		{
			if (highlightBrush != null && isMouseEntered)
				g.FillRectangle(highlightBrush, rectangle);
			else
				g.FillRectangle(backgroundBrush ?? column.BackgroundBrush, rectangle);
			if (Paint != null) {
				Paint(this, new PaintEventArgs(g, rectangle));
			}
			if (text.Length > 0) {
				g.DrawString(text, font, textBrush, rectangle, textFormat);
			}
		}
		#endregion
		
		#region Text drawing
		string text = string.Empty;
		
		public string Text {
			get {
				return text;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value", "Use string.Empty instead of null!");
				if (text != value) {
					text = value;
					OnItemChanged();
				}
			}
		}
		
		Font font = Control.DefaultFont;
		
		public Font Font {
			get {
				return font;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				if (font != value) {
					font = value;
					OnItemChanged();
				}
			}
		}
		
		Brush textBrush = SystemBrushes.ControlText;
		
		public Brush TextBrush {
			get {
				return textBrush;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				if (textBrush != value) {
					textBrush = value;
					OnItemChanged();
				}
			}
		}
		
		static StringFormat defaultTextFormat;
		
		public static StringFormat DefaultTextFormat {
			get {
				if (defaultTextFormat == null) {
					defaultTextFormat = (StringFormat)StringFormat.GenericDefault.Clone();
					defaultTextFormat.FormatFlags |= StringFormatFlags.NoWrap;
				}
				return defaultTextFormat;
			}
		}
		
		StringFormat textFormat = DefaultTextFormat;
		
		public StringFormat TextFormat {
			get {
				return textFormat;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				if (textFormat != value) {
					textFormat = value;
					OnItemChanged();
				}
			}
		}
		#endregion
		
		#region Mouse Events
		public event EventHandler<DynamicListEventArgs> Click;
		
		public void PerformClick(DynamicList list)
		{
			if (Click != null)
				Click(this, new DynamicListEventArgs(list));
			HandleLabelEditClick(list);
		}
		
		public event EventHandler<DynamicListEventArgs> DoubleClick;
		
		public void PerformDoubleClick(DynamicList list)
		{
			if (DoubleClick != null)
				DoubleClick(this, new DynamicListEventArgs(list));
		}
		
		public event EventHandler<DynamicListEventArgs> MouseHover;
		
		internal void OnMouseHover(DynamicList list)
		{
			if (MouseHover != null) {
				MouseHover(this, new DynamicListEventArgs(list));
			}
		}
		
		public event EventHandler<DynamicListEventArgs> MouseLeave;
		
		internal void OnMouseLeave(DynamicList list)
		{
			if (MouseLeave != null) {
				MouseLeave(this, new DynamicListEventArgs(list));
			}
			if (highlightBrush != null) OnItemChanged();
		}
		
		public event EventHandler<DynamicListEventArgs> MouseEnter;
		
		internal void OnMouseEnter(DynamicList list)
		{
			if (MouseEnter != null) {
				MouseEnter(this, new DynamicListEventArgs(list));
			}
			if (highlightBrush != null) OnItemChanged();
		}
		
		public event EventHandler<DynamicListMouseEventArgs> MouseMove;
		
		internal void OnMouseMove(DynamicListMouseEventArgs e)
		{
			if (MouseMove != null) {
				MouseMove(this, e);
			}
		}
		
		public event EventHandler<DynamicListMouseEventArgs> MouseDown;
		
		internal void OnMouseDown(DynamicListMouseEventArgs e)
		{
			if (MouseDown != null) {
				MouseDown(this, e);
			}
		}
		
		public event EventHandler<DynamicListMouseEventArgs> MouseUp;
		
		internal void OnMouseUp(DynamicListMouseEventArgs e)
		{
			if (MouseUp != null) {
				MouseUp(this, e);
			}
		}
		#endregion
		
		#region Label editing
		bool allowLabelEdit;
		
		public bool AllowLabelEdit {
			get {
				return allowLabelEdit;
			}
			set {
				allowLabelEdit = value;
			}
		}
		
		public event EventHandler<DynamicListEventArgs> BeginLabelEdit;
		public event EventHandler<DynamicListEventArgs> FinishLabelEdit;
		public event EventHandler<DynamicListEventArgs> CancelledLabelEdit;
		
		void HandleLabelEditClick(DynamicList list)
		{
			if (!allowLabelEdit)
				return;
			if (BeginLabelEdit != null)
				BeginLabelEdit(this, new DynamicListEventArgs(list));
			TextBox txt = new TextBox();
			txt.Text = this.Text;
			AssignControlUntilFocusChange(txt);
			bool escape = false;
			txt.KeyDown += delegate(object sender2, KeyEventArgs e2) {
				if (e2.KeyData == Keys.Enter || e2.KeyData == Keys.Escape) {
					e2.Handled = true;
					if (e2.KeyData == Keys.Escape) {
						if (CancelledLabelEdit != null)
							CancelledLabelEdit(this, new DynamicListEventArgs(list));
						escape = true;
					}
					this.Control = null;
					txt.Dispose();
				}
			};
			txt.LostFocus += delegate {
				if (!escape) {
					this.Text = txt.Text;
					if (FinishLabelEdit != null)
						FinishLabelEdit(this, new DynamicListEventArgs(list));
				}
			};
		}
		
		/// <summary>
		/// Display the control for this item. Automatically assign focus to the control
		/// and removes+disposes the control when it looses focus.
		/// </summary>
		public void AssignControlUntilFocusChange(Control control)
		{
			MethodInvoker method = delegate {
				if (!control.Focus()) {
					control.Focus();
				}
				control.LostFocus += delegate {
					this.Control = null;
					control.Dispose();
				};
			};
			
			control.HandleCreated += delegate {
				control.BeginInvoke(method);
			};
			this.Control = control;
		}
		#endregion
	}
	
	public class DynamicListEventArgs : EventArgs
	{
		DynamicList list;
		
		public DynamicList List {
			[DebuggerStepThrough]
			get {
				return list;
			}
		}
		
		public DynamicListEventArgs(DynamicList list)
		{
			if (list == null) throw new ArgumentNullException("list");
			this.list = list;
		}
	}
	
	public class DynamicListMouseEventArgs : MouseEventArgs
	{
		DynamicList list;
		
		public DynamicList List {
			[DebuggerStepThrough]
			get {
				return list;
			}
		}
		
		public DynamicListMouseEventArgs(DynamicList list, MouseEventArgs me)
			: base(me.Button, me.Clicks, me.X, me.Y, me.Delta)
		{
			if (list == null) throw new ArgumentNullException("list");
			this.list = list;
		}
	}
	
	public class MeasureWidthEventArgs : EventArgs
	{
		Graphics graphics;
		
		public Graphics Graphics {
			get {
				return graphics;
			}
		}
		
		int itemWidth;
		
		public int ItemWidth {
			get {
				return itemWidth;
			}
			set {
				itemWidth = value;
			}
		}
		
		public MeasureWidthEventArgs(Graphics graphics)
		{
			if (graphics == null)
				throw new ArgumentNullException("graphics");
			this.graphics = graphics;
		}
	}
}

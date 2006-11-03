// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.TreeGrid
{
	public sealed class DynamicListItem
	{
		DynamicListRow row;
		
		internal DynamicListItem(DynamicListRow row)
		{
			this.row = row;
		}
		
		public void RaiseItemChanged()
		{
			row.RaiseItemChanged(this);
		}
		
		Cursor cursor;
		
		public DynamicListRow Row {
			get {
				return row;
			}
		}
		
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
					RaiseItemChanged();
				}
			}
		}
		
		Brush backgroundBrushInactive;
		
		public Brush BackgroundBrushInactive {
			get {
				return backgroundBrushInactive;
			}
			set {
				if (backgroundBrushInactive != value) {
					backgroundBrushInactive = value;
					RaiseItemChanged();
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
					RaiseItemChanged();
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
					RaiseItemChanged();
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
				// Prevent GDI exception (forum-12284) when text is very long
				if (text.Length > short.MaxValue) {
					text = text.Substring(0, short.MaxValue - 1);
				}
				int width = 2 + (int)graphics.MeasureString(text, font, new PointF(0, 0), textFormat).Width;
				minimumWidth = Math.Max(minimumWidth, width);
			}
		}
		
		public event EventHandler<ItemPaintEventArgs> Paint;
		
		internal void PaintTo(Graphics g, Rectangle rectangle, DynamicList list, DynamicListColumn column, bool isMouseEntered)
		{
			Rectangle fillRectangle = rectangle;
			fillRectangle.Width += 1;
			if (highlightBrush != null && isMouseEntered) {
				g.FillRectangle(highlightBrush, fillRectangle);
			} else {
				bool isActivated = list.IsActivated;
				Brush bgBrush = GetBrush(isActivated, backgroundBrush, backgroundBrushInactive);
				if (bgBrush == null) {
					bgBrush = GetBrush(isActivated, column.BackgroundBrush, column.BackgroundBrushInactive);
					if (isActivated && list.RowAtMousePosition == row && column.RowHighlightBrush != null)
						bgBrush = column.RowHighlightBrush;
				}
				g.FillRectangle(bgBrush, fillRectangle);
			}
			if (Paint != null) {
				Paint(this, new ItemPaintEventArgs(g, rectangle, fillRectangle, list, column, this, isMouseEntered));
			}
			if (text.Length > 0) {
				g.DrawString(text, font, textBrush, rectangle, textFormat);
			}
		}
		
		Brush GetBrush(bool isActive, Brush activeBrush, Brush inactiveBrush)
		{
			return isActive ? (activeBrush ?? inactiveBrush) : (inactiveBrush ?? activeBrush);
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
					RaiseItemChanged();
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
					RaiseItemChanged();
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
					RaiseItemChanged();
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
					RaiseItemChanged();
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
			if (highlightBrush != null) RaiseItemChanged();
		}
		
		public event EventHandler<DynamicListEventArgs> MouseEnter;
		
		internal void OnMouseEnter(DynamicList list)
		{
			if (MouseEnter != null) {
				MouseEnter(this, new DynamicListEventArgs(list));
			}
			if (highlightBrush != null) RaiseItemChanged();
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
		public event EventHandler<DynamicListEventArgs> CanceledLabelEdit;
		
		void HandleLabelEditClick(DynamicList list)
		{
			if (!allowLabelEdit)
				return;
			TextBox txt = new TextBox();
			txt.Text = this.Text;
			AssignControlUntilFocusChange(txt);
			if (BeginLabelEdit != null)
				BeginLabelEdit(this, new DynamicListEventArgs(list));
			bool escape = false;
			txt.KeyDown += delegate(object sender2, KeyEventArgs e2) {
				if (e2.KeyData == Keys.Enter || e2.KeyData == Keys.Escape) {
					e2.Handled = true;
					if (e2.KeyData == Keys.Escape) {
						if (CanceledLabelEdit != null)
							CanceledLabelEdit(this, new DynamicListEventArgs(list));
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
	
	public class ItemPaintEventArgs : PaintEventArgs
	{
		DynamicList list;
		DynamicListColumn column;
		DynamicListItem item;
		bool isMouseEntered;
		Rectangle fillRectangle;
		
		public Rectangle FillRectangle {
			get {
				return fillRectangle;
			}
		}
		
		public DynamicList List {
			get {
				return list;
			}
		}
		
		public DynamicListColumn Column {
			get {
				return column;
			}
		}
		
		public DynamicListRow Row {
			get {
				return item.Row;
			}
		}
		
		public DynamicListItem Item {
			get {
				return item;
			}
		}
		
		public bool IsMouseEntered {
			get {
				return isMouseEntered;
			}
		}
		
		public ItemPaintEventArgs(Graphics graphics, Rectangle rectangle, Rectangle fillRectangle,
		                          DynamicList list, DynamicListColumn column,
		                          DynamicListItem item, bool isMouseEntered)
			: base(graphics, rectangle)
		{
			this.fillRectangle = fillRectangle;
			this.list = list;
			this.column = column;
			this.item = item;
			this.isMouseEntered = isMouseEntered;
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.TreeGrid
{
	public class DynamicList : UserControl, VerticalScrollContainer.IScrollable
	{
		public const int MaxColumnCount = 1000;
		
		public DynamicList()
			: this(new CollectionWithEvents<DynamicListColumn>(), new CollectionWithEvents<DynamicListRow>())
		{
		}
		
		public DynamicList(CollectionWithEvents<DynamicListColumn> columns, CollectionWithEvents<DynamicListRow> rows)
		{
			inUpdate = true;
			if (columns == null)
				throw new ArgumentNullException("columns");
			if (rows == null)
				throw new ArgumentNullException("rows");
			this.columns = columns;
			this.rows = rows;
			// we have to register our events on the existing items
			foreach (DynamicListColumn column in columns) {
				OnColumnAdded(null, new CollectionItemEventArgs<DynamicListColumn>(column));
			}
			foreach (DynamicListRow row in rows) {
				OnRowAdded(null, new CollectionItemEventArgs<DynamicListRow>(row));
			}
			columns.Added   += OnColumnAdded;
			columns.Removed += OnColumnRemoved;
			rows.Added      += OnRowAdded;
			rows.Removed    += OnRowRemoved;
			this.BackColor = DefaultBackColor;
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			inUpdate = false;
			RecalculateColumnWidths();
		}
		
		public new static readonly Color DefaultBackColor = Color.White;
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) {
				columns.Added   -= OnColumnAdded;
				columns.Removed -= OnColumnRemoved;
				rows.Added      -= OnRowAdded;
				rows.Removed    -= OnRowRemoved;
				foreach (DynamicListColumn column in columns) {
					OnColumnRemoved(null, new CollectionItemEventArgs<DynamicListColumn>(column));
				}
				foreach (DynamicListRow row in rows) {
					OnRowRemoved(null, new CollectionItemEventArgs<DynamicListRow>(row));
				}
			}
		}
		
		void OnColumnAdded(object sender, CollectionItemEventArgs<DynamicListColumn> e)
		{
			e.Item.MinimumWidthChanged += ColumnMinimumWidthChanged;
			e.Item.WidthChanged += ColumnWidthChanged;
			RecalculateColumnWidths();
		}
		
		void OnColumnRemoved(object sender, CollectionItemEventArgs<DynamicListColumn> e)
		{
			e.Item.MinimumWidthChanged -= ColumnMinimumWidthChanged;
			e.Item.WidthChanged -= ColumnWidthChanged;
			RecalculateColumnWidths();
		}
		
		void OnRowAdded(object sender, CollectionItemEventArgs<DynamicListRow> e)
		{
			e.Item.HeightChanged += RowHeightChanged;
			e.Item.ItemChanged   += RowItemChanged;
			if (Visible && Parent != null)
				e.Item.NotifyListVisibilityChange(this, true);
		}
		
		void OnRowRemoved(object sender, CollectionItemEventArgs<DynamicListRow> e)
		{
			e.Item.HeightChanged -= RowHeightChanged;
			e.Item.ItemChanged   -= RowItemChanged;
			if (Visible)
				e.Item.NotifyListVisibilityChange(this, false);
		}
		
		bool oldVisible = false;
		
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			bool visible = Visible && Parent != null;
			if (visible == oldVisible)
				return;
			oldVisible = visible;
			foreach (DynamicListRow row in Rows) {
				row.NotifyListVisibilityChange(this, visible);
			}
		}
		
		void ColumnMinimumWidthChanged(object sender, EventArgs e)
		{
			RecalculateColumnWidths();
		}
		
		void RowHeightChanged(object sender, EventArgs e)
		{
			Redraw();
		}
		
		void RowItemChanged(object sender, EventArgs e)
		{
			Redraw();
		}
		
		bool inRecalculateColumnWidths;
		bool inRecalculateNeedsRedraw;
		
		void RecalculateColumnWidths()
		{
			if (inUpdate) return;
			if (inRecalculateColumnWidths) return;
			inRecalculateColumnWidths = true;
			inRecalculateNeedsRedraw = false;
			try {
				int availableWidth = ClientSize.Width;
				int minRequiredWidth = 0;
				foreach (DynamicListColumn c in columns) {
					if (c.AllowGrow)
						minRequiredWidth += c.MinimumWidth;
					else
						availableWidth -= c.Width;
					availableWidth -= 1;
				}
				// everyone gets c.MinimumWidth * availableWidth / minRequiredWidth
				foreach (DynamicListColumn c in columns) {
					if (c.AllowGrow)
						c.Width = Math.Max(2, c.MinimumWidth * availableWidth / minRequiredWidth);
				}
			} finally {
				inRecalculateColumnWidths = false;
			}
			if (inRecalculateNeedsRedraw) {
				Redraw();
			}
		}
		
		void ColumnWidthChanged(object sender, EventArgs e)
		{
			if (inRecalculateColumnWidths) {
				inRecalculateNeedsRedraw = true;
				return;
			}
			Redraw();
		}
		
		bool inUpdate;
		
		public void BeginUpdate()
		{
			inUpdate = true;
		}
		
		public void EndUpdate()
		{
			inUpdate = false;
			RecalculateColumnWidths();
		}
		
		void Redraw()
		{
			if (inUpdate) return;
			Invalidate();
		}
		
		List<Control> allowedControls = new List<Control>();
		List<Control> removedControls = new List<Control>();
		int scrollOffset = 0;
		
		public int ScrollOffset {
			get {
				return scrollOffset;
			}
			set {
				if (scrollOffset != value) {
					scrollOffset = value;
					Redraw();
				}
			}
		}
		
		int VerticalScrollContainer.IScrollable.ScrollOffsetY {
			get {
				return this.ScrollOffset;
			}
			set {
				this.ScrollOffset = value;
			}
		}
		
		int VerticalScrollContainer.IScrollable.ScrollHeightY {
			get {
				return this.TotalRowHeight;
			}
		}
		
		public int GetRequiredWidth(Graphics graphics)
		{
			int width = 0;
			for (int i = 0; i < columns.Count; i++) {
				if (columns[i].AutoSize) {
					int minimumWidth = DynamicListColumn.DefaultWidth;
					foreach (DynamicListRow row in Rows) {
						DynamicListItem item = row[i];
						item.MeasureMinimumWidth(graphics, ref minimumWidth);
					}
					width += minimumWidth;
				} else {
					width += columns[i].Width;
				}
				width += 1;
			}
			return width;
		}
		
		int lineMarginY = 0;
		
		public int LineMarginY {
			get {
				return lineMarginY;
			}
			set {
				if (lineMarginY == value)
					return;
				lineMarginY = value;
				Redraw();
			}
		}
		
		bool inOnPaint;
		
		protected override void OnPaint(PaintEventArgs e)
		{
			if (inOnPaint) {
				Debug.WriteLine("Prevent nested OnPaint call");
				base.OnPaint(e);
				return;
			}
			inOnPaint = true;
			try {
				DoPaint(e);
				base.OnPaint(e);
			} finally {
				inOnPaint = false;
			}
		}
		
		void DoPaint(PaintEventArgs e)
		{
			Debug.WriteLine("DynamicList.DoPaint");
			Graphics g = e.Graphics;
			allowedControls.Clear();
			
			int columnIndex = -1;
			foreach (DynamicListColumn col in Columns) {
				columnIndex += 1;
				if (!col.AutoSize)
					continue;
				int minimumWidth = DynamicListColumn.DefaultWidth;
				foreach (DynamicListRow row in Rows) {
					DynamicListItem item = row[columnIndex];
					item.MeasureMinimumWidth(e.Graphics, ref minimumWidth);
				}
				col.MinimumWidth = minimumWidth;
			}
			
			int controlIndex = 0;
			int xPos;
			int yPos = -scrollOffset;
			Size clientSize = ClientSize;
			foreach (DynamicListRow row in Rows) {
				if (yPos + row.Height > 0 && yPos < clientSize.Height) {
					xPos = 0;
					for (columnIndex = 0; columnIndex < columns.Count; columnIndex++) {
						DynamicListColumn col = columns[columnIndex];
						Rectangle rect = new Rectangle(xPos, yPos, col.Width, row.Height);
						if (columnIndex == columns.Count - 1)
							rect.Width = clientSize.Width - 1 - rect.Left;
						DynamicListItem item = row[columnIndex];
						Control ctl = item.Control;
						if (ctl != null) {
							allowedControls.Add(ctl);
							if (rect != ctl.Bounds)
								ctl.Bounds = rect;
							if (!this.Controls.Contains(ctl)) {
								this.Controls.Add(ctl);
								this.Controls.SetChildIndex(ctl, controlIndex);
							}
							controlIndex += 1;
						} else {
							item.PaintTo(e.Graphics, rect, this, col, item == itemAtMousePosition);
						}
						xPos += col.Width + 1;
					}
				}
				yPos += row.Height + lineMarginY;
			}
			xPos = 0;
			Form containerForm = FindForm();
			bool isFocused;
			if (containerForm is IActivatable)
				isFocused = (containerForm as IActivatable).IsActivated;
			else
				isFocused = this.Focused;
			for (columnIndex = 0; columnIndex < columns.Count - 1; columnIndex++) {
				DynamicListColumn col = columns[columnIndex];
				
				xPos += col.Width + 1;
				
				Color separatorColor;
				if (isFocused) {
					separatorColor = col.ColumnSeperatorColor;
					if (separatorColor.IsEmpty)
						separatorColor = col.ColumnSeperatorColorInactive;
				} else {
					separatorColor = col.ColumnSeperatorColorInactive;
					if (separatorColor.IsEmpty)
						separatorColor = col.ColumnSeperatorColor;
				}
				if (separatorColor.IsEmpty) separatorColor = BackColor;
				using (Pen separatorPen = new Pen(separatorColor)) {
					e.Graphics.DrawLine(separatorPen, xPos - 1, 1, xPos - 1, Math.Min(clientSize.Height, yPos) - 2);
				}
			}
			removedControls.Clear();
			foreach (Control ctl in Controls) {
				if (!allowedControls.Contains(ctl))
					removedControls.Add(ctl);
			}
			foreach (Control ctl in removedControls) {
				Debug.WriteLine("DynamicList Removing control");
				Controls.Remove(ctl);
				Debug.WriteLine("DynamicList Control removed");
			}
			allowedControls.Clear();
			removedControls.Clear();
		}
		
		/// <summary>
		/// Gets if the parent form of this list is the active window.
		/// </summary>
		public bool IsActivated {
			get {
				Form containerForm = FindForm();
				if (containerForm is IActivatable)
					return (containerForm as IActivatable).IsActivated;
				else
					return this.Focused;
			}
		}
		
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			RecalculateColumnWidths();
		}
		
		public DynamicListRow GetRowFromPoint(int yPos)
		{
			int y = -scrollOffset;
			foreach (DynamicListRow row in Rows) {
				if (yPos < y)
					break;
				if (yPos <= y + row.Height)
					return row;
				y += row.Height + lineMarginY;
			}
			return null;
		}
		
		/// <summary>
		/// Gets the upper left corner of the specified row.
		/// </summary>
		public Point GetPositionFromRow(DynamicListRow row)
		{
			int y = -scrollOffset;
			foreach (DynamicListRow r in Rows) {
				if (r == row)
					return new Point(0, y);
				y += r.Height + lineMarginY;
			}
			throw new ArgumentException("The row in not in this list!");
		}
		
		/// <summary>
		/// Gets the height of all rows.
		/// </summary>
		public int TotalRowHeight {
			get {
				int y = 0;
				foreach (DynamicListRow r in Rows) {
					y += r.Height + lineMarginY;
				}
				return y;
			}
		}
		
		public int GetColumnIndexFromPoint(int xPos)
		{
			int columnIndex = 0;
			int x = 0;
			foreach (DynamicListColumn col in Columns) {
				if (xPos < x)
					break;
				if (xPos <= x + col.Width)
					return columnIndex;
				x += col.Width + 1;
				columnIndex += 1;
			}
			return -1;
		}
		
		public DynamicListItem GetItemFromPoint(Point position)
		{
			DynamicListRow row = GetRowFromPoint(position.Y);
			if (row == null)
				return null;
			int columnIndex = GetColumnIndexFromPoint(position.X);
			if (columnIndex < 0)
				return null;
			return row[columnIndex];
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			DynamicListItem item = GetItemFromPoint(PointToClient(Control.MousePosition));
			if (item != null) item.PerformClick(this);
		}
		
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			DynamicListItem item = GetItemFromPoint(PointToClient(Control.MousePosition));
			if (item != null) item.PerformDoubleClick(this);
		}
		
		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);
			DynamicListItem item = GetItemFromPoint(PointToClient(Control.MousePosition));
			if (item != null) item.OnMouseHover(this);
		}
		
		DynamicListItem itemAtMousePosition;
		DynamicListRow rowAtMousePosition;
		
		public DynamicListItem ItemAtMousePosition {
			get {
				return itemAtMousePosition;
			}
		}
		
		public DynamicListRow RowAtMousePosition {
			get {
				return rowAtMousePosition;
			}
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			DynamicListRow row = GetRowFromPoint(e.Y);
			if (rowAtMousePosition != row) {
				rowAtMousePosition = row;
				Invalidate();
			}
			if (row == null)
				return;
			int columnIndex = GetColumnIndexFromPoint(e.X);
			if (columnIndex < 0)
				return;
			DynamicListItem item = row[columnIndex];
			if (itemAtMousePosition != item) {
				if (itemAtMousePosition != null) {
					OnLeaveItem(itemAtMousePosition);
				}
				ResetMouseEventArgs(); // raise hover again
				itemAtMousePosition = item;
				if (item != null) {
					if (item.Cursor != null)
						this.Cursor = item.Cursor;
					item.OnMouseEnter(this);
				}
			}
			if (item != null) {
				item.OnMouseMove(new DynamicListMouseEventArgs(this, e));
			}
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			DynamicListItem item = GetItemFromPoint(e.Location);
			if (item != null) item.OnMouseDown(new DynamicListMouseEventArgs(this, e));
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			DynamicListItem item = GetItemFromPoint(e.Location);
			if (item != null) item.OnMouseUp(new DynamicListMouseEventArgs(this, e));
		}
		
		protected override void OnMouseLeave(EventArgs e)
		{
			rowAtMousePosition = null;
			if (itemAtMousePosition != null) {
				OnLeaveItem(itemAtMousePosition);
				itemAtMousePosition = null;
			}
			base.OnMouseLeave(e);
		}
		
		protected virtual void OnLeaveItem(DynamicListItem item)
		{
			itemAtMousePosition.OnMouseLeave(this);
			this.Cursor = Cursors.Default;
		}
		
		readonly CollectionWithEvents<DynamicListColumn> columns;
		readonly CollectionWithEvents<DynamicListRow>    rows;
		
		public CollectionWithEvents<DynamicListColumn> Columns {
			[DebuggerStepThrough]
			get {
				return columns;
			}
		}
		
		[Browsable(false)]
		public CollectionWithEvents<DynamicListRow> Rows {
			[DebuggerStepThrough]
			get {
				return rows;
			}
		}
	}
	
	public interface IActivatable
	{
		bool IsActivated { get; }
		event EventHandler Activated;
		event EventHandler Deactivate;
	}
}

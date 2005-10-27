// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.TreeGrid
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
		
		public new static readonly Color DefaultBackColor = SystemColors.ControlLightLight;
		
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
		}
		
		void OnRowRemoved(object sender, CollectionItemEventArgs<DynamicListRow> e)
		{
			e.Item.HeightChanged -= RowHeightChanged;
			e.Item.ItemChanged   -= RowItemChanged;
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
		
		protected override void OnPaint(PaintEventArgs e)
		{
			//Debug.WriteLine("OnPaint");
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
			int yPos = -scrollOffset;
			int clientHeight = ClientSize.Height;
			foreach (DynamicListRow row in Rows) {
				if (yPos + row.Height > 0 && yPos < clientHeight) {
					columnIndex = 0;
					int xPos = 0;
					foreach (DynamicListColumn col in Columns) {
						Rectangle rect = new Rectangle(xPos, yPos, col.Width, row.Height);
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
							item.PaintTo(e.Graphics, rect, col, item == itemAtMousePosition);
						}
						xPos += col.Width + 1;
						columnIndex += 1;
					}
				}
				yPos += row.Height + 1;
			}
			removedControls.Clear();
			foreach (Control ctl in Controls) {
				if (!allowedControls.Contains(ctl))
					removedControls.Add(ctl);
			}
			foreach (Control ctl in removedControls) {
				Debug.WriteLine("Removing control");
				Controls.Remove(ctl);
				Debug.WriteLine("Control removed");
			}
			allowedControls.Clear();
			removedControls.Clear();
			base.OnPaint(e);
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
				y += row.Height + 1;
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
				y += r.Height + 1;
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
					y += r.Height + 1;
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
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			DynamicListItem item = GetItemFromPoint(e.Location);
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
}

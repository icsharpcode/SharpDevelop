using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Aga.Controls.Tree.NodeControls;
using System.Drawing.Imaging;

namespace Aga.Controls.Tree
{
	public partial class TreeViewAdv
	{
		private void SetDropPosition(Point pt)
		{
			TreeNodeAdv node = GetNodeAt(pt);
			_dropPosition.Node = node;
			if (node != null)
			{
				int rowHeight = _rowLayout.GetRowBounds(node.Row).Height;
				float pos = (pt.Y - ColumnHeaderHeight - ((node.Row - FirstVisibleRow) * rowHeight)) / (float)rowHeight;
				if (pos < TopEdgeSensivity)
					_dropPosition.Position = NodePosition.Before;
				else if (pos > (1 - BottomEdgeSensivity))
					_dropPosition.Position = NodePosition.After;
				else
					_dropPosition.Position = NodePosition.Inside;
			}
		}

		#region Keys

		protected override bool IsInputKey(Keys keyData)
		{
			if (((keyData & Keys.Up) == Keys.Up)
				|| ((keyData & Keys.Down) == Keys.Down)
				|| ((keyData & Keys.Left) == Keys.Left)
				|| ((keyData & Keys.Right) == Keys.Right))
				return true;
			else
				return base.IsInputKey(keyData);
		}

		internal void ChangeInput()
		{
			if ((ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				if (!(Input is InputWithShift))
					Input = new InputWithShift(this);
			}
			else if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (!(Input is InputWithControl))
					Input = new InputWithControl(this);
			}
			else
			{
				if (!(Input.GetType() == typeof(NormalInputState)))
					Input = new NormalInputState(this);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled)
			{
				if (Search.IsActive)
					Search.KeyDown(e);

				if (!e.Handled)
				{
					if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
						ChangeInput();
					Input.KeyDown(e);
					if (!e.Handled)
					{
						foreach (NodeControlInfo item in GetNodeControls(CurrentNode))
						{
							item.Control.KeyDown(e);
							if (e.Handled)
								break;
						}
					}
				}
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (!e.Handled)
			{
				if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
					ChangeInput();
				if (!e.Handled)
				{
					foreach (NodeControlInfo item in GetNodeControls(CurrentNode))
					{
						item.Control.KeyUp(e);
						if (e.Handled)
							return;
					}
				}
			}
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if (!e.Handled)
				Search.Search(e.KeyChar);
		}

		#endregion

		#region Mouse

		private TreeNodeAdvMouseEventArgs CreateMouseArgs(MouseEventArgs e)
		{
			TreeNodeAdvMouseEventArgs args = new TreeNodeAdvMouseEventArgs(e);
			args.ViewLocation = new Point(e.X + OffsetX,
				e.Y + _rowLayout.GetRowBounds(FirstVisibleRow).Y - ColumnHeaderHeight);
			args.ModifierKeys = ModifierKeys;
			args.Node = GetNodeAt(e.Location);
			NodeControlInfo info = GetNodeControlInfoAt(args.Node, e.Location);
			args.ControlBounds = info.Bounds;
			args.Control = info.Control;
			return args;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			Search.EndSearch();
			if (SystemInformation.MouseWheelScrollLines > 0)
			{
				int lines = e.Delta / 120 * SystemInformation.MouseWheelScrollLines;
				int newValue = _vScrollBar.Value - lines;
				_vScrollBar.Value = Math.Max(_vScrollBar.Minimum,
					Math.Min(_vScrollBar.Maximum - _vScrollBar.LargeChange + 1, newValue));
			}
			base.OnMouseWheel(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!Focused)
				Focus();

			Search.EndSearch();
			if (e.Button == MouseButtons.Left)
			{
				TreeColumn c;
				c = GetColumnAt(e.Location, true);
				if (c != null)
				{
					Input = new ResizeColumnState(this, c, e.Location);
					return;
				}
				c = GetColumnAt(e.Location, false);
				if (c != null)
				{
					Input = new ClickColumnState(this, c, e.Location);
					UpdateView();
					return;
				}
			}

			ChangeInput();
			TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);

			if (args.Node != null && args.Control != null)
				args.Control.MouseDown(args);

			if (!args.Handled)
				Input.MouseDown(args);

			base.OnMouseDown(e);
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);

			if (args.Node != null && args.Control != null)
				args.Control.MouseDoubleClick(args);

			if (!args.Handled)
			{
				if (args.Node != null && args.Button == MouseButtons.Left)
					args.Node.IsExpanded = !args.Node.IsExpanded;
			}

			args.Handled = false;
			if (args.Node != null)
				OnNodeMouseDoubleClick(args);

			base.OnMouseDoubleClick(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);
			if (Input is ResizeColumnState)
				Input.MouseUp(args);
			else
			{
				if (args.Node != null && args.Control != null)
					args.Control.MouseUp(args);
				if (!args.Handled)
					Input.MouseUp(args);

				base.OnMouseUp(e);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Input.MouseMove(e))
				return;

			base.OnMouseMove(e);
			SetCursor(e);
			if (e.Location.Y <= ColumnHeaderHeight)
			{
				_toolTip.Active = false;
			}
			else
			{
				UpdateToolTip(e);
				if (ItemDragMode && Dist(e.Location, ItemDragStart) > ItemDragSensivity
					&& CurrentNode != null && CurrentNode.IsSelected)
				{
					ItemDragMode = false;
					_toolTip.Active = false;
					OnItemDrag(e.Button, Selection.ToArray());
				}
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_hotColumn = null;
			UpdateHeaders();
			base.OnMouseLeave(e);
		}

		private void SetCursor(MouseEventArgs e)
		{
			if (GetColumnAt(e.Location, true) == null)
				_innerCursor = null;
			else
				_innerCursor = Cursors.VSplit;

			TreeColumn col = GetColumnAt(e.Location, false);
			if (col != _hotColumn)
			{
				_hotColumn = col;
				UpdateHeaders();
			}
		}

		internal TreeColumn GetColumnAt(Point p, bool divider)
		{
			if (p.Y > ColumnHeaderHeight)
				return null;

			int x = -OffsetX;
			foreach (TreeColumn c in Columns)
			{
				if (c.IsVisible)
				{
					Rectangle rect;
					if (divider)
						rect = new Rectangle(x + c.Width - (DividerWidth / 2), 0, DividerWidth, ColumnHeaderHeight);
					else
						rect = new Rectangle(x, 0, c.Width, ColumnHeaderHeight);
					x += c.Width;
					if (rect.Contains(p))
						return c;
				}
			}
			return null;
		}

		TreeNodeAdv _hoverNode;
		NodeControl _hoverControl;
		private void UpdateToolTip(MouseEventArgs e)
		{
			if (ShowNodeToolTips)
			{
				TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);
				if (args.Node != null)
				{
					if (args.Node != _hoverNode || args.Control != _hoverControl)
					{
						string msg = GetNodeToolTip(args);
						if (!String.IsNullOrEmpty(msg))
						{
							_toolTip.SetToolTip(this, msg);
							_toolTip.Active = true;
						}
						else
							_toolTip.SetToolTip(this, null);
					}
				}
				else
					_toolTip.SetToolTip(this, null);

				_hoverControl = args.Control;
				_hoverNode = args.Node;
			}
			else
				_toolTip.SetToolTip(this, null);
		}

		private string GetNodeToolTip(TreeNodeAdvMouseEventArgs args)
		{
			string msg = args.Control.GetToolTip(args.Node);

			BaseTextControl btc = args.Control as BaseTextControl;
			if (btc != null && btc.DisplayHiddenContentInToolTip && String.IsNullOrEmpty(msg))
			{
				Size ms = btc.MeasureSize(args.Node, _measureContext);
				if (ms.Width > args.ControlBounds.Size.Width || ms.Height > args.ControlBounds.Size.Height
					|| args.ControlBounds.Right - OffsetX > DisplayRectangle.Width)
					msg = btc.GetLabel(args.Node);
			}

			if (String.IsNullOrEmpty(msg) && DefaultToolTipProvider != null)
				msg = DefaultToolTipProvider.GetToolTip(args.Node, args.Control);

			return msg;
		}

		#endregion

		#region DragDrop

		private bool _dragAutoScrollFlag = false;
		private Bitmap _dragBitmap = null;

		private void DragTimerTick(object sender, EventArgs e)
		{
			_dragAutoScrollFlag = true;
		}

		private void DragAutoScroll()
		{
			_dragAutoScrollFlag = false;
			Point pt = PointToClient(MousePosition);
			if (pt.Y < 20 && _vScrollBar.Value > 0)
				_vScrollBar.Value--;
			else if (pt.Y > Height - 20 && _vScrollBar.Value <= _vScrollBar.Maximum - _vScrollBar.LargeChange)
				_vScrollBar.Value++;
		}

		public void DoDragDropSelectedNodes(DragDropEffects allowedEffects)
		{
			if (SelectedNodes.Count > 0)
			{
				TreeNodeAdv[] nodes = new TreeNodeAdv[SelectedNodes.Count];
				SelectedNodes.CopyTo(nodes, 0);
				DoDragDrop(nodes, allowedEffects);
			}
		}

		private void CreateDragBitmap(IDataObject data)
		{
			if (UseColumns || !DisplayDraggingNodes)
				return;
			
			TreeNodeAdv[] nodes = data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
			if (nodes != null && nodes.Length > 0)
			{
				Rectangle rect = DisplayRectangle;
				Bitmap bitmap = new Bitmap(rect.Width, rect.Height);
				using (Graphics gr = Graphics.FromImage(bitmap))
				{
					gr.Clear(BackColor);
					DrawContext context = new DrawContext();
					context.Graphics = gr;
					context.Font = Font;
					context.Enabled = true;
					int y = 0;
					int maxWidth = 0;
					foreach (TreeNodeAdv node in nodes)
					{
						if (node.Tree == this)
						{
							int x = 0;
							int height = _rowLayout.GetRowBounds(node.Row).Height;
							foreach (NodeControl c in NodeControls)
							{
								int width = c.MeasureSize(node, context).Width;
								rect = new Rectangle(x, y, width, height);
								x += (width + 1);
								context.Bounds = rect;
								c.Draw(node, context);
							}
							y += height;
							maxWidth = Math.Max(maxWidth, x);
						}
					}

					if (maxWidth > 0 && y > 0)
					{
						_dragBitmap = new Bitmap(maxWidth, y, PixelFormat.Format32bppArgb);
						using (Graphics tgr = Graphics.FromImage(_dragBitmap))
							tgr.DrawImage(bitmap, Point.Empty);
						BitmapHelper.SetAlphaChanelValue(_dragBitmap, 150);
					}
					else
						_dragBitmap = null;
				}
			}
		}

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			ItemDragMode = false;
			Point pt = PointToClient(new Point(drgevent.X, drgevent.Y));
			if (_dragAutoScrollFlag)
				DragAutoScroll();
			SetDropPosition(pt);
			UpdateView();
			base.OnDragOver(drgevent);
		}

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			Search.EndSearch();
			DragMode = true;
			CreateDragBitmap(drgevent.Data);
			base.OnDragEnter(drgevent);
		}

		protected override void OnDragLeave(EventArgs e)
		{
			DragMode = false;
			UpdateView();
			base.OnDragLeave(e);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			DragMode = false;
			UpdateView();
			base.OnDragDrop(drgevent);
		}

		#endregion

	}
}

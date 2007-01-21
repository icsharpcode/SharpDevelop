using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Collections;
using System.Drawing.Design;
using Aga.Controls.Tree.NodeControls;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Aga.Controls.Tree
{
	public partial class TreeViewAdv : Control
	{
		private const int LeftMargin = 7;
		internal const int ItemDragSensivity = 4;
		private readonly int _columnHeaderHeight;
		private const int DividerWidth = 9; 

		private Pen _linePen;
		private Pen _markPen;
		private bool _suspendUpdate;
		private bool _needFullUpdate;
		private bool _fireSelectionEvent;
		private NodePlusMinus _plusMinus;
		private Control _currentEditor;
		private EditableControl _currentEditorOwner;
		private ToolTip _toolTip;
		private Timer _dragTimer;
		private IRowLayout _rowLayout;
		private DrawContext _measureContext;
		private TreeColumn _hotColumn = null;

		#region Public Events

		[Category("Action")]
		public event ItemDragEventHandler ItemDrag;
		private void OnItemDrag(MouseButtons buttons, object item)
		{
			if (ItemDrag != null)
				ItemDrag(this, new ItemDragEventArgs(buttons, item));
		}

		[Category("Behavior")]
		public event EventHandler<TreeNodeAdvMouseEventArgs> NodeMouseDoubleClick;
		private void OnNodeMouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			if (NodeMouseDoubleClick != null)
				NodeMouseDoubleClick(this, args);
		}

		[Category("Behavior")]
		public event EventHandler<TreeColumnEventArgs> ColumnWidthChanged;
		internal void OnColumnWidthChanged(TreeColumn column)
		{
			if (ColumnWidthChanged != null)
				ColumnWidthChanged(this, new TreeColumnEventArgs(column));
		}

		[Category("Behavior")]
		public event EventHandler<TreeColumnEventArgs> ColumnReordered;
		internal void OnColumnReordered(TreeColumn column)
		{
			if (ColumnReordered != null)
				ColumnReordered(this, new TreeColumnEventArgs(column));
		}

		[Category("Behavior")]
		public event EventHandler<TreeColumnEventArgs> ColumnClicked;
		internal void OnColumnClicked(TreeColumn column)
		{
			if (ColumnClicked != null)
				ColumnClicked(this, new TreeColumnEventArgs(column));
		}

		[Category("Behavior")]
		public event EventHandler SelectionChanged;
		internal void OnSelectionChanged()
		{
			if (SuspendSelectionEvent)
				_fireSelectionEvent = true;
			else
			{
				_fireSelectionEvent = false;
				if (SelectionChanged != null)
					SelectionChanged(this, EventArgs.Empty);
			}
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Collapsing;
		internal void OnCollapsing(TreeNodeAdv node)
		{
			if (Collapsing != null)
				Collapsing(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Collapsed;
		internal void OnCollapsed(TreeNodeAdv node)
		{
			if (Collapsed != null)
				Collapsed(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Expanding;
		internal void OnExpanding(TreeNodeAdv node)
		{
			if (Expanding != null)
				Expanding(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Expanded;
		internal void OnExpanded(TreeNodeAdv node)
		{
			//ListView t;
			if (Expanded != null)
				Expanded(this, new TreeViewAdvEventArgs(node));
		}

		#endregion

		public TreeViewAdv()
		{
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint
				| ControlStyles.UserPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.ResizeRedraw
				| ControlStyles.Selectable
				, true);


			if (Application.RenderWithVisualStyles)
				_columnHeaderHeight = 20;
			else
				_columnHeaderHeight = 17;

			BorderStyle = BorderStyle.Fixed3D;
			_hScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
			_vScrollBar.Width = SystemInformation.VerticalScrollBarWidth;
			_rowLayout = new FixedRowHeightLayout(this, RowHeight);
			_rowMap = new List<TreeNodeAdv>();
			_selection = new List<TreeNodeAdv>();
			_readonlySelection = new ReadOnlyCollection<TreeNodeAdv>(_selection);
			_columns = new TreeColumnCollection(this);
			_toolTip = new ToolTip();
			_dragTimer = new Timer();
			_dragTimer.Interval = 100;
			_dragTimer.Tick += new EventHandler(DragTimerTick);
			
			_measureContext = new DrawContext();
			_measureContext.Font = Font;
			_measureContext.Graphics = Graphics.FromImage(new Bitmap(1, 1));

			Input = new NormalInputState(this);
			Search = new IncrementalSearch();
			CreateNodes();
			CreatePens();

			ArrangeControls();

			_plusMinus = new NodePlusMinus();
			_controls = new NodeControlsCollection(this);
		}

		#region Public Methods

		public TreePath GetPath(TreeNodeAdv node)
		{
			if (node == _root)
				return TreePath.Empty;
			else
			{
				Stack<object> stack = new Stack<object>();
				while (node != _root && node != null)
				{
					stack.Push(node.Tag);
					node = node.Parent;
				}
				return new TreePath(stack.ToArray());
			}
		}

		public TreeNodeAdv GetNodeAt(Point point)
		{
			NodeControlInfo info = GetNodeControlInfoAt(point);
			return info.Node;
		}

		public NodeControlInfo GetNodeControlInfoAt(Point point)
		{
			if (point.X < 0 || point.Y < 0)
				return NodeControlInfo.Empty;

			int row = _rowLayout.GetRowAt(point);
			if (row < RowCount && row >= 0)
				return GetNodeControlInfoAt(_rowMap[row], point);
			else
				return NodeControlInfo.Empty;
		}

		public void BeginUpdate()
		{
			_suspendUpdate = true;
		}

		public void EndUpdate()
		{
			_suspendUpdate = false;
			if (_needFullUpdate)
				FullUpdate();
			else
				UpdateView();
		}

		public void ExpandAll()
		{
			BeginUpdate();
			SetIsExpanded(_root, true);
			EndUpdate();
		}

		public void CollapseAll()
		{
			BeginUpdate();
			SetIsExpanded(_root, false);
			EndUpdate();
		}


		/// <summary>
		/// Expand all parent nodes, andd scroll to the specified node
		/// </summary>
		public void EnsureVisible(TreeNodeAdv node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			if (!IsMyNode(node))
				throw new ArgumentException();

			TreeNodeAdv parent = node.Parent;
			while (parent != _root)
			{
				parent.IsExpanded = true;
				parent = parent.Parent;
			}
			ScrollTo(node);
		}

		/// <summary>
		/// Make node visible, scroll if needed. All parent nodes of the specified node must be expanded
		/// </summary>
		/// <param name="node"></param>
		public void ScrollTo(TreeNodeAdv node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			if (!IsMyNode(node))
				throw new ArgumentException();

			if (node.Row < 0)
				CreateRowMap();

			int row = -1;
 
			if (node.Row < FirstVisibleRow)
				row = node.Row;
			else
			{
				int pageStart = _rowLayout.GetRowBounds(FirstVisibleRow).Top;
				int rowBottom = _rowLayout.GetRowBounds(node.Row).Bottom;
				if (rowBottom > pageStart + DisplayRectangle.Height - ColumnHeaderHeight)
					row = _rowLayout.GetFirstRow(node.Row);
			}

			if (row >= _vScrollBar.Minimum && row <= _vScrollBar.Maximum)
				_vScrollBar.Value = row;
		}

		public void ClearSelection()
		{
			while (Selection.Count > 0)
				Selection[0].IsSelected = false;
		}

		#endregion

		protected override void OnSizeChanged(EventArgs e)
		{
			ArrangeControls();
			SafeUpdateScrollBars();
			base.OnSizeChanged(e);
		}

		private void ArrangeControls()
		{
			int hBarSize = _hScrollBar.Height;
			int vBarSize = _vScrollBar.Width;
			Rectangle clientRect = ClientRectangle;
			
			_hScrollBar.SetBounds(clientRect.X, clientRect.Bottom - hBarSize,
				clientRect.Width - vBarSize, hBarSize);

			_vScrollBar.SetBounds(clientRect.Right - vBarSize, clientRect.Y,
				vBarSize, clientRect.Height - hBarSize);
		}

		private void SafeUpdateScrollBars()
		{
			if (InvokeRequired)
				Invoke(new MethodInvoker(UpdateScrollBars));
			else
				UpdateScrollBars();
		}

		private void UpdateScrollBars()
		{
			UpdateVScrollBar();
			UpdateHScrollBar();
			UpdateVScrollBar();
			UpdateHScrollBar();
			_hScrollBar.Width = DisplayRectangle.Width;
			_vScrollBar.Height = DisplayRectangle.Height;
		}

		private void UpdateHScrollBar()
		{
			_hScrollBar.Maximum = ContentWidth;
			_hScrollBar.LargeChange = Math.Max(DisplayRectangle.Width, 0);
			_hScrollBar.SmallChange = 5;
			_hScrollBar.Visible = _hScrollBar.LargeChange < _hScrollBar.Maximum;
			_hScrollBar.Value = Math.Min(_hScrollBar.Value, _hScrollBar.Maximum - _hScrollBar.LargeChange + 1);
		}

		private void UpdateVScrollBar()
		{
			_vScrollBar.Maximum = Math.Max(RowCount - 1, 0);
			_vScrollBar.LargeChange = _rowLayout.PageRowCount;
			_vScrollBar.Visible = (RowCount > 0) && (_vScrollBar.LargeChange <= _vScrollBar.Maximum);
			_vScrollBar.Value = Math.Min(_vScrollBar.Value, _vScrollBar.Maximum - _vScrollBar.LargeChange + 1);
		}

		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				CreateParams res = base.CreateParams;
				switch (BorderStyle)
				{
					case BorderStyle.FixedSingle:
							res.Style |= 0x800000;
							break;
					case BorderStyle.Fixed3D:
							res.ExStyle |= 0x200;
						break;
				}
				return res;
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			HideEditor();
			UpdateView();
			ChangeInput();
			base.OnGotFocus(e);
		}

		protected override void OnLeave(EventArgs e)
		{
			HideEditor();
			UpdateView();
			base.OnLeave(e);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			_measureContext.Font = Font;
			_rowLayout.ClearCache();
			FullUpdate();
		}

		internal IEnumerable<NodeControlInfo> GetNodeControls(TreeNodeAdv node)
		{
			if (node == null)
				yield break;

			Rectangle rowRect = _rowLayout.GetRowBounds(node.Row);
			int y = rowRect.Y;
			int x = (node.Level - 1) * _indent + LeftMargin;
			int width = 0;
			Rectangle rect = Rectangle.Empty;

			if (ShowPlusMinus)
			{
				width = _plusMinus.MeasureSize(node, _measureContext).Width;
				rect = new Rectangle(x, y, width, rowRect.Height);
				if (UseColumns && Columns.Count > 0 && Columns[0].Width < rect.Right)
					rect.Width = Columns[0].Width - x;

				yield return new NodeControlInfo(_plusMinus, rect, node);
				x += (width + 1);
			}

			if (!UseColumns)
			{
				foreach (NodeControl c in NodeControls)
				{
					width = c.MeasureSize(node, _measureContext).Width;
					rect = new Rectangle(x, y, width, rowRect.Height);
					x += (width + 1);
					yield return new NodeControlInfo(c, rect, node);
				}
			}
			else
			{
				int right = 0;
				foreach (TreeColumn col in Columns)
				{
					if (col.IsVisible)
					{
						right += col.Width;
						for (int i = 0; i < NodeControls.Count; i++)
						{
							NodeControl nc = NodeControls[i];
							if (nc.ParentColumn == col)
							{
								bool isLastControl = true;
								for (int k = i + 1; k < NodeControls.Count; k++)
									if (NodeControls[k].ParentColumn == col)
									{
										isLastControl = false;
										break;
									}

								width = right - x;
								if (!isLastControl)
									width = nc.MeasureSize(node, _measureContext).Width;
								int maxWidth = Math.Max(0, right - x);
								rect = new Rectangle(x, y, Math.Min(maxWidth, width), rowRect.Height);
								x += (width + 1);
								yield return new NodeControlInfo(nc, rect, node);
							}
						}
						x = right;
					}
				}
			}
		}

		internal static double Dist(Point p1, Point p2)
		{
			return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
		}

		public void FullUpdate()
		{
			_rowLayout.ClearCache();
			CreateRowMap();
			SafeUpdateScrollBars();
			UpdateView();
			_needFullUpdate = false;
		}

		public void UpdateView()
		{
			if (!_suspendUpdate)
				Invalidate(false);
		}

		internal void UpdateHeaders()
		{
			Invalidate(new Rectangle(0,0, Width, ColumnHeaderHeight));
		}

		internal void UpdateColumns()
		{
			FullUpdate();
		}

		private void CreateNodes()
		{
			Selection.Clear();
			SelectionStart = null;
			_root = new TreeNodeAdv(this, null);
			_root.IsExpanded = true;
			if (_root.Nodes.Count > 0)
				CurrentNode = _root.Nodes[0];
			else
				CurrentNode = null;
		}

		internal void ReadChilds(TreeNodeAdv parentNode)
		{
			if (!parentNode.IsLeaf)
			{
				parentNode.IsExpandedOnce = true;

				List<TreeNodeAdv> oldNodes = new List<TreeNodeAdv>(parentNode.Nodes);
				parentNode.Nodes.Clear();

				if (Model != null)
				{
					IEnumerable items = Model.GetChildren(GetPath(parentNode));
					if (items != null)
						foreach (object obj in items)
						{
							bool found = false;
							if (obj != null)
							{
								for (int i = 0; i < oldNodes.Count; i++)
									if (obj == oldNodes[i].Tag)
									{
										AddNode(parentNode, -1, oldNodes[i]);
										oldNodes.RemoveAt(i);
										found = true;
										break;
									}
							}
							if (!found)
								AddNewNode(parentNode, obj, -1);
						}
				}
			}
		}

		private void AddNewNode(TreeNodeAdv parent, object tag, int index)
		{
			TreeNodeAdv node = new TreeNodeAdv(this, tag);
			AddNode(parent, index, node);
		}

		private void AddNode(TreeNodeAdv parent, int index, TreeNodeAdv node)
		{
			if (index >= 0 && index < parent.Nodes.Count)
				parent.Nodes.Insert(index, node);
			else
				parent.Nodes.Add(node);

			node.IsLeaf = Model.IsLeaf(GetPath(node));
			if (node.IsLeaf)
				node.Nodes.Clear();
			if (!LoadOnDemand || node.IsExpandedOnce)
				ReadChilds(node);
		}

		private void CreateRowMap()
		{
			_rowMap.Clear();
			int row = 0;
			_contentWidth = 0;
			foreach (TreeNodeAdv node in VisibleNodes)
			{
				node.Row = row;
				_rowMap.Add(node);
				if (!UseColumns)
				{
					Rectangle rect = GetNodeBounds(node);
					_contentWidth = Math.Max(_contentWidth, rect.Right);
				}
				row++;
			}
			if (UseColumns)
			{
				_contentWidth = 0;
				foreach (TreeColumn col in _columns)
					if (col.IsVisible)
						_contentWidth += col.Width;
			}
		}

		private NodeControlInfo GetNodeControlInfoAt(TreeNodeAdv node, Point point)
		{
			Rectangle rect = _rowLayout.GetRowBounds(FirstVisibleRow);
			point.Y += (rect.Y - ColumnHeaderHeight);
			point.X += OffsetX;
			foreach (NodeControlInfo info in GetNodeControls(node))
				if (info.Bounds.Contains(point))
					return info;

			return NodeControlInfo.Empty;
		}

		private Rectangle GetNodeBounds(TreeNodeAdv node)
		{
			Rectangle res = Rectangle.Empty;
			foreach (NodeControlInfo info in GetNodeControls(node))
			{
				if (res == Rectangle.Empty)
					res = info.Bounds;
				else
					res = Rectangle.Union(res, info.Bounds);
			}
			return res;
		}

		private void _vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			FirstVisibleRow = _vScrollBar.Value;
		}

		private void _hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			OffsetX = _hScrollBar.Value;
		}

		private void SetIsExpanded(TreeNodeAdv root, bool value)
		{
			foreach (TreeNodeAdv node in root.Nodes)
			{
				node.IsExpanded = value;
				SetIsExpanded(node, value);
			}
		}

		internal void SmartFullUpdate()
		{
			if (_suspendUpdate)
				_needFullUpdate = true;
			else
				FullUpdate();
		}

		internal bool IsMyNode(TreeNodeAdv node)
		{
			if (node == null)
				return false;

			if (node.Tree != this)
				return false;

			while (node.Parent != null)
				node = node.Parent;

			return node == _root;
		}

		public void UpdateSelection()
		{
			bool flag = false;

			if (!IsMyNode(CurrentNode))
				CurrentNode = null;
			if (!IsMyNode(_selectionStart))
				_selectionStart = null;

			for (int i = Selection.Count - 1; i >= 0; i--)
				if (!IsMyNode(Selection[i]))
				{
					flag = true;
					Selection.RemoveAt(i);
				}

			if (flag)
				OnSelectionChanged();
		}

		internal void ChangeColumnWidth(TreeColumn column)
		{
			if (!(_input is ResizeColumnState))
			{
				FullUpdate();
				OnColumnWidthChanged(column);
			}
		}

		#region Editor

		public void DisplayEditor(Control control, EditableControl owner)
		{
			if (control == null || owner == null)
				throw new ArgumentNullException();

			if (CurrentNode != null)
			{
				HideEditor();
				_currentEditor = control;
				_currentEditorOwner = owner;
				UpdateEditorBounds();

				UpdateView();
				control.Parent = this;
				control.Focus();
				owner.UpdateEditor(control);
			}
		}

		public void UpdateEditorBounds()
		{
			if (_currentEditor != null)
			{
				EditorContext context = new EditorContext();
				context.Owner = _currentEditorOwner;
				context.CurrentNode = CurrentNode;
				context.Editor = _currentEditor;
				context.DrawContext = _measureContext;

				SetEditorBounds(context);
			}
		}

		public void HideEditor()
		{
			if (_currentEditorOwner != null)
			{
				_currentEditorOwner.HideEditor(_currentEditor);
				_currentEditor = null;
				_currentEditorOwner = null;
			}
		}

		private void SetEditorBounds(EditorContext context)
		{
			foreach (NodeControlInfo info in GetNodeControls(context.CurrentNode))
			{
				if (context.Owner == info.Control && info.Control is EditableControl)
				{
					Point p = info.Bounds.Location;
					p.X -= OffsetX;
					p.Y -= (_rowLayout.GetRowBounds(FirstVisibleRow).Y - ColumnHeaderHeight);
					int width = DisplayRectangle.Width - p.X;
					if (UseColumns && info.Control.ParentColumn != null && Columns.Contains(info.Control.ParentColumn))
					{
						Rectangle rect = GetColumnBounds(info.Control.ParentColumn.Index);
						width = rect.Right - OffsetX - p.X;
					}
					context.Bounds = new Rectangle(p.X, p.Y, width, info.Bounds.Height);
					((EditableControl)info.Control).SetEditorBounds(context);
					return;
				}
			}
		}

		private Rectangle GetColumnBounds(int column)
		{
			int x = 0;
			for (int i = 0; i < Columns.Count; i++)
			{
				if (Columns[i].IsVisible)
				{
					if (i < column)
						x += Columns[i].Width;
					else
						return new Rectangle(x, 0, Columns[i].Width, 0);
				}
			}
			return Rectangle.Empty;
		}

		#endregion

		#region ModelEvents
		private void BindModelEvents()
		{
			_model.NodesChanged += new EventHandler<TreeModelEventArgs>(_model_NodesChanged);
			_model.NodesInserted += new EventHandler<TreeModelEventArgs>(_model_NodesInserted);
			_model.NodesRemoved += new EventHandler<TreeModelEventArgs>(_model_NodesRemoved);
			_model.StructureChanged += new EventHandler<TreePathEventArgs>(_model_StructureChanged);
		}

		private void UnbindModelEvents()
		{
			_model.NodesChanged -= new EventHandler<TreeModelEventArgs>(_model_NodesChanged);
			_model.NodesInserted -= new EventHandler<TreeModelEventArgs>(_model_NodesInserted);
			_model.NodesRemoved -= new EventHandler<TreeModelEventArgs>(_model_NodesRemoved);
			_model.StructureChanged -= new EventHandler<TreePathEventArgs>(_model_StructureChanged);
		}

		private void _model_StructureChanged(object sender, TreePathEventArgs e)
		{
			if (e.Path == null)
				throw new ArgumentNullException();

			TreeNodeAdv node = FindNode(e.Path);
			if (node != null)
			{
				ReadChilds(node);
				UpdateSelection();
				SmartFullUpdate();
			}
			else 
				throw new ArgumentException("Path not found");
		}

		private void _model_NodesRemoved(object sender, TreeModelEventArgs e)
		{
			TreeNodeAdv parent = FindNode(e.Path);
			if (parent != null)
			{
				if (e.Indices != null)
				{
					List<int> list = new List<int>(e.Indices);
					list.Sort();
					for (int n = list.Count - 1; n >= 0; n--)
					{
						int index = list[n];
						if (index >= 0 && index <= parent.Nodes.Count)
							parent.Nodes.RemoveAt(index);
						else
							throw new ArgumentOutOfRangeException("Index out of range");
					}
				}
				else
				{
					for (int i = parent.Nodes.Count - 1; i >= 0; i--)
					{
						for (int n = 0; n < e.Children.Length; n++)
							if (parent.Nodes[i].Tag == e.Children[n])
							{
								parent.Nodes.RemoveAt(i);
								break;
							}
					}
				}
			}
			UpdateSelection();
			SmartFullUpdate();
		}

		private void _model_NodesInserted(object sender, TreeModelEventArgs e)
		{
			if (e.Indices == null)
				throw new ArgumentNullException("Indices");

			TreeNodeAdv parent = FindNode(e.Path);
			if (parent != null)
			{
				for (int i = 0; i < e.Children.Length; i++)
					AddNewNode(parent, e.Children[i], e.Indices[i]);
			}
			SmartFullUpdate();
		}

		private void _model_NodesChanged(object sender, TreeModelEventArgs e)
		{
			TreeNodeAdv parent = FindNode(e.Path);
			if (parent != null)
			{
				if (e.Indices != null)
				{
					foreach (int index in e.Indices)
					{
						if (index >= 0 && index < parent.Nodes.Count)
						{
							TreeNodeAdv node = parent.Nodes[index];
							Rectangle rect = GetNodeBounds(node);
							_contentWidth = Math.Max(_contentWidth, rect.Right);
						}
						else
							throw new ArgumentOutOfRangeException("Index out of range");
					}
				}
				else
				{
					foreach (TreeNodeAdv node in parent.Nodes)
					{
						foreach (object obj in e.Children)
							if (node.Tag == obj)
							{
								Rectangle rect = GetNodeBounds(node);
								_contentWidth = Math.Max(_contentWidth, rect.Right);
							}
					}
				}
			}
			_rowLayout.ClearCache();
			SafeUpdateScrollBars();
			UpdateView();
		}

		public TreeNodeAdv FindNode(TreePath path)
		{
			if (path.IsEmpty())
				return _root;
			else
				return FindNode(_root, path, 0);
		}

		private TreeNodeAdv FindNode(TreeNodeAdv root, TreePath path, int level)
		{
			foreach (TreeNodeAdv node in root.Nodes)
				if (node.Tag == path.FullPath[level])
				{
					if (level == path.FullPath.Length - 1)
						return node;
					else
						return FindNode(node, path, level + 1);
				}
			return null;
		}
		#endregion
	}
}

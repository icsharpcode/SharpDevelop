using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Drawing.Design;

using Aga.Controls.Tree.NodeControls;

namespace Aga.Controls.Tree
{
	public partial class TreeViewAdv
	{
		private Cursor _innerCursor = null;

		public override Cursor Cursor
		{
			get
			{
				if (_innerCursor != null)
					return _innerCursor;
				else
					return base.Cursor;
			}
			set
			{
				base.Cursor = value;
			}
		}

		#region Internal Properties

		private bool _dragMode;
		private bool DragMode
		{
			get { return _dragMode; }
			set
			{
				_dragMode = value;
				_dragTimer.Enabled = value;
				if (!value)
				{
					if (_dragBitmap != null)
						_dragBitmap.Dispose();
					_dragBitmap = null;
				}
			}
		}

		internal int ColumnHeaderHeight
		{
			get
			{
				if (UseColumns)
					return _columnHeaderHeight;
				else
					return 0;
			}
		}

		/// <summary>
		/// returns all nodes, which parent is expanded
		/// </summary>
		private IEnumerable<TreeNodeAdv> VisibleNodes
		{
			get
			{
				TreeNodeAdv node = Root;
				while (node != null)
				{
					node = node.NextVisibleNode;
					if (node != null)
						yield return node;
				}
			}
		}

		private bool _suspendSelectionEvent;
		internal bool SuspendSelectionEvent
		{
			get { return _suspendSelectionEvent; }
			set
			{
				_suspendSelectionEvent = value;
				if (!_suspendSelectionEvent && _fireSelectionEvent)
					OnSelectionChanged();
			}
		}

		private List<TreeNodeAdv> _rowMap;
		internal List<TreeNodeAdv> RowMap
		{
			get { return _rowMap; }
		}

		private TreeNodeAdv _selectionStart;
		internal TreeNodeAdv SelectionStart
		{
			get { return _selectionStart; }
			set { _selectionStart = value; }
		}

		private InputState _input;
		internal InputState Input
		{
			get { return _input; }
			set
			{
				_input = value;
			}
		}

		private bool _itemDragMode;
		internal bool ItemDragMode
		{
			get { return _itemDragMode; }
			set { _itemDragMode = value; }
		}

		private Point _itemDragStart;
		internal Point ItemDragStart
		{
			get { return _itemDragStart; }
			set { _itemDragStart = value; }
		}


		/// <summary>
		/// Number of rows fits to the current page
		/// </summary>
		internal int CurrentPageSize
		{
			get
			{
				return _rowLayout.CurrentPageSize;
			}
		}

		/// <summary>
		/// Number of all visible nodes (which parent is expanded)
		/// </summary>
		internal int RowCount
		{
			get
			{
				return _rowMap.Count;
			}
		}

		private int _contentWidth = 0;
		private int ContentWidth
		{
			get
			{
				return _contentWidth;
			}
		}

		private int _firstVisibleRow;
		internal int FirstVisibleRow
		{
			get { return _firstVisibleRow; }
			set
			{
				HideEditor();
				_firstVisibleRow = value;
				UpdateView();
			}
		}

		private int _offsetX;
		internal int OffsetX
		{
			get { return _offsetX; }
			private set
			{
				HideEditor();
				_offsetX = value;
				UpdateView();
			}
		}

		public override Rectangle DisplayRectangle
		{
			get
			{
				Rectangle r = ClientRectangle;
				//r.Y += ColumnHeaderHeight;
				//r.Height -= ColumnHeaderHeight;
				int w = _vScrollBar.Visible ? _vScrollBar.Width : 0;
				int h = _hScrollBar.Visible ? _hScrollBar.Height : 0;
				return new Rectangle(r.X, r.Y, r.Width - w, r.Height - h);
			}
		}

		private List<TreeNodeAdv> _selection;
		internal List<TreeNodeAdv> Selection
		{
			get { return _selection; }
		}

		#endregion

		#region Public Properties

		#region DesignTime

		private IncrementalSearch _search;
		[Category("Behavior"), TypeConverter(typeof(ExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IncrementalSearch Search
		{
			get { return _search; }
			set 
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_search = value;
				_search.Tree = this;
			}
		}

		private bool _displayDraggingNodes;
		[DefaultValue(false), Category("Behavior")]
		public bool DisplayDraggingNodes
		{
			get { return _displayDraggingNodes; }
			set { _displayDraggingNodes = value; }
		}

		private bool _fullRowSelect;
		[DefaultValue(false), Category("Behavior")]
		public bool FullRowSelect
		{
			get { return _fullRowSelect; }
			set
			{
				_fullRowSelect = value;
				UpdateView();
			}
		}

		private bool _useColumns;
		[DefaultValue(false), Category("Behavior")]
		public bool UseColumns
		{
			get { return _useColumns; }
			set
			{
				_useColumns = value;
				FullUpdate();
			}
		}

		private bool _allowColumnReorder;
		[DefaultValue(false), Category("Behavior")]
		public bool AllowColumnReorder
		{
			get { return _allowColumnReorder; }
			set { _allowColumnReorder = value; }
		}

		private bool _showLines = true;
		[DefaultValue(true), Category("Behavior")]
		public bool ShowLines
		{
			get { return _showLines; }
			set
			{
				_showLines = value;
				UpdateView();
			}
		}

		private bool _showPlusMinus = true;
		[DefaultValue(true), Category("Behavior")]
		public bool ShowPlusMinus
		{
			get { return _showPlusMinus; }
			set
			{
				_showPlusMinus = value;
				FullUpdate();
			}
		}

		private bool _showNodeToolTips = false;
		[DefaultValue(false), Category("Behavior")]
		public bool ShowNodeToolTips
		{
			get { return _showNodeToolTips; }
			set { _showNodeToolTips = value; }
		}

		[DefaultValue(true), Category("Behavior"), Obsolete("No longer used")]
		public bool KeepNodesExpanded
		{
			get { return true; }
			set {}
		}

		private ITreeModel _model;
		[Category("Data")]
		public ITreeModel Model
		{
			get { return _model; }
			set
			{
				if (_model != value)
				{
					if (_model != null)
						UnbindModelEvents();
					_model = value;
					CreateNodes();
					FullUpdate();
					if (_model != null)
						BindModelEvents();
				}
			}
		}

		private BorderStyle _borderStyle;
		[DefaultValue(BorderStyle.Fixed3D), Category("Appearance")]
		public BorderStyle BorderStyle
		{
			get
			{
				return this._borderStyle;
			}
			set
			{
				if (_borderStyle != value)
				{
					_borderStyle = value;
					base.UpdateStyles();
				}
			}
		}

		private bool _autoRowHeight = false;
		[DefaultValue(false), Category("Appearance")]
		public bool AutoRowHeight
		{
			get
			{
				return _autoRowHeight;
			}
			set
			{
				_autoRowHeight = value;
				if (value)
					_rowLayout = new AutoRowHeightLayout(this, RowHeight);
				else
					_rowLayout = new FixedRowHeightLayout(this, RowHeight);
				FullUpdate();
			}
		}

		private int _rowHeight = 16;
		[DefaultValue(16), Category("Appearance")]
		public int RowHeight
		{
			get
			{
				return _rowHeight;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException("value");

				_rowHeight = value;
				_rowLayout.PreferredRowHeight = value;
				FullUpdate();
			}
		}

		private TreeSelectionMode _selectionMode = TreeSelectionMode.Single;
		[DefaultValue(TreeSelectionMode.Single), Category("Behavior")]
		public TreeSelectionMode SelectionMode
		{
			get { return _selectionMode; }
			set { _selectionMode = value; }
		}

		private bool _hideSelection;
		[DefaultValue(false), Category("Behavior")]
		public bool HideSelection
		{
			get { return _hideSelection; }
			set
			{
				_hideSelection = value;
				UpdateView();
			}
		}

		private float _topEdgeSensivity = 0.3f;
		[DefaultValue(0.3f), Category("Behavior")]
		public float TopEdgeSensivity
		{
			get { return _topEdgeSensivity; }
			set
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException();
				_topEdgeSensivity = value;
			}
		}

		private float _bottomEdgeSensivity = 0.3f;
		[DefaultValue(0.3f), Category("Behavior")]
		public float BottomEdgeSensivity
		{
			get { return _bottomEdgeSensivity; }
			set
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("value should be from 0 to 1");
				_bottomEdgeSensivity = value;
			}
		}

		private bool _loadOnDemand;
		[DefaultValue(false), Category("Behavior")]
		public bool LoadOnDemand
		{
			get { return _loadOnDemand; }
			set { _loadOnDemand = value; }
		}

		private int _indent = 19;
		[DefaultValue(19), Category("Behavior")]
		public int Indent
		{
			get { return _indent; }
			set
			{
				_indent = value;
				UpdateView();
			}
		}

		private Color _lineColor = SystemColors.ControlDark;
		[Category("Behavior")]
		public Color LineColor
		{
			get { return _lineColor; }
			set
			{
				_lineColor = value;
				CreateLinePen();
				UpdateView();
			}
		}

		private Color _dragDropMarkColor = Color.Black;
		[Category("Behavior")]
		public Color DragDropMarkColor
		{
			get { return _dragDropMarkColor; }
			set
			{
				_dragDropMarkColor = value;
				CreateMarkPen();
			}
		}

		private float _dragDropMarkWidth = 3.0f;
		[DefaultValue(3.0f), Category("Behavior")]
		public float DragDropMarkWidth
		{
			get { return _dragDropMarkWidth; }
			set
			{
				_dragDropMarkWidth = value;
				CreateMarkPen();
			}
		}

		private TreeColumnCollection _columns;
		[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Collection<TreeColumn> Columns
		{
			get { return _columns; }
		}

		private NodeControlsCollection _controls;
		[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(NodeControlCollectionEditor), typeof(UITypeEditor))]
		public Collection<NodeControl> NodeControls
		{
			get
			{
				return _controls;
			}
		}

		#endregion

		#region RunTime

		private IToolTipProvider _defaultToolTipProvider = null;
		[Browsable(false)]
		public IToolTipProvider DefaultToolTipProvider
		{
			get { return _defaultToolTipProvider; }
			set { _defaultToolTipProvider = value; }
		}

		[Browsable(false)]
		public IEnumerable<TreeNodeAdv> AllNodes
		{
			get
			{
				if (_root.Nodes.Count > 0)
				{
					TreeNodeAdv node = _root.Nodes[0];
					while (node != null)
					{
						yield return node;
						if (node.Nodes.Count > 0)
							node = node.Nodes[0];
						else if (node.NextNode != null)
							node = node.NextNode;
						else
							node = node.BottomNode;
					}
				}
			}
		}

		private DropPosition _dropPosition;
		[Browsable(false)]
		public DropPosition DropPosition
		{
			get { return _dropPosition; }
			set { _dropPosition = value; }
		}

		private TreeNodeAdv _root;
		[Browsable(false)]
		public TreeNodeAdv Root
		{
			get { return _root; }
		}

		private ReadOnlyCollection<TreeNodeAdv> _readonlySelection;
		[Browsable(false)]
		public ReadOnlyCollection<TreeNodeAdv> SelectedNodes
		{
			get
			{
				return _readonlySelection;
			}
		}

		[Browsable(false)]
		public TreeNodeAdv SelectedNode
		{
			get
			{
				if (Selection.Count > 0)
				{
					if (CurrentNode != null && CurrentNode.IsSelected)
						return CurrentNode;
					else
						return Selection[0];
				}
				else
					return null;
			}
			set
			{
				if (SelectedNode == value)
					return;

				BeginUpdate();
				try
				{
					if (value == null)
					{
						ClearSelection();
					}
					else
					{
						if (!IsMyNode(value))
							throw new ArgumentException();

						ClearSelection();
						value.IsSelected = true;
						CurrentNode = value;
						EnsureVisible(value);
					}
				}
				finally
				{
					EndUpdate();
				}
			}
		}

		private TreeNodeAdv _currentNode;
		[Browsable(false)]
		public TreeNodeAdv CurrentNode
		{
			get { return _currentNode; }
			internal set { _currentNode = value; }
		}


		#endregion

		#endregion

	}
}

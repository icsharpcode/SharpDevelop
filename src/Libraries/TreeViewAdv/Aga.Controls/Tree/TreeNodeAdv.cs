using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Aga.Controls.Tree
{
	[Serializable]
	public sealed class TreeNodeAdv: ISerializable
	{
		#region NodeCollection
		//TODO: use one common Parent-Children collection
		private class NodeCollection : Collection<TreeNodeAdv>
		{
			private TreeNodeAdv _owner;

			public NodeCollection(TreeNodeAdv owner)
			{
				_owner = owner;
			}

			protected override void ClearItems()
			{
				while (this.Count != 0)
					this.RemoveAt(this.Count - 1);
			}

			protected override void InsertItem(int index, TreeNodeAdv item)
			{
				if (item == null)
					throw new ArgumentNullException("item");

				if (item.Parent != _owner)
				{
					if (item.Parent != null)
						item.Parent.Nodes.Remove(item);
					item._parent = _owner;
					base.InsertItem(index, item);
				}
			}

			protected override void RemoveItem(int index)
			{
				TreeNodeAdv item = this[index];
				item._parent = null;
				base.RemoveItem(index);
			}

			protected override void SetItem(int index, TreeNodeAdv item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				RemoveAt(index);
				InsertItem(index, item);
			}
		}
		#endregion

		private Collection<TreeNodeAdv> _nodes;
		private ReadOnlyCollection<TreeNodeAdv> _children;

		#region Properties

		private TreeViewAdv _tree;
		internal TreeViewAdv Tree
		{
			get { return _tree; }
		}

		private int _row;
		internal int Row
		{
			get { return _row; }
			set { _row = value; }
		}

		public int Index
		{
			get
			{
				if (_parent == null)
					return -1;
				else
					return _parent.Nodes.IndexOf(this);
			}
		}

		private bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set 
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					if (Tree.IsMyNode(this))
					{
						if (_isSelected)
						{
							if (!_tree.Selection.Contains(this))
								_tree.Selection.Add(this);

							if (_tree.Selection.Count == 1)
								_tree.CurrentNode = this;
						}
						else
							_tree.Selection.Remove(this);
						_tree.UpdateView();
						_tree.OnSelectionChanged();
					}
				}
			}
		}

		private bool _isLeaf;
		public bool IsLeaf
		{
			get { return _isLeaf; }
			internal set { _isLeaf = value; }
		}

		private bool _isExpandedOnce;
		public bool IsExpandedOnce
		{
			get { return _isExpandedOnce; }
			internal set { _isExpandedOnce = value; }
		}

		private bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set 
			{
				if (Tree == null)
					_isExpanded = value;
				else if (Tree.IsMyNode(this) && _isExpanded != value)
					AssignIsExpanded(value);
			}
		}

		private TreeNodeAdv _parent;
		public TreeNodeAdv Parent
		{
			get { return _parent; }
			//internal set { _parent = value; }
		}

		public int Level
		{
			get
			{
				if (_parent == null)
					return 0;
				else
					return _parent.Level + 1;
			}
		}

		public TreeNodeAdv NextNode
		{
			get
			{
				if (_parent != null)
				{
					int index = _parent.Nodes.IndexOf(this);
					if (index < _parent.Nodes.Count - 1)
						return _parent.Nodes[index + 1];
				}
				return null;
			}
		}

		internal TreeNodeAdv BottomNode
		{
			get
			{
				TreeNodeAdv parent = this.Parent;
				if (parent != null)
				{
					if (parent.NextNode != null)
						return parent.NextNode;
					else
						return parent.BottomNode;
				}
				return null;
			}
		}

		internal TreeNodeAdv NextVisibleNode
		{
			get
			{
				if (IsExpanded && Nodes.Count > 0)
					return Nodes[0];
				else if (NextNode != null)
					return NextNode;
				else
					return BottomNode;
			}
		}

		public bool CanExpand
		{
			get
			{
				return (_nodes.Count > 0 || (!IsExpandedOnce && !IsLeaf));
			}
		}

		private object _tag;
		public object Tag
		{
			get { return _tag; }
		}

		internal Collection<TreeNodeAdv> Nodes
		{
			get { return _nodes; }
		}

		public ReadOnlyCollection<TreeNodeAdv> Children
		{
			get
			{
				return _children;
			}
		}

		#endregion

		public TreeNodeAdv(object tag): this(null, tag)
		{
		}

		internal TreeNodeAdv(TreeViewAdv tree, object tag)
		{
			_row = -1;
			_tree = tree;
			_nodes = new NodeCollection(this);
			_children = new ReadOnlyCollection<TreeNodeAdv>(_nodes);
			_tag = tag;
		}

		public override string ToString()
		{
			if (Tag != null)
				return Tag.ToString();
			else
				return base.ToString();
		}

		private void AssignIsExpanded(bool value)
		{
			if (value)
				Tree.OnExpanding(this);
			else
				Tree.OnCollapsing(this);

			if (value && !_isExpandedOnce)
			{
				Cursor oldCursor = Tree.Cursor;
				Tree.Cursor = Cursors.WaitCursor;
				try
				{
					Tree.ReadChilds(this);
				}
				finally
				{
					Tree.Cursor = oldCursor;
				}
			}
			_isExpanded = value;
			Tree.SmartFullUpdate();

			if (value)
				Tree.OnExpanded(this);
			else
				Tree.OnCollapsed(this);
		}

		#region ISerializable Members

        private TreeNodeAdv(SerializationInfo info, StreamingContext context): this(null, null)
        {
			int nodesCount = 0;
			nodesCount = info.GetInt32("NodesCount");
			_isExpanded = info.GetBoolean("IsExpanded");
			_tag = info.GetValue("Tag", typeof(object));

			for (int i = 0; i < nodesCount; i++)
			{
				TreeNodeAdv child = (TreeNodeAdv)info.GetValue("Child" + i, typeof(TreeNodeAdv));
				_nodes.Add(child);
			}

        }

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("IsExpanded", IsExpanded);
			info.AddValue("NodesCount", Nodes.Count);
			if ((Tag != null) && Tag.GetType().IsSerializable)
				info.AddValue("Tag", Tag, Tag.GetType());

			for (int i = 0; i < Nodes.Count; i++)
				info.AddValue("Child" + i, Nodes[i], typeof(TreeNodeAdv));

		}

		#endregion
	}
}

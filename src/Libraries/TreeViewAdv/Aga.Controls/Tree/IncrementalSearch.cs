using System;
using System.Collections.Generic;
using System.Text;
using Aga.Controls.Tree.NodeControls;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Aga.Controls.Tree
{
	public class IncrementalSearch
	{
		#region Properties

		private IncrementalSearchMode _mode = IncrementalSearchMode.Standard;
		[DefaultValue(IncrementalSearchMode.Standard)]
		public IncrementalSearchMode Mode
		{
			get { return _mode; }
			set 
			{
				if (value != _mode)
				{
					_mode = value;
					EndSearch();
				}
			}
		}

		private Color _backColor = Color.Pink;
		public Color BackColor
		{
			get { return _backColor; }
			set { _backColor = value; }
		}

		private Color _fontColor = Color.Black;
		public Color FontColor
		{
			get { return _fontColor; }
			set { _fontColor = value; }
		}

		private TreeViewAdv _tree;
		[Browsable(false)]
		public TreeViewAdv Tree
		{
			get { return _tree; }
			internal set { _tree = value; }
		}

		private TreeNodeAdv _currentNode;
		protected TreeNodeAdv CurrentNode
		{
			get { return _currentNode; }
		}

		private NodeControl _currentControl;
		protected NodeControl CurrentControl
		{
			get { return _currentControl; }
		}

		private bool _isActive;
		[Browsable(false)]
		public bool IsActive
		{
			get { return _isActive; }
			protected set { _isActive = value; }
		}

		#endregion

		public override string ToString()
		{
			return GetType().Name;
		}

		public virtual void Search(Char value)
		{
			if (!Char.IsControl(value))
			{
				if (Mode == IncrementalSearchMode.Standard)
					StandardSearch(value);
				else if (Mode == IncrementalSearchMode.Continuous)
					ContinuousSearch(value);
			}
		}

		private void ContinuousSearch(Char value)
		{
			if (value == ' ' && String.IsNullOrEmpty(_searchString))
				return; //Ingnore leading space

			IsActive = true;
			_searchString += Char.ToLowerInvariant(value);

			if (!DoContinuousSearch())
				RemoveLastChar();

			Tree.UpdateView();
		}

		private void StandardSearch(Char value)
		{
			if (value == ' ')
				return;

			string ch = char.ToLowerInvariant(value).ToString();
			TreeNodeAdv node = null;
			if (Tree.SelectedNode != null)
				node = Tree.SelectedNode.NextVisibleNode;
			if (node == null)
				node = Tree.Root;

			foreach (string label in IterateNodeLabels(node))
			{
				if (label.StartsWith(ch))
				{
					Tree.SelectedNode = CurrentNode;
					return;
				}
			}
		}

		public virtual void EndSearch()
		{
			_currentControl = null;
			_currentNode = null;
			if (IsActive)
			{
				_searchString = "";
				IsActive = false;
				Tree.UpdateView();
			}
		}

		public virtual void Draw(DrawContext context)
		{
			if (Mode == IncrementalSearchMode.Continuous)
			{
				if (!String.IsNullOrEmpty(_searchString) && CurrentNode != null)
				{
					foreach (NodeControlInfo info in Tree.GetNodeControls(CurrentNode))
						if (info.Control == _selectedControl)
						{
							SizeF ms = context.Graphics.MeasureString(_searchString, context.Font);
							RectangleF rect = info.Bounds;
							rect.Width = ms.Width;
							using (Brush backBrush = new SolidBrush(BackColor),
								fontBrush = new SolidBrush(FontColor))
							{
								context.Graphics.FillRectangle(backBrush, rect);
								context.Graphics.DrawString(_searchString, context.Font, fontBrush, rect);
							}
							break;
						}
				}
			}
		}

		public virtual void KeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			if (Mode == IncrementalSearchMode.Continuous)
			{
				if (e.KeyCode == Keys.Back)
				{
					e.Handled = true;
					e.SuppressKeyPress = true;
					RemoveLastChar();
					DoContinuousSearch();
					Tree.UpdateView();
				}
				else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down
					|| e.KeyCode == Keys.Left || e.KeyCode == Keys.Right
					|| e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
					EndSearch();
				else
					e.Handled = true;
			}
		}

		protected IEnumerable<string> IterateNodeLabels(TreeNodeAdv start)
		{
			_currentNode = start;
			while(_currentNode != null)
			{
				foreach (string label in GetNodeLabels(_currentNode))
					yield return label;

				_currentNode = _currentNode.NextVisibleNode;
				if (_currentNode == null)
					_currentNode = Tree.Root;

				if (start == _currentNode)
					break;
			} 
		}

		private IEnumerable<string> GetNodeLabels(TreeNodeAdv node)
		{
			foreach (NodeControl nc in Tree.NodeControls)
			{
				_currentControl = nc;
				BindableControl bc = nc as BindableControl;
				if (bc != null && bc.IncrementalSearchEnabled)
				{
					object obj = bc.GetValue(node);
					if (obj != null)
						yield return obj.ToString().ToLowerInvariant();
				}
			}
		}

		private string _searchString = "";
		private NodeControl _selectedControl;

		private void RemoveLastChar()
		{
			if (_searchString.Length > 0)
				_searchString = _searchString.Substring(0, _searchString.Length - 1);
		}

		private bool DoContinuousSearch()
		{
			bool found = false;
			if (!String.IsNullOrEmpty(_searchString))
			{
				TreeNodeAdv node = null;
				if (Tree.SelectedNode != null)
					node = Tree.SelectedNode;
				if (node == null)
					node = Tree.Root.NextVisibleNode;

				if (!String.IsNullOrEmpty(_searchString))
				{
					foreach (string label in IterateNodeLabels(node))
					{
						if (label.StartsWith(_searchString))
						{
							found = true;
							_selectedControl = CurrentControl;
							Tree.SelectedNode = CurrentNode;
							break;
						}
					}
				}
			}
			return found;
		}

	}
}

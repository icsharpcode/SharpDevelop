// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using NoGoop.Win32;

namespace NoGoop.Controls
{
	// You must add the parent of this control to the control
	// you want to be the parent.  The parent of this control
	// is a TreeListPanel which does some funny things in the
	// WndProc.
	public class TreeListView : TreeView
	{
		protected TreeListPanel         _panel;
		
		// See comments below
		protected bool                  _inMouseDown;
		protected bool                  _selected;
		
		protected bool                  _useCompareTo;
		
		// For now, just make this an untyped array list
		// Though its really supposed to contain only column headers
		public class ColumnHeaderCollection : ArrayList
		{
		}
		
		protected ColumnHeaderCollection _columns;
		
		internal Panel Panel {
			get {
				return _panel;
			}
		}
		
		internal bool UseCompareTo {
			get {
				return _useCompareTo;
			}
			set {
				_useCompareTo = value;
			}
		}
		
		public TreeListView()
		{
			_columns = new ColumnHeaderCollection();
			_panel = new TreeListPanel();
			_panel.TreeListView = this;
			Scrollable = true;
		}
		
		// Called after this control is set up with its properties
		internal void SetupPanel()
		{
			_panel.Setup();
		}
		
		internal ColumnHeaderCollection Columns {
			get {
				return _columns;
			}
		}
		
		internal TreeNode FindNodeByFullPath(String fullPath)
		{
			return FindNodeByFullPathInt(Nodes, fullPath);
		}
		
		internal TreeNode FindNodeByFullPathInt(TreeNodeCollection nodes, String fullPath)
		{
			int pathSep = fullPath.IndexOf(PathSeparator);
			String partPath;
			if (pathSep == -1)
				partPath = fullPath;
			else
				partPath = fullPath.Substring(0, pathSep);
			
			foreach (TreeNode node in nodes)
			{
				if (node.Text.Equals(partPath))
				{
					// We are at the bottom
					if (pathSep == -1)
						return node;
					String restPath = fullPath.Substring
						(PathSeparator.Length + pathSep);
					return FindNodeByFullPathInt(node.Nodes,
												restPath);
				}
			}
			// Not found
			return null;
		}
		
		// Used to allow normal selection processing if the node
		// is explicitly selected
		// This must be used instead of the SelectedNode property to
		// *set* the SelectedNode
		public void SetSelectedNode(TreeNode node)
		{
			// Allow selection processing to happen
			_selected = false;
			SelectedNode = node;
		}
		
		// This is here just to invoke this private method using
		// reflection
		internal TreeListNode GetNodeFromHandle(int handle)
		{
			MethodInfo m = typeof(TreeView).GetMethod("NodeFromHandle", 
													 BindingFlags.Instance | 
													 BindingFlags.NonPublic);
			ParameterInfo[] p = m.GetParameters();
			Object[] paramValues = new Object[1];
			paramValues[0] = (IntPtr)handle;
			return (TreeListNode)m.Invoke(this, paramValues);
		}
		
		internal virtual void HScrolled()
		{
			// Meant to be overridden
		}
		
		internal virtual void VScrolled()
		{
			// Meant to be overridden
		}
		
		internal void Add(TreeNodeCollection parent, TreeListNode node)
		{
			if (!_useCompareTo)
			{
				parent.Add(node);
				return;
			}
			/*
			Console.WriteLine("AddSorted: " + node);
			Console.WriteLine("Current nodes: ");
			for (int j = 0; j < parent.Count; j++)
				Console.WriteLine(" " + parent[j]);
			*/
			int i;
			for (i = parent.Count - 1; i >= 0; i--)
			{
				TreeListNode lookNode = (TreeListNode)parent[i];
				//                Console.WriteLine("looking at: " + lookNode
				//                                  + " (" + i + ")");
				if (lookNode.CompareTo(node) <= 0)
					break;
			}
			parent.Insert(i + 1, node);
		}
		
		// Called by the OnBeforeSelect handler to determine
		// if its valid to respond to the event.  If this is response
		// to a mouse down event, because of the code below that selects
		// the mode in response to the mouse down event we have to do
		// some special things.  The problem is, we select the node below
		// but then if the tree shifts, later, in handling the same mouse
		// down event, the regular TreeView processing selects the node
		// at the original mouse position causing the wrong node to end
		// up being selected.  This way, we allow only one select per
		// mouse-down event.
		protected bool CanSelectNode()
		{
			if (_inMouseDown)
			{
				if (!_selected)
					return true;
				return false;
			}
			return true;
		}
		
		// Make sure we select the node if its clicked anywhere
		protected override void OnMouseDown(MouseEventArgs e)
		{
			TreeNode node = GetNodeAt(0, e.Y); 
			SelectedNode = node;
			// Prevent further selections in this event sequence,
			// see comment above.
			_selected = true;
			base.OnMouseDown(e);
		}
		
		protected override void WndProc(ref Message m)
		{
			bool mouseDown = false;
			switch (m.Msg) { 
			case Windows.WM_HSCROLL:
				HScrolled();
				break;
			case Windows.WM_VSCROLL:
				VScrolled();
				break;
			case Windows.WM_LBUTTONDOWN:
				mouseDown = true;
				break;
			case Windows.WM_RBUTTONDOWN:
				mouseDown = true;
				break;
			}
			if (mouseDown)
			{
				_inMouseDown = true;
				_selected = false;
			}
			base.WndProc(ref m);
			if (mouseDown)
			{
				_inMouseDown = false;
			}
		}
	}
}

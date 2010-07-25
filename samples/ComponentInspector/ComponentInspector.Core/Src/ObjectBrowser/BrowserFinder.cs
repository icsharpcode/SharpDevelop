// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{

	public delegate bool SearchNodeDelegate(BrowserFinder finder,
											ISearchNode node);

	public delegate void SearchStatusDelegate(String action,
											  String obj);

	public delegate void SearchInvalidateDelegate(BrowserFinder finder);


	// This class finds nodes in a browser tree
	// Create a new one for each search

	// We go to the trouble of not expanding the tree we are
	// searching because we don't want to take the memory
	// associated with touching everything.  For the object tree
	// we don't expand at all, because of the possibility of
	// infinite expansion.  For the other trees, we cause the
	// underlying structures to be built, but the trees are not
	// expanded as the search progresses.  This is all controlled
	// using the ISearchNode interface.  Sometimes a search node
	// is a BrowserTreeNode, sometimes its one of the underlying
	// objects, like BasicInfo.  This is control by the classes
	// the make up the trees.

	public class BrowserFinder : ISearcher
	{

		protected String                _findWhat;

		protected int                   _compareType;

		protected int                   _maxLevel;

		// Use the node's name
		protected bool                  _useName;
		// Use the node's value
		protected bool                  _useValue;

		protected SearchNodeDelegate    _nodeFound;
		protected SearchNodeDelegate    _nodeLooking;
		protected SearchStatusDelegate  _searchStatus;
		protected SearchInvalidateDelegate  _searchInvalidate;

		// Stupid compiler
		internal BrowserTreeNode        _startNode;

		// The tree being searched
		protected TreeView              _tree;

		protected Stack                 _searchStack;


		internal const bool EXPAND = true;

		internal const int COMPARE_FULL         = 1;
		internal const int COMPARE_STARTS       = 2;
		internal const int COMPARE_CONTAINS     = 3;

		// A really high level number - for all levels
		internal const int ALL_LEVELS           = 1000;

		protected static BrowserFinder  _browserFinder;

		internal static BrowserFinder BFinder {
			get {
				return _browserFinder;
			}
		}

		public Stack SearchStack {
			get {
				return _searchStack;
			}
		}

		internal BrowserFinder(String findWhat,
							   int compareType,
							   int maxLevel,
							   bool useName,
							   bool useValue,
							   BrowserTreeNode startNode, 
							   SearchNodeDelegate nodeFound,
							   SearchNodeDelegate nodeLooking,
							   SearchStatusDelegate searchStatus,
							   SearchInvalidateDelegate searchInvalidate)
		{
			_findWhat = findWhat;
			_compareType = compareType;
			_maxLevel = maxLevel;
			_useName = useName;
			_useValue = useValue;
			_startNode = startNode;
			_tree = startNode.TreeView;
			_nodeFound = nodeFound;
			_nodeLooking = nodeLooking;
			_searchStatus = searchStatus;
			_searchInvalidate = searchInvalidate;
			_searchStack = new Stack();
			_browserFinder = this;
		}

		// ISearcher

		public void ReportStatus(String action, String obj)
		{
			if (_searchStatus != null)
				_searchStatus(action, obj);
		}

		// Used when the tree has changed to make sure there is not
		// anything pointing to it
		internal void Invalidate(TreeView tree)
		{
			if (tree == _tree) {
				if (_searchInvalidate != null)
					_searchInvalidate(this);
			}
		}


		// Returns the full name of the current node
		internal String GetFullName()
		{
			StringBuilder fullName = new StringBuilder(300);

			// The array of a stack is returned in the order
			// of the pops
			Object[] nodes = _searchStack.ToArray();

			bool firstTime = true;
			for (int i = nodes.Length - 1; i >= 0; i--) {
				if (!firstTime)
					fullName.Append(Constants.PATH_SEP);
				firstTime = false;
				fullName.Append(((ISearchNode)nodes[i]).GetSearchNameString());
			}

			return fullName.ToString();
		}

		protected bool DoCompare(String str)
		{
			if (str == null)
				return false;

			//Console.WriteLine("looking at: " + str);
			switch (_compareType)
			{
			case COMPARE_FULL:
				if (str.Equals(_findWhat))
					return true;
				break;
			case COMPARE_STARTS:
				if (str.StartsWith(_findWhat))
					return true;
				break;
			case COMPARE_CONTAINS:
				if (str.IndexOf(_findWhat) != -1)
					return true;
				break;
			default:
				throw new Exception("Invalid value for compareType: "
									+ _compareType);
			}
			return false;
		}

		// Do the search
		internal void Search()
		{
			// The search can disturb the selected node, make sure
			// its restored.
			TreeNode saveNode = _startNode.TreeView.SelectedNode;
			SearchInternal(_startNode, 0);
			_startNode.TreeView.SelectedNode = saveNode;
		}

		// returns true if the search is to continue
		protected bool SearchInternal(ISearchNode node, int level)
		{
			if (level > _maxLevel)
				return true;

			// Remember the chain of nodes during the search
			_searchStack.Push(node);

			if (_nodeLooking != null) {
				// Cancel if _nodeLooking returns flase
				if (!_nodeLooking(this, node)) {
					_searchStack.Pop();
					return false;
				}
			}

			// Intermediate nodes can't match
			if (((_useName && DoCompare(node.GetSearchNameString())) ||
				 (_useValue && DoCompare(node.GetSearchValueString()))) &&
				!(node is IntermediateTreeNode))
				_nodeFound(this, node);

			bool continueSearch = true;

			// Call HasChildren because some nodes might use that
			// to prepare themselves for the search
			if (node.HasSearchChildren(this)) {
				foreach (ISearchNode childNode in node.GetSearchChildren()) {
					continueSearch = SearchInternal(childNode, level + 1);
					if (!continueSearch)
						break;
				}
			}

			if (!continueSearch) {
				TraceUtil.WriteLineInfo(null, "Search cancelled");
				_searchStack.Pop();
				return false;
			}

			_searchStack.Pop();
			return true;
		}
	}
}

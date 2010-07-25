// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.TreeNodes
{
	// Used to support the dymanic addition of children as the
	// tree is expanded
	internal class BrowserTreeNode : TreeListNode, IMenuTreeNode, ISearchNode, ISearchMaterializer
	{
		protected bool                  _childrenAdded;
		protected bool                  _isDummy;
		protected bool                  _isDragSource;
		protected bool                  _isDropTarget;
		protected bool                  _isObjectContainer;
		protected bool                  _allowDelete;
		
		// Defines a hierarchy of intermediate nodes.  For example, the
		// first node type might define an intermediate node for a base
		// class, and the second node type might be one for the member
		// type.  This node is added as a child of all of the nodes
		// in this hierarchy.
		// This defines the parent intermediate nodes and may be null
		// if intermediate nodes are not used for a particular node.
		// These should only be provided if they are to be actually used.
		// For example, if there is a threshhold value on the intermediate
		// nodes, then these should not be populated
		protected ArrayList             _intermediateNodeTypes;
		
		// True if there are actually child intermediate nodes present
		protected bool                  _hasChildIntNodes;
		
		// The order of the node types when being sorted in a tree.
		// This is used for difference member types for example
		protected int                   _nodeOrder;
		
		// This is the parent node without regard to the intermediate nodes
		// in general this should be used for most operations that require
		// the parent node.
		protected BrowserTreeNode       _logicalParent;
		
		// Similar to the above, this is the collection of child nodes
		// that bypasses the intermediate nodes.
		protected ArrayList             _logicalNodes;
		
		// Set if the number of children is greater than the threshold,
		// This is used to help determine if the intermediate node types
		// should be populated
		protected bool                  _useIntermediates;
		protected static Font           _regularFont;
		protected static Font           _boldFont;
		
		internal bool AllowDelete {
			set {
				_allowDelete = value;
			}
		}
		
		static BrowserTreeNode()
		{
			_regularFont = new Font("Tahoma", 8f, FontStyle.Regular);
			_boldFont = new Font(_regularFont, FontStyle.Bold);
		}
		
		internal BrowserTreeNode()
		{
			_allowDelete = true;
			_logicalNodes = new ArrayList();
			// Stupid - see comment in PostConstructor
			NodeFont = _regularFont;
			// Set the icon based on this object's type, if that's what
			// we are using; some subclasses set it themselves based
			// on other criteria
			SetPresInfo(this.GetType());
			// Add the column for the object value
			ColumnData.Add(null);
		}
		
		internal void SetPresInfo(Object infoObj)
		{
			SetPresInfo(PresentationMap.GetInfo(infoObj));
		}
		
		internal void SetPresInfo(PresentationInfo presInfo)
		{
			if (presInfo != null) {
				_nodeOrder = presInfo._sortOrder;
				ImageIndex = presInfo._iconIndex;
				SelectedImageIndex = ImageIndex;
			}
		}
		
		public bool IsDragSource 
		{ 
			get {
				return _isDragSource;
			}
			set {
				_isDragSource = value;
			}
		}
		
		public bool IsDropTarget { 
			get {
				return _isDropTarget;
			}
			set {
				_isDropTarget = value;
			}
		}
		
		public bool IsObjectContainer { 
			get {
				return _isObjectContainer;
			}
			set {
				_isObjectContainer = value;
			}
		}
		
		public int NodeOrder { 
			get {
				return _nodeOrder;
			}
			set {
				_nodeOrder = value;
			}
		}
		
		// Called when the constructor is finished
		protected virtual void PostConstructor()
		{
			Text = GetName();
			// Stupid - because a bold font is bigger than the TreeView
			// plain font, we need to make the default bold and then
			// unbold the ones we want plain.
			if (_isDragSource)
				NodeFont = _boldFont;
			AddDummy();
		}
		
		// Use this property to indicate that the node behaves
		// like a normal tree node with the children added by hand
		public bool ChildrenAlreadyAdded { 
			get {
				return _childrenAdded;
			}
			set {
				_childrenAdded = value;
			}
		}
		
		internal BrowserTreeNode LogicalParent {
			get {
				return _logicalParent;
			}
		}
		
		internal ArrayList LogicalNodes {
			get {
				return _logicalNodes;
			}
		}
		
		public ArrayList IntermediateNodeTypes {
			get {
				return _intermediateNodeTypes;
			}
			set {
				_intermediateNodeTypes = value;
			}
		}
		
		public bool UseIntermediates {
			get {
				return _useIntermediates;
			}
			set {
				_useIntermediates = value;
			}
		}
		
		// ISearchNode
		// Default implementation
		public virtual ISearchMaterializer GetSearchMaterializer(ISearcher searcher)
		{
			return this;
		}
		
		// Default implementation
		public virtual int GetImageIndex()
		{
			return ImageIndex;
		}
		
		// Default implementation
		public virtual String GetSearchNameString()
		{
			return GetName();
		}
		
		// Default implementation
		public virtual String GetSearchValueString()
		{
			return null;
		}
		
		// Default implementation
		public virtual ICollection GetSearchChildren()
		{
			// Use Nodes because they are appropriately sorted; 
			// _logicalNodes are not, and the search mechanism
			// skips over intermediate nodes anyway
			return Nodes;
		}
		
		// Default implementation
		public virtual bool HasSearchChildren(ISearcher searcher)
		{
			ExpandNode();
			return _logicalNodes.Count > 0;
		}
		
		public virtual void DoDrop(IDragDropItem node)
		{
			throw new Exception("Requires subclass implementation");
		}
		
		public void AddChildren()
		{
			if (_childrenAdded)
				return;
			_childrenAdded = true;
			ICollection children = GetChildren();
			//Console.WriteLine("children count: " + children.Count);
			_useIntermediates = false;
			if (((BrowserTree)TreeView).UseIntermediateNodes) {
				if (children.Count > ((BrowserTree)TreeView).IntermediateNodeThreshold)
					_useIntermediates = true;
			}
			TreeView.BeginUpdate();
			foreach (Object child in children) {
				BrowserTreeNode newNode = AllocateChildNode(child);
				AddLogicalNode(newNode);
			}
			TreeView.EndUpdate();
		}
		
		// Gets the objects to iterate over to make the child nodes
		protected virtual ICollection GetChildren()
		{
			// Default implementation, behave like a standard tree node
			return Nodes;
		}
		
		// Allocates the correct type of node
		protected virtual BrowserTreeNode AllocateChildNode(Object obj)
		{
			// not used by default
			return null;
		}
		
		// Determines is this node has children
		protected virtual bool HasChildren()
		{
			// Default implementation, behave like a standard tree node
			if (Nodes.Count == 0)
				return false;
			if (Nodes.Count == 1 &&
				((BrowserTreeNode)Nodes[0])._isDummy)
				return false;
			return true;
		}
		
		// After the new node has been added to the tree
		protected virtual void AddedToTree()
		{
		}
		
		// Adds a dummy node to this node to indicate that children
		// are present.  The actual nodes are added when the tree is
		// expanded.
		public void AddDummy()
		{
			// Dummy might already be there
			if (Nodes.Count > 0)
				return;
			if (HasChildren()) {
				BrowserTreeNode dummy = new BrowserTreeNode();
				dummy._isDummy = true;
				Nodes.Add(dummy);
			}
		}
		
		public void RemoveDummy()
		{
			// Dummy might not be there in the case of a node
			// that does not use it
			if (Nodes.Count == 0)
				return;
			BrowserTreeNode dummy = (BrowserTreeNode)Nodes[0];
			if (dummy._isDummy)
				Nodes.RemoveAt(0);
		}
		
		public virtual void SelectThisItem()
		{
			((TreeListView)TreeView).SetSelectedNode(this);
		}
		
		public virtual void Select()
		{
			BrowserTree tree = (BrowserTree)TreeView;
			try {
				tree.TreeMenuHelper.SetupActionMenu(this, tree);
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning
					(this, "Exception in menu processing: " + ex);
				tree.TreeMenuHelper.DisableActionMenu();
			}
			try {
				SetupParamPanel();
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(this, "Exception in SetupParamPanel " + ex);
			}
			DoSelectInvoke();
			GetDetailText();
		}
		
		public virtual void DoSelectInvoke()
		{
			// overridden
		}
		
		public virtual void IntermediateChildSelect()
		{
			// overridden
		}
		
		public virtual void SetupParamPanel()
		{
			ObjectBrowser.ParamPanel.Clear();
		}
		
		// Really does not do anything with this node, just gets
		// rid of all of the children
		public virtual void InvalidateNode()
		{
			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("Invalidating: " + Text);
			((BrowserTree)TreeView).Invalidating = true;
			TreeView.BeginUpdate();
			// Tell any search that the results should be discarded
			BrowserFinder bf = BrowserFinder.BFinder; 
			if (bf != null)
				bf.Invalidate(TreeView);
			try {
				// Work around a bug in clear processing
				Nodes.Clear();
				LogicalNodes.Clear();
			} catch {
				Nodes.Clear();
				LogicalNodes.Clear();
			}
			_childrenAdded = false;
			_hasChildIntNodes = false;
			if (IsExpanded)
				AddChildren();
			else
				AddDummy();
			TreeView.EndUpdate();
			((BrowserTree)TreeView).Invalidating = false;
		}
		protected void AddChildToIntermediate(BrowserTreeNode childNode,
											 IntermediateNodeType intNodeType,
											 int intIndex)
		{
			// Look for the intermediate node with the right type
			bool found = false;
			foreach (BrowserTreeNode intNode in Nodes) {
				if (intNode is IntermediateTreeNode && ((IntermediateTreeNode)intNode).NodeType ==
					intNodeType) {
					intNode.AddLogicalNode(childNode, ++intIndex);
					found = true;
					break;
				}
			}
			// Add a new intermediate node
			if (!found) {
				IntermediateTreeNode intNode = new IntermediateTreeNode(intNodeType, this);
				intNode._useIntermediates = _useIntermediates;
				intNode.SetPresInfo(intNodeType.PresentationInfo);
				((TreeListView)TreeView).Add(Nodes, intNode);
				intNode.AddLogicalNode(childNode, ++intIndex);
				_hasChildIntNodes = true;
			}
		}
		internal delegate void AddLogicalInvoker(IBrowserNode node);
		
		public void AddLogicalNode(IBrowserNode child)
		{
			AddLogicalNode(child, 0);
		}
		
		// intIndex is the index into the intermediate node types
		// to consider user for adding this node
		protected void AddLogicalNode(IBrowserNode child, int intIndex)
		{
			BrowserTreeNode childNode = (BrowserTreeNode)child;
			// See if we have an intermediate node type
			if (childNode._intermediateNodeTypes != null &&
				intIndex < childNode._intermediateNodeTypes.Count) {
				AddChildToIntermediate(childNode,
									   (IntermediateNodeType)childNode.
									  _intermediateNodeTypes[intIndex],
									  intIndex);
			} else {
				((TreeListView)TreeView).Add(Nodes, childNode);
			}
			
			// Update the logical tree only when the child is 
			// added to the first level intermediate node
			if (intIndex == 0) {
				childNode._logicalParent = this;
				_logicalNodes.Add(childNode);
				childNode.AddedToTree();
			}
		}
		
		// Invalidate all of the nodes under this node (and this node)
		public void InvalidateAll()
		{
			InvalidateNode();
			foreach (TreeNode node in Nodes) {
				if (node is IBrowserNode)
					((IBrowserNode)node).InvalidateAll();
			}
		}
		// Return true to cancel the expansion
		public virtual bool ExpandNode()
		{
			RemoveDummy();
			try {
				AddChildren();
			} catch (Exception ex) {
				// Should never hit this, but sometimes things go wrong
				// and this way, the user can get the stack trace
				ErrorDialog.Show(ex,
								"(bug, please report this)Error Expanding "
								+ this,
								"Error Expanding " + this,
								MessageBoxIcon.Error);
			}
			return false;
		}
		
		public void PointToNode()
		{
			// This is the node to point to, might change, see comments
			// below.
			BrowserTreeNode node = this;
			TreeListView tree = (TreeListView)TreeView;
			if (!IsVisible) {
				// Ensure visible may cause a node above us to  
				// be Invalidated() and therefore leaving this
				// node as an orphan.  Should this happen, we need to
				// Find the correct node.
				String savePath = FullPath;
				EnsureVisible();
				// We got invalidated, refind the right node by name
				if (TreeView == null) {
					node = (BrowserTreeNode)tree.FindNodeByFullPath(savePath);
					// Should not happen
					if (node == null) {
						ErrorDialog.Show("(bug) node " + savePath
										+ " not found in PointToNode",
										"Error Finding Node",
										MessageBoxIcon.Error);
						return;
						
					}
				}
			}
			// If the node was previously selected, we need to 
			// force the selection processing again because focus
			// may have gone elsewhere requiring the detail panel to
			// get reset.  Need a better way to handle this.
			if (tree.SelectedNode == node) {
				DetailPanel.Clear();
				node.Select();
			}
			tree.SetSelectedNode(node);
			tree.Focus();
		}
		
		protected void RemoveHack()
		{
			try {
				Remove();
			} catch {
				// Work around MS bug in remove 
			}
		}
		
		// Removes this node from the logical nodes
		public virtual void RemoveLogicalNode()
		{
			DetailPanel.Clear();
			// The parent might no longer contain this node because it
			// might have been invalidated
			if (Parent == null) {
				if (TreeView != null && TreeView.Nodes.Contains(this))
					RemoveHack();
				_logicalParent = null;
				return;
			}
			if (_logicalParent == Parent && Parent.Nodes.Contains(this)) {
				_logicalParent._logicalNodes.Remove(this);
				_logicalParent = null;
				RemoveHack();
				return;
			}
			if (_logicalParent != Parent) {
				throw new Exception("Removal in the case of intermediate nodes not yet supported");
			}
		}
		
		public virtual bool HasGetProp()            { return false; }
		public virtual bool HasSetProp()            { return false; }
		public virtual bool HasSetField()           { return false; }
		public virtual bool HasInvokeMeth()         { return false; }
		public virtual bool HasCreateObj()          { return false; }
		public virtual bool HasCast()               { return false; }
		public virtual bool HasCut()                { return false; }
		public virtual bool HasCopy()               { return false; }
		public virtual bool HasCopyText0()          { return true; }
		public virtual bool HasCopyText1()          { return false; }
		public virtual bool HasPaste()              { return false; }
		public virtual bool HasDesignSurface()      { return false; }
		public virtual bool HasEventLogging()       { return false; }
		public virtual bool HasDelete()             { return false; }
		public virtual bool HasRename()             { return false; }
		public virtual bool HasClose()              { return false; }
		public virtual bool HasDocumentation()      { return false; }
		public virtual bool HasConvert()            { return false; }
		public virtual bool HasRegister()           { return false; }
		public virtual bool HasUnregister()         { return false; }
		public virtual bool HasRemoveFavorite()     { return false; }
		
		public virtual String GetName()
		{
			return Text;
		}
		
		public virtual bool Paste(IBrowserNode node, bool isCopy)
		{
			throw new Exception("Paste not implemented");
		}
		
		public virtual void ShowTitle()
		{
			int removeCount = ((BrowserTree)TreeView)._fullPathRemoveCount;
			String fullPath = FullPath;
			if (removeCount > 0 && 
				removeCount < fullPath.Length &&
				fullPath.StartsWith(((BrowserTree)TreeView).
									_fullPathRemoveString))
			{
				StatusPanel.StatusText = 
					fullPath.Substring(((BrowserTree)TreeView).
									  _fullPathRemoveCount);
			}
			else
			{
				StatusPanel.StatusText = fullPath;
			}
		}
		public virtual void GetDetailText()
		{
			DetailPanel.Add("Tree Node Type", 
							ObjectBrowser.INTERNAL,
							300,
							GetType().ToString());
			ShowTitle();
		}
		protected virtual int OrderCompareTo(BrowserTreeNode other)
		{
			if (_nodeOrder > other._nodeOrder)
				return 1;
			if (_nodeOrder < other._nodeOrder)
				return -1;
			return 0;
		}
		public override int CompareTo(Object other)
		{
			if (other is BrowserTreeNode)
			{
				int orderCompare = OrderCompareTo((BrowserTreeNode)other);
				if (orderCompare != 0)
					return orderCompare;
				return 
					GetName().CompareTo(((BrowserTreeNode)other).GetName());
			}
			return -1;
		}
		public override String ToString()
		{
			return FullPath;
		}
	}
}

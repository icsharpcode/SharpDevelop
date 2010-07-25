// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.ObjBrowser.GuiDesigner;
using NoGoop.ObjBrowser.Panels;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	public class BrowserTree : TreeListView, IDragDropSourceTarget, IDragDropItem
	{
		protected ContextMenuStrip      _treeMenu;
		// Stupid compiler
		internal ActionMenuHelper       _treeMenuHelper;
		protected ListView              _partnerList;
		protected bool                  _isDragSource;
		protected bool                  _isDropTarget;
		protected bool                  _isObjectContainer;
		protected TreeNode              _expandingNode;
		
		// Used during cut/copy/paste
		internal IBrowserNode           _cutCopyNode;
		protected bool                  _isCopy;
		// Some node it being invalidated, ignore selection events
		protected bool                  _invalidating;
		// Number of child nodes this node must have in order to get
		// intermediate nodes.
		protected int                   _intermediateNodeThreshold;
		// The above is enabled only if this is true
		protected bool                  _useIntermediateNodes;
		internal int                    _fullPathRemoveCount;
		internal String                 _fullPathRemoveString;
		
		static Type[] dragSourceTypes = new Type[] { typeof(TypeTreeNode), 
			typeof(MemberTreeNode),
			typeof(AssemblyTreeNode),
			typeof(ComTypeTreeNode)};
		
		public BrowserTree()
		{
			InitializeComponent();
			PathSeparator = Constants.PATH_SEP;
		}
		
		public BrowserTree(string omitTopName) : this()
		{
			_fullPathRemoveString = omitTopName;
			_fullPathRemoveCount = omitTopName.Length + PathSeparator.Length;
		}
		
		internal ActionMenuHelper TreeMenuHelper {
			get {
				return _treeMenuHelper;
			}
		}
		
		internal bool Invalidating {
			get {
				return _invalidating;
			}
			set {
				_invalidating = value;
			}
		}
		
		public bool IsDragSource { 
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
		
		public bool IsObjectContainer 
		{ 
			get {
				return _isObjectContainer;
			}
			set {
				_isObjectContainer = value;
			}
		}
		
		public static Type[] DragSourceTypes {
			get {
				return dragSourceTypes;
			}
		}
		
		Type[] IDragDropSourceTarget.DragSourceTypes {
			get {
				return DragSourceTypes;
			}
		}
		
		internal IBrowserNode CutCopyNode { 
			get {
				return _cutCopyNode;
			}
		}
		
		internal int IntermediateNodeThreshold {
			get {
				return _intermediateNodeThreshold;
			}
			set {
				_intermediateNodeThreshold = value;
			}
		}
		
		internal bool UseIntermediateNodes {
			get {
				return _useIntermediateNodes;
			}
			set {
				_useIntermediateNodes = value;
			}
		}
		
		void InitializeComponent()
		{
			_treeMenu = new ContextMenuStrip();
			_treeMenuHelper = new ActionMenuHelper();
			_treeMenuHelper.BuildContextMenu(_treeMenu);
			ContextMenuStrip = _treeMenu;
			HideSelection = false;
			// Sort the nodes using their compare to function
			UseCompareTo = true;
			DragDropSupport ddSupport = new DragDropSupport(this);
			ImageList = PresentationMap._icons;
			_columns = new ColumnHeaderCollection();
		}
		
		public IDragDropItem GetItemAt(Point point)
		{
			TreeNode node = GetNodeAt(point);
			if (node != null)
				return (IDragDropItem)node;
			return this;
		}
		
		// Here the tree is functioning like a tree node
		// for drag/drop.  This is used from a DragDropPanel
		// where something is dropped on the design surface, 
		// but this tree is delegated to handling it.
		// We give it to the top-level node.
		public virtual void DoDrop(IDragDropItem node)
		{
			if (_isObjectContainer) {
				IDragDropItem targetNode = ObjectBrowser.GetTopLevelObjectNode();
				targetNode.DoDrop(node);
			}
		}
		
		public virtual void SelectThisItem()
		{
		}		
		
		// Invalidate all of the nodes in the tree
		internal void InvalidateAll()
		{
			foreach (TreeNode node in Nodes) {
				if (node is IBrowserNode)
					((IBrowserNode)node).InvalidateAll();
			}
		}
		
		protected void TreeNodePopupClickCommon(object sender, EventArgs e, bool setMember)
		{
			IInvokableTreeNode node = (IInvokableTreeNode)SelectedNode;
			if (node != null) {
				// Attempt in invoke the specified method
				DetailPanel.Clear();
				node.Invoke(setMember, false, !Constants.IGNORE_EXCEPTION);
				node.GetDetailText();
			}
		}
		
		internal void TreeNodePopupClick(object sender, EventArgs e)
		{
			TreeNodePopupClickCommon(sender, e, false);
		}
		
		internal void TreeNodePopupClickSet(object sender, EventArgs e)
		{
			TreeNodePopupClickCommon(sender, e, true);
		}
		
		internal void TreeNodePopupCreateObj(object sender, EventArgs e)
		{
			ObjectCreator.CreateObject((IDragDropItem)SelectedNode, ObjectBrowser.GetTopLevelObjectNode());
		}
		
		internal void TreeNodePopupCut(object sender, EventArgs e)
		{
			_cutCopyNode = (IBrowserNode)SelectedNode;
			_isCopy = false;
		}
		
		internal void TreeNodePopupCopy(object sender, EventArgs e)
		{
			_cutCopyNode = (IBrowserNode)SelectedNode;
			_isCopy = true;
		}
		
		internal void TreeNodePopupCopyText0(object sender, EventArgs e)
		{
			Object obj = ((TreeListNode)SelectedNode).Text;
			if (obj == null)
				obj = String.Empty;
			Clipboard.SetDataObject(obj);
		}
		
		internal void TreeNodePopupCopyText1(object sender, EventArgs e)
		{
			Object obj = ((TreeListNode)SelectedNode).ColumnData[0];
			if (obj == null)
				obj = String.Empty;
			Clipboard.SetDataObject(obj);
		}
		
		internal void TreeNodePopupPaste(object sender, EventArgs e)
		{
			((IBrowserNode)SelectedNode).Paste(_cutCopyNode, _isCopy);
			if (!_isCopy)
				_cutCopyNode = null;
		}
		
		internal void TreeNodePopupDelete(object sender, EventArgs e)
		{
			IBrowserNode node = (IBrowserNode)SelectedNode;
			if (node != null)
				node.RemoveLogicalNode();
		}
		
		internal void TreeNodePopupRename(object sender, EventArgs e)
		{
			IInvokableTreeNode node = (IInvokableTreeNode)SelectedNode;
			throw new Exception("Implement this");
		}
		
		internal void ConvertClick(object sender, EventArgs e)
		{
			IConvertableTreeNode node = (IConvertableTreeNode)SelectedNode;
			if (node != null)
				node.DoConvert();
		}
		
		internal void RegisterClick(object sender, EventArgs e)
		{
			IConvertableTreeNode node = (IConvertableTreeNode)SelectedNode;
			if (node != null)
				node.DoRegister();
		}
		
		internal void UnregisterClick(object sender, EventArgs e)
		{
			IConvertableTreeNode node = (IConvertableTreeNode)SelectedNode;
			if (node != null)
				node.DoUnregister();
		}
		
		internal void RemoveFavoriteClick(object sender, EventArgs e)
		{
			IFavoriteTreeNode node = (IFavoriteTreeNode)SelectedNode;
			if (node != null)
				node.DoRemoveFavorite();
		}
		
		internal void CastClick(object sender, EventArgs e)
		{
			ICastableTreeNode node = (ICastableTreeNode)SelectedNode;
			if (node != null)
				node.DoCast();
		}
		
		internal delegate void AddNodeInvoker(TreeListNode node);
		
		internal virtual void AddNode(TreeListNode node)
		{
			Add(Nodes, node);
		}
		
		// Called before the menu is popped up
		protected void TreeNodePopup(object sender, EventArgs e)
		{
			// Do the select processing before we show the menu, 
			// this is necessary if the user right clicks on a node
			// before selecting it
			SetSelectedNode(GetNodeAt(PointToClient(MousePosition)));
		}
		
		// When the control selection changes, select object tree
		// node corresponding to the first control selected
		internal void ControlSelectionChanged(object sender, EventArgs e)
		{
			DesignerHost host = DesignerHost.Host;
			// FIXME
			// Don't do anything here because the pointers from the
			// nodes stored in the site might not be right.  Need to fix
			// this so the site does not contain node pointers because
			// they are subject to removal from the tree upon invalidation.
			/***
			Control con = (Control)host.PrimarySelection;
			if (con != null)
			{
				if (con.Site is DesignerSite)
				{
					DesignerSite site = (DesignerSite)con.Site;
					if (site.TargetNode != null)
					{
						SetSelectedNode(site.TargetNode);
					}
				}
			}
			****/
		}
		
		// Should be internal, but compiler gives warning
		public virtual void DoTabSelected()
		{
			if (SelectedNode is IBrowserNode)
				((IBrowserNode)SelectedNode).Select();
		}
		
		internal override void VScrolled()
		{
			((IBrowserNode)GetNodeAt(0, 0)).ShowTitle();
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			// Process the subclass first
			base.OnMouseDown(e);
			// Use the mouse down event because we want to make sure everything
			// is setup in the case of a drag.  With a drag, the Enter
			// or GetFocus events do not occur, you only see the mouse
			// down event.
			// If we get focus on the node that was previously
			// selected, we need to make sure we re-select the node
			// because this can happen when another tree has been
			// selected and other parts of the UI have been altered.
			// But we must only reselect the node if the mouse is
			// actually pointing to the node to avoid the situation
			// where we are processing the selection for the SelectedNode
			// when the point of the MouseDown was to move the selection.
			if (SelectedNode != null &&
				SelectedNode == GetNodeAt(PointToClient(MousePosition))) {
				DetailPanel.Clear();
				if (SelectedNode is IBrowserNode) {
					// Selection could take a while
					Cursor save = Cursor.Current;
					Cursor.Current = Cursors.WaitCursor;
					((IBrowserNode)SelectedNode).Select();
					Cursor.Current = save;
				}
			}
		}
		
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			if (e.Node is IBrowserNode) {
				try {
					IBrowserNode node = (IBrowserNode)e.Node;
					// Expansion could take a while
					Cursor save = Cursor.Current;
					Cursor.Current = Cursors.WaitCursor;
					// Allow the expansion to be cancelled
					// Do select processing on expansion as well
					//BeginUpdate();
					if (node.ExpandNode())
						e.Cancel = true;
					//EndUpdate();
					Cursor.Current = save;
				} catch (Exception ex) {
					ErrorDialog.Show(ex,
									"(this is a bug, please report) "
									+ "Exception on node expand",
									MessageBoxIcon.Error);
				}
			}
			base.OnBeforeExpand(e);
		}
		
		protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			bool canSelect = CanSelectNode();
			// Only treat select events if valid to do so
			if (canSelect && !e.Cancel && !_invalidating) {
				try {
					//Console.WriteLine("Select: " + e.Node);
					//Console.WriteLine(new StackTrace());
					DetailPanel.Clear();
					if (e.Node is IBrowserNode) {
						// Selection can sometimes take a while
						Cursor save = Cursor.Current;
						Cursor.Current = Cursors.WaitCursor;
						((IBrowserNode)e.Node).Select();
						Cursor.Current = save;
					}
				} catch (Exception ex) {
					ErrorDialog.Show(ex,
									"(this is a bug, please report) "
									+ "Exception on node select",
									MessageBoxIcon.Error);
				}
			}
			if (!canSelect)
				e.Cancel = true;
			base.OnBeforeSelect(e);
		}
	}
}

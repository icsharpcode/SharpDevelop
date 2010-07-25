// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.Obj;
using NoGoop.ObjBrowser.Dialogs;
using NoGoop.ObjBrowser.GuiDesigner;
using NoGoop.ObjBrowser.LinkHelpers;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.TreeNodes
{
	// Represents an object in a tree which is not a member of
	// a type
	internal class ObjectTreeNode : BrowserTreeNode, IMenuTreeNode, IEventLoggingMenuNode, ICastableTreeNode, IObjectNode
	{
		// Set if a cast is applied to this node
		protected CastInfo              _castInfo;
		
		// The object that is the result of the cast.  This cannot
		// be kept in ObjectInfo because that is global for an object,
		// and a cast applies only to an object in a particular
		// context (ObjectTreeNode).  Therefore, if the _castInfo
		// pointer is set, we have to use that information before
		// considering the ObjectInfo's version.
		protected Object                _castObject;
		protected ObjectInfo            _objInfo;
		protected bool                  _comNode;
		
		protected ITypeTreeHandler      _typeHandler;
		public virtual Object[] CurrentPropIndexValues {
			get {
				// Not used here, subclassed
				return null;
			}
			set {
				// Not used here, subclassed
			}
		}
		
		internal bool IsComNode {
			get {
				return _comNode;
			}
		}
		
		public ObjectInfo ObjectInfo {
			get {
				return _objInfo;
			}
		}
		// Returns the type of an object associated with this node,
		// if any
		public Type ObjType {
			get {
				if (_castInfo != null)
					return _castInfo.CastType;
				return ObjectInfo.ObjType;
			}
		}
		
		// Returns the value of the object associated with this node
		public Object Obj {
			get {
				if (_castObject != null)
					return _castObject;
				return ObjectInfo.Obj;
			}
		}
		
		public virtual bool EventLogging {
			get {
				int eventCount = ObjType.GetEvents(ReflectionHelper.ALL_BINDINGS).Length;
				if (eventCount > 0) {
					return eventCount == ObjectBrowser.EventLog.LoggingCount(Obj);
				}
				return false;
			}
		}
		
		public ObjectInfo ParentObjectInfo  {
			get {
				if (_logicalParent is IObjectNode)
					return ((ObjectTreeNode)_logicalParent).ObjectInfo;
				return null;
			}
		}
		
		public IObjectNode ParentObjectNode  {
			get {
				if (_logicalParent is IObjectNode)
					return (IObjectNode)_logicalParent;
				return null;
			}
		}
		
		internal ObjectTreeNode(bool comNode) : base()
		{
			_comNode = comNode;
		}
		internal ObjectTreeNode(bool comNode, ObjectInfo objInfo) : this(comNode)
		{
			_objInfo = objInfo;
			_isObjectContainer = 
				typeof(IList).IsAssignableFrom(ObjType);
			_isDropTarget = _isObjectContainer;
			SetTypeHandler();
			DoDisplayValue();
			PostConstructor();
		}
		
		public override String GetSearchValueString()
		{
			return (String)ColumnData[0];
		}
		
		// Override to not expand
		public override bool HasSearchChildren(ISearcher searcher)
		{
			return _logicalNodes.Count > 0;
		}
		
		public void DoDisplayValue()
		{
			ColumnData[0] = _objInfo.GetStringValue();
			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("DisplayVal: " + ColumnData[0]);
		}
		
		protected virtual void ObjectValueChanged()
		{
			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("ObjectValueChanged: " + Obj);
			TreeView.Invalidate();
			InvalidateNode();
		}
		
		// Searches the entire object tree for objects that 
		// actually exist with the given type; does not cause
		// anything to be expanded.
		internal static ArrayList GetNodesOfType(Type type)
		{
			ArrayList nodes = new ArrayList();
			SearchFor(type, null, ObjectBrowser.ObjTree.Nodes, nodes);
			return nodes;
		}
		internal const bool CREATE_OBJ = true;
		
		// Find the specified object
		internal static ObjectTreeNode FindObject(Object obj, bool createObj)
		{
			ArrayList nodes = new ArrayList();
			SearchFor(null, obj, ObjectBrowser.ObjTree.Nodes, nodes);
			if (nodes.Count > 0)
				return (ObjectTreeNode)nodes[0];
			else
			{
				if (createObj)
				{
					if (TraceUtil.If(null, TraceLevel.Verbose))
						Trace.WriteLine("FindObj - not found - creating");
					ObjectTreeNode tlNode = 
						ObjectBrowser.GetTopLevelObjectNode();
					tlNode.AddObject(obj);
					ObjectTreeNode node = FindObject(obj, false);
					if (TraceUtil.If(null, TraceLevel.Verbose))
					{
						Trace.WriteLine("FindObj - found created node: " 
										+ node);
					}
					return node;
				}
				return null;
			}
		}
		// Finds the specified member within this object
		internal ObjectTypeTreeNode FindMember(MemberInfo member)
		{
			Type memberInfoType = member.GetType();
			// Make sure we have the members populated
			ExpandNode();
			foreach (Object node in LogicalNodes)
			{
				if (node is ObjectTypeTreeNode)
				{
					ObjectInfo objInfo = ((ObjectTypeTreeNode)node).ObjectInfo;
					if (objInfo != null)
					{
						MemberInfo mi = objInfo.ObjMemberInfo;
						if (!mi.GetType().Equals(memberInfoType))
							continue;
						if (objInfo.ObjMemberInfo.ToString().
							Equals(member.ToString()))
							return (ObjectTypeTreeNode)node;
					}
				}
			}
			return null;
		}
		// Finds the node containing the specified object
		// returns true if it found the object
		internal static bool SelectObject(Object obj, 
										 bool createObj)
		{
			ObjectTreeNode node = FindObject(obj, createObj);
			if (node != null)
			{
				node.PointToNode();
				return true;
			}
			return false;
		}
		// Finds the node containing the specified object
		// returns true if it found the object
		internal static bool SelectObjectMember(IObjectMember om,
												bool createObj)
		{
			ObjectTreeNode node = FindObject(om.Obj, createObj);
			if (node != null)
			{
				node = node.FindMember(om.Member);
				if (node == null)
				{
					ErrorDialog.Show("You requested to show member "
									+ om.Member
									+ " but it cannot be shown because "
									+ "it is not visible according to the "
									+ "preferences you have selected "
									+ "in the object tree.  To show this "
									+ "member, please modify those "
									+ "preferences.",
									"Member not visible",
									MessageBoxIcon.Error);
					return false;
				}
				node.PointToNode();
				return true;
			}
			return false;
		}
		// Search for either an object or a type
		protected static void SearchFor(Type type,
										Object lookObj,
										ICollection nodesToSearch, 
										ArrayList outputNodes)
		{
			foreach (TreeNode treeNode in nodesToSearch)
			{
				if (treeNode is ObjectTreeNode)
				{
					ObjectTreeNode node = (ObjectTreeNode)treeNode;
					Object obj = node.Obj;
					if (type != null)
					{
						// We don't want to consider value types
						if (obj != null &&
							!typeof(ValueType).IsAssignableFrom(obj.GetType()) &&
							type.IsAssignableFrom(obj.GetType()))
						{
							outputNodes.Add(node);
						}
					}
					else
					{
						if (obj == lookObj)
						{
							if (TraceUtil.If(null, TraceLevel.Verbose))
							{
								Trace.WriteLine("SearchFor obj found: " 
												+ node);
							}
							// There should only be one node that matches
							outputNodes.Add(node);
							break;
						}
					}
					SearchFor(type, lookObj, node.LogicalNodes, 
							 outputNodes);
				}
				// The ones that are not ObjectTreeNode are the dummy nodes
			}
		}
		protected void SetTypeHandler()
		{
			_typeHandler = TypeHandlerManager.Instance.GetTypeHandler(ObjType, this);
		}
		// Gets the objects to iterate over to make the child nodes
		protected override ICollection GetChildren()
		{
			if (_typeHandler != null && _typeHandler.Enabled)
				return _typeHandler.GetChildren();
			bool showFields = ComponentInspectorProperties.ShowFields;
			bool showProps = ComponentInspectorProperties.ShowProperties;
			bool showMethods = ComponentInspectorProperties.ShowMethods;
			bool showEvents = ComponentInspectorProperties.ShowEvents;
			bool showPublicOnly = ComponentInspectorProperties.ShowPublicMembersOnly;
			bool showBase = ComponentInspectorProperties.ShowBaseClasses;
			bool showPropMeth = ComponentInspectorProperties.ShowPropertyAccessorMethods;
			Type objType = ObjType;
			bool comObject = objType.IsCOMObject;
			ArrayList members = ClassCache.GetMembers(objType);
			ArrayList membersOut = new ArrayList();
			foreach (MemberInfo member in members) {
				// Get rid of these (types are inner classes)
				if (member is ConstructorInfo ||
					member is Type)
					continue;
				if (member is FieldInfo && !showFields)
					continue;
				if (member is PropertyInfo && !showProps)
					continue;
				if (member is MethodInfo && !showMethods)
					continue;
				if (member is EventInfo && !showEvents)
					continue;
				if (showPublicOnly) {
					// Sigh, seems that the property visiblity is not shown in
					// reflection and the visbility is in each type of member
					// rather than the superclass
					if ((member is MethodInfo && !((MethodInfo)member).IsPublic) ||
						(member is FieldInfo && !((FieldInfo)member).IsPublic))
						continue;
				}
				if (!showBase) {
					if (!objType.Equals(member.DeclaringType))
						continue;
				}
				// Ignore property accessors/event adders if specified
				if (!showPropMeth && member.MemberType == MemberTypes.Method) {
					if (member.Name.StartsWith("get_") ||
						member.Name.StartsWith("set_") ||
						member.Name.StartsWith("add_") ||
						member.Name.StartsWith("remove_")) {
						continue;
					}
					
					if (comObject) {
						if (member.Name.IndexOf("_Event_add_") != -1 ||
							member.Name.IndexOf("_Event_remove_") != -1 ||
							member.Name.IndexOf("_get_") != -1 ||
							member.Name.IndexOf("_set_") != -1) {
							continue;
						}
					}
				}
				membersOut.Add(member);
			}
			return membersOut;
		}
		
		// Allocates the correct type of node
		protected override BrowserTreeNode AllocateChildNode(Object obj)
		{
			if (_typeHandler != null && _typeHandler.Enabled)
				return _typeHandler.AllocateChildNode((ObjectInfo)obj);
			MemberInfo member = (MemberInfo)obj;
			ObjectInfo objInfo = ObjectInfoFactory.GetObjectInfo(_comNode, member, ObjType);
			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("AllocateChild (member): " + member);
			ObjectTypeTreeNode  newNode = new ObjectTypeTreeNode(_comNode,
				objInfo, member, _useIntermediates);
			return newNode;
		}
		
		public override void Select()
		{
			// Recalculate this if a type handler takes care of it,
			// because the children are not types, they are typically 
			// objects
			try {
				if (_typeHandler != null && !_typeHandler.IsCurrent())
					InvalidateNode();
			} catch (Exception ex) {
				ErrorDialog.Show(ex, 
								"Exception in getting object list using "
								+ _typeHandler.Info.Name
								+ " type handler",
								"Exception in Getting Object List",
								MessageBoxIcon.Error);
				// Continue with what we have
			}
			// Reevaluate data, might have changed
			DoDisplayValue();
			base.Select();
		}
		
		public override void IntermediateChildSelect()
		{
			GetDetailTextInt(!ObjectInfo.SHOW_MEMBER);
		}
		
		// Determines if this node has children
		protected override bool HasChildren()
		{
			if (_typeHandler != null && _typeHandler.Enabled)
				return _typeHandler.HasChildren();
			return ReflectionHelper.DoesTypeHaveKids(ObjType);
		}
		
		public override void RemoveLogicalNode()
		{
			// Is the parent is a list, remove this node from the list
			if (LogicalParent is ObjectTreeNode) {
				ObjectTreeNode parent = (ObjectTreeNode)LogicalParent;
				ObjectInfo parentObjInfo = parent.ObjectInfo;
				// Tell the parent not to remove this node, only the object
				// since we are removing ourself
				if (typeof(IList).IsAssignableFrom(parentObjInfo.ObjType))
					parent.RemoveObject(Obj, SKIP_REMOVE_NODE);
			}
			// Is this is a control, remove it from the design
			// surface
			if (Obj != null &&
				Obj is Control)
				ObjectBrowser.ImagePanel.RemoveControl((Control)Obj);
			// Turn off any event logging
			SetEventLogging(false, !SHOW_PROBLEMS);
			base.RemoveLogicalNode();
		}
		
		public override bool HasCopy()
		{
			// Makes no sense to copy top-level nodes
			if (Parent != null)
				return true;
			return false;
		}
		
		public override bool HasCopyText1()
		{
			return true;
		}
		
		public override bool HasCut()
		{
			return HasCopy() && _allowDelete;
		}
		
		public override bool HasPaste()
		{
			// Only if we have cut/copied something
			if (((BrowserTree)TreeView).CutCopyNode != null) {
				// Can paste into lists
				if (typeof(IList).IsAssignableFrom(ObjType))
					return true;
			}
			return false;
		}
		
		public override bool HasDelete()
		{
			return _allowDelete;
		}
		
		public override bool HasRename()
		{
			// Makes no sense to rename top-level nodes
			if (Parent != null)
				return true;
			return false;
		}
		
		public override bool HasCast()
		{
			// Makes no sense to cast top-level nodes
			if (Parent != null)
				return true;
			return false;
		}
		
		public void DoCast()
		{
			// There will be no member info if this is an object
			// tree node (not an object type tree node)
			CastDialog cd = new CastDialog(_castInfo, ObjectInfo.ObjMemberInfo, _objInfo.Obj);
			// Returns true if it worked
			if (!cd.DoShowDialog())
				return;
			_castInfo = cd.CastInfo;
			// Reapply the cast if it exists, since the object is
			// refreshed whenever the tree
			if (_castInfo != null && _objInfo.Obj != null) {
				_castObject = _castInfo.DoCast(_objInfo.Obj);
			}
			ObjectValueChanged();
			// Reset the detail panel if the cast information changed
			DetailPanel.Clear();
			GetDetailText();
		}
		
		public override bool HasEventLogging()
		{
			// Any object has event logging that controls all of
			// the events for the object, except top level objects
			if (Parent != null)
				return true;
			return false;
		}
		
		public virtual void EventLoggingClick(object sender, EventArgs e)
		{
			bool newState = !EventLogging;
			SetEventLogging(newState, SHOW_PROBLEMS);
		}
		
		protected const bool SHOW_PROBLEMS = true;
		
		protected void SetEventLogging(bool newState, bool showProblems)
		{
			StringBuilder problems = new StringBuilder();
			// Do for all events regardless of what's displayed in the
			// tree
			foreach (EventInfo ei in ObjType.GetEvents(ReflectionHelper.ALL_BINDINGS)) {
				try {
					EventLogList.ObjectEvent oe = new EventLogList.ObjectEvent(Obj, ei);
					ObjectBrowser.EventLog.ToggleLogging(oe, _objInfo.GetName(), newState);
				} catch (Exception ex) {
					// The outer exception may be a wrapper
					Exception showException = ex.InnerException;
					if (showException == null)
						showException = ex;
					problems.Append("\n\n" + ei.Name + " - ");
					problems.Append(showException.Message);
					problems.Append(" (");
					problems.Append(showException.GetType().FullName);
					problems.Append(")");
				}
			}
			if (showProblems && problems.Length != 0) {
				ErrorDialog.Show
					("The following events were not enabled for "
					+ "logging due to errors:"
					+ problems.ToString(),
					"Some Events Were not Enabled",
					MessageBoxIcon.Error);
			}
		}
		// Create an object and drop it into the list
		public override void DoDrop(IDragDropItem node)
		{
			try {
				if (_isObjectContainer) {
					ObjectCreator.CreateObject(node, this);
				}
			} catch (Exception ex) {
				ErrorDialog.Show(ex, 
								"Exception on Drop",
								MessageBoxIcon.Error);
			}
		}
		internal delegate bool AddObjectInvoker(ObjectInfo newObjInfo);
		
		// Adds an object to this node, might or might not be a new object
		internal bool AddObject(ObjectInfo newObjInfo)
		{
			IList list = (IList)Obj;
			try {
				list.Add(newObjInfo.Obj);
			} catch (Exception ex) {
				ErrorDialog.Show(ex,
								"Exception adding object to list",
								MessageBoxIcon.Error);
				return false;
			}
			InvalidateNode();
			return true;
		}
		// Adds an object to this node, might or might not be a new object
		internal ObjectInfo AddObject(Object obj)
		{
			ObjectInfo newObjInfo = ObjectInfoFactory.GetObjectInfo(IsComNode, obj);
			if (AddObject(newObjInfo))
				return newObjInfo;
			return null;
		}
			
		// Adds the newly created object to this node
		internal ObjectInfo AddNewObject(Object obj, IDragDropItem sourceNode)
		{
			// Returns null if it did not work
			ObjectInfo objInfo = AddObject(obj);
			if (objInfo == null)
				return null;
			// Expand this and select the node that
			// contains the object we just created
			Expand();
			foreach (TreeNode n in LogicalNodes) {
				if (n is ObjectTreeNode) {
					ObjectTreeNode objTreeNode = 
						(ObjectTreeNode)n;
					if (objTreeNode.Obj == obj)
						objTreeNode.PointToNode();
				}
			}
			// Point the control's site to this so that we can track
			// the selection of controls in the object tree
			// FIXME - this does not work since this node can be removed
			// from the tree, nothing should refer to these tree nodes
			if (obj is Control) {
				Control control = (Control)obj;
				if (control.Site != null) {
					((DesignerSite)control.Site).TargetNode = (ObjectTreeNode)TreeView.SelectedNode;
				}
			}
			// If we are giving focus to a control not on the
			// design surface (see create object above), don't
			// give focus to the newly created tree node
			if (sourceNode == null ||
				!(sourceNode is TypeTreeNode &&
				 obj is Control &&
				  !((TypeTreeNode)sourceNode).OnDesignSurface))
				TreeView.Focus();
			return objInfo;
		}
		
		protected const bool SKIP_REMOVE_NODE = true;
		
		internal void RemoveObject(Object obj)
		{
			RemoveObject(obj, !SKIP_REMOVE_NODE);
		}
		
		internal void RemoveObject(Object obj, bool skipRemoveNode)
		{
			IList list = (IList)Obj;
			list.Remove(obj);
			if (_typeHandler != null && _typeHandler.Enabled) {
				ObjectTreeNode otNode = null;
				for (int i = 0; i < LogicalNodes.Count; i++) {
					if (LogicalNodes[i] is ObjectTreeNode) {
						otNode = (ObjectTreeNode)LogicalNodes[i];
						if (otNode.Obj == obj)
							break;
					}
				}
				if (skipRemoveNode)
					return;
				if (otNode != null) {
					otNode.RemoveLogicalNode();
				} else {
					throw new Exception("Node does not exist where expected " 
										+ this + " " + obj);
				}
			}
		}
		// Returns false if something went wrong
		public override bool Paste(IBrowserNode node, bool isCopy)
		{
			try {
				// Save the object, cuz it may get removed if this is a cut
				Object cutCopyObj = ((ObjectTreeNode)node).Obj;
				// First, remove the old object from its parent.  This is
				// necessary to handle things like moving controls; they
				// will be upset if they are added to another node without
				// first removing them from the previous one.
				if (!isCopy)
					node.RemoveLogicalNode();
				if (AddObject(cutCopyObj) == null)
					return false;
			} catch (Exception ex) {
				ErrorDialog.Show(ex,
								"Exception during paste",
								MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
		
		public override int CompareTo(Object other)
		{
			// Don't sort on anything but the categories, leave the
			// other order alone
			if (other is BrowserTreeNode)
				return OrderCompareTo((BrowserTreeNode)other);
			return -1;
		}
		
		public override String GetName()
		{
			return _objInfo.GetName();
		}
		
		protected void GetDetailTextInt(bool showMember)
		{
			base.GetDetailText();
			_objInfo.GetDetailText(showMember);
			if (_castInfo != null) {
				DetailPanel.AddLink("Cast Type",
									!ObjectBrowser.INTERNAL,
									20,
									TypeLinkHelper.TLHelper,
									_castInfo.CastType);
			}
		}
		
		public override void GetDetailText()
		{
			GetDetailTextInt(ObjectInfo.SHOW_MEMBER);
		}
	}
}

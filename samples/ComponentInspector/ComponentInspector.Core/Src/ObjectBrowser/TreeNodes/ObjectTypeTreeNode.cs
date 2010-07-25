// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//	 <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.Obj;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.TreeNodes
{


	// This node represents a potential object which is really a 
	// member of some type of some other object, called the parent
	// object.
	internal class ObjectTypeTreeNode : ObjectTreeNode, 
		IInvokableTreeNode, IEventLoggingNode
	{
		// Set when this is invoked, even if no value was returned
		protected bool				  _invoked;


		// The property index values set when this property was last
		// successfully accessed.  This is saved incase we need to set
		// the same property again, as will be the case when the
		// object below change.
		protected Object[]			  _currentPropIndexValues;

		public override Object[] CurrentPropIndexValues {
			get {
				return _currentPropIndexValues;
			}
			set {
				_currentPropIndexValues = value;
			}
		}

		public override bool EventLogging {
			get {
				if (Obj != null)
					return base.EventLogging;
				Object parent = ((ObjectTreeNode)_logicalParent).Obj;
				return ObjectBrowser.EventLog.IsLogging(parent, LogEventInfo);
			}
		}

		internal ObjectTypeTreeNode(bool comNode) : base(comNode)
		{
		}

		internal ObjectTypeTreeNode(bool comNode, ObjectInfo objInfo) : base(comNode, objInfo)
		{
			Text = GetName();
		}

		internal ObjectTypeTreeNode(bool comNode, 
									ObjectInfo objInfo,
									MemberInfo member,
									bool useIntermediates) : this(comNode, objInfo)
		{

			PresentationInfo pi = PresentationMap.GetInfo(member.MemberType);


			// Needs an intermediate node for the base class type
			if (useIntermediates &&
				ComponentInspectorProperties.ShowBaseCategories &&
				!_objInfo.ObjParentType.Equals(member.DeclaringType))
			{
				PresentationInfo basePi = PresentationMap.GetInfo(PresentationMap.BASE_CLASS);
				_intermediateNodeTypes = new ArrayList();
				_intermediateNodeTypes.Add(basePi._intermediateNodeType);
			}
			else if (ComponentInspectorProperties.ShowObjectAsBaseClass &&
					 (ReflectionHelper.TypeEqualsObject(member.DeclaringType) ||
					  ReflectionHelper.TypeEqualsMarshalByRef(member.DeclaringType) ||
					  NoGoop.Win32.ActiveX.TypeEqualsComRoot(member.DeclaringType)))
			{
				PresentationInfo basePi = PresentationMap.GetInfo(PresentationMap.BASE_CLASS);
				_intermediateNodeTypes = new ArrayList();
				_intermediateNodeTypes.Add(basePi._intermediateNodeType);
			}

			if (useIntermediates && ComponentInspectorProperties.ShowMemberCategories)
			{
				if (_intermediateNodeTypes == null)
					_intermediateNodeTypes = new ArrayList();
				_intermediateNodeTypes.Add(pi._intermediateNodeType);
			}

			// We can cast a member which can be permanently remembered
			_castInfo = CastInfo.GetCastInfo(member);

			ImageIndex = pi._iconIndex;
			SelectedImageIndex = ImageIndex;
			_nodeOrder = pi._sortOrder;
		}

		public void Invoke(bool setMember, 
						   bool autoInvoke,
						   bool ignoreException)
		{
			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("Invoke: " + this + " obj: " + Obj);

			Object[] paramValues = null;
			Object fieldPropValue = null;

			// If this has no member info, then its not a type based node
			// and can't be invoked
			if (_objInfo.ObjMemberInfo == null)
				return;

			// Returns false if it did not work
			if (!_objInfo.CalcParams(ignoreException))
				return;

			if (_objInfo.NeedsParamValues(setMember))
			{
				paramValues = ObjectBrowser.ParamPanel.
					GetParameterValues(ignoreException, setMember,
									   out fieldPropValue);

				// This is an error case, something went wrong, but
				// it was already reported to the user
				if (paramValues == null)
					return;
			}
				
			// Clear out in case there is an error
			ColumnData[0] = null;

			Object prevValue = Obj;

			Cursor save = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			bool worked = _objInfo.Invoke(ParentObjectInfo,
										  this,
										  paramValues, 
										  fieldPropValue,
										  setMember, 
										  autoInvoke,
										  ignoreException);
			Cursor.Current = save;

			if (!worked)
			{
				if (TraceUtil.If(this, TraceLevel.Verbose))
					Trace.WriteLine("Invoke failed - obj: " + Obj);
				InvalidateNode();
				return;
			}

			_invoked = true;
			
			if (_castInfo != null)
				_castObject = _castInfo.DoCast(_objInfo.Obj);

			if (prevValue != Obj)
				ObjectValueChanged();

			// We have changed the value of an indexed property
			// update the param panel to reflect that, because
			// the only thing that might have been typed into the
			// param panel is the index value, not the property
			// value (on a get), or we might have changed to a different
			// index value
			if (!setMember &&
				_objInfo.ObjMemberInfo is PropertyInfo &&
				paramValues != null && paramValues.Length >= 0)
			{
				ObjectBrowser.ParamPanel.UpdatePropertyValue(Obj);
			}

			Text = GetName();
			DoDisplayValue();
		}

		protected override void ObjectValueChanged()
		{
			SetTypeHandler();
			base.ObjectValueChanged();
		}

		// Returns true if this node is automatically invoked
		public bool IsAutoInvoked(bool ignoreException)
		{
			if (TraceUtil.If(this, TraceLevel.Verbose))
			{
				TraceUtil.WriteLineVerbose(this, "IsAutoInvoked: " + this);
			}

			if (_objInfo.ObjMemberInfo == null)
				return false;
			if (_objInfo.ObjMemberInfo is FieldInfo)
				return true;
			if (_objInfo.ObjMemberInfo is PropertyInfo)
			{
				// We pay attention to the Auto Invoke property
				// only on the implicit invocation where an exception 
				// is not reported.  We do allow the property to be
				// gotten by explicitly selecting the node.
				if (!ignoreException || ComponentInspectorProperties.AutoInvokeProperties)
				{
					if (_objInfo.CalcParams(ignoreException) &&
						!_objInfo.NeedsParamValues(false) &&
						ClassCache.ShouldAutoInvokeProp(this))
						return true;
				}
			}
			return false;
		}


		public override bool HasSetProp()
		{
			if (_objInfo.ObjMemberInfo is PropertyInfo)
			{
				if (((PropertyInfo)_objInfo.ObjMemberInfo).CanWrite)
					return true;
			}
			return false;
		}

		public override bool HasGetProp()
		{
			if (_objInfo.ObjMemberInfo is PropertyInfo) {
				if (!IsAutoInvoked(Constants.IGNORE_EXCEPTION))
					return true;
			}
			return false;
		}

		public override bool HasSetField()
		{
			if (_objInfo.ObjMemberInfo is FieldInfo) {
				if (!((FieldInfo)_objInfo.ObjMemberInfo).IsLiteral)
					return true;
			}
			return false;
		}

		public override bool HasInvokeMeth()
		{
			if (_objInfo.ObjMemberInfo is MethodInfo)
				return true;
			return false;
		}


		public override bool HasCopy()
		{
			// Same conditions as for casting
			return HasCast();
		}

		public override bool HasCast()
		{
			MemberInfo mi = _objInfo.ObjMemberInfo;
			if (mi is EventInfo)
				return false;

			if (mi is MethodInfo) {
				if (((MethodInfo)mi).ReturnType.Equals(typeof(void)))
					return false;
			}
			return true;
		}


		public override bool HasEventLogging()
		{
			if (_objInfo.ObjMemberInfo is EventInfo)
				return true;
			// An object actually present can have logging turned on for it
			// This is handled by the parent
			if (Obj != null)
				return true;
			return false;
		}

		// Called when menu requests the change of event logging state
		public override void EventLoggingClick(object sender, EventArgs e)
		{
			// Things with an object are handled by the parent (which does
			// all of the events for the object)
			if (Obj != null) {
				base.EventLoggingClick(sender, e);
				return;
			}

			ObjectTreeNode parentNode = (ObjectTreeNode)_logicalParent;
			Object parent = parentNode.Obj;
			if (parent == null) {
				ErrorDialog.Show("You cannot enable event logging "
								+ ObjectInfo.NULL_PARENT_TEXT,
								"Cannot Enable Event Logging",
								MessageBoxIcon.Error);
				return;
			}

			bool newState = !EventLogging;

			try {
				// Tell the event logger
				ObjectBrowser.EventLog.ToggleLogging(this, newState);
			} catch (Exception ex) {
				ErrorDialog.Show(ex,
								 "Unable to Change Event Logging",
								 MessageBoxIcon.Error);
			}
		}


		public EventInfo LogEventInfo {
			get {
				if (_objInfo.ObjMemberInfo is EventInfo)
					return (EventInfo)_objInfo.ObjMemberInfo;
				return null;
			}
		}

		public Object LogEventObject {
			get {
				return ((ObjectTreeNode)_logicalParent).Obj;
			}
		}

		public String LogObjectName {
			get {
				return ParentObjectInfo.GetName();
			}
		}

		// Can't cut a type node, allowed though for the parent object node
		public override bool HasCut()
		{
			return false;
		}

		// Need to override from ObjectTreeNode which allows delete/rename
		public override bool HasDelete()
		{
			return false;
		}

		public override bool HasRename()
		{
			return false;
		}

		// After the new node has been added to the tree
		protected override void AddedToTree()
		{
			// This occurs on expansion, ignore the exception
			try {
				if (IsAutoInvoked(Constants.IGNORE_EXCEPTION)) {
					Invoke(false, true, Constants.IGNORE_EXCEPTION);
					// Need to recheck adding the dummy because
					// the invoke could have created an object
					AddDummy();
				}
				
			} catch (Exception ex) {
				if (TraceUtil.If(this, TraceLevel.Verbose))
					Trace.WriteLine("Add AutoInvoke Exception: " + ex);
				// IsAutoInvoked can throw, just ignore it
			}

		}

		public override void DoSelectInvoke()
		{
			try {
				// User explicitly selects it, show the exception
				if (IsAutoInvoked(!Constants.IGNORE_EXCEPTION))
					Invoke(false, true, !Constants.IGNORE_EXCEPTION);
			} catch (Exception ex) {
				if (TraceUtil.If(this, TraceLevel.Info))
					Trace.WriteLine("Select AutoInvoke Exception: " + ex);
				// Ignore, IsAutoInvoked can throw
			}
		}

		public override void SetupParamPanel()
		{
			if (_objInfo.ObjMemberInfo is PropertyInfo || _objInfo.ObjMemberInfo is FieldInfo) {
				ObjectBrowser.ParamPanel.
					Setup(ActionMenuHelper.CalcInvokeActionName(this, false),
						  ActionMenuHelper.CalcInvokeActionName(this, true),
						  new EventHandler(((BrowserTree)TreeView).TreeNodePopupClick),
						  new EventHandler(((BrowserTree)TreeView).TreeNodePopupClickSet),
						  this,
						  ((ObjectTreeNode)_logicalParent).Obj,
						  _objInfo.ObjMemberInfo,
						  _objInfo.ObjParameters);
			} else {
				ObjectBrowser.ParamPanel.Setup(ActionMenuHelper.CalcInvokeActionName(this, false),
						  new EventHandler(((BrowserTree)TreeView).TreeNodePopupClickSet),
						  this,
						  _objInfo.ObjParameters);
			}

			
		}

		public override String GetName()
		{
			// If we have an object then change the name to this
			if (ComponentInspectorProperties.ShowBaseClassNames) {
				if (!_objInfo.ObjMemberInfo.DeclaringType.Equals(_objInfo.ObjParentType)) {
					return _objInfo.ObjMemberInfo.Name + " (" + _objInfo.ObjMemberInfo.DeclaringType.Name + ")";
				}
			}
			return _objInfo.ObjMemberInfo.Name;
		}
	}
}

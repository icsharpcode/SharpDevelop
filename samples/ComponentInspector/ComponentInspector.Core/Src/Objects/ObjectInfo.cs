// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//	 <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.ObjBrowser;
using NoGoop.ObjBrowser.LinkHelpers;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;

namespace NoGoop.Obj
{

	internal class ObjectInfo : IObjectInfo
	{
		protected Object					_obj;

		// The best type known for the object.  If the object is
		// not present, this is the saem as the _objTypeStatic.  It
		// could be a more specific type in the case where the type
		// was determined dynamically.
		protected Type					  _objType;

		// The type calculated based on the static definitions
		protected Type					  _objTypeStatic;

		// Information about this method/property that is 
		// used to obtain this object.  This is a member of the object's
		// parent in the object graph
		protected MemberInfo				_objMemberInfo;

		// The type of the parent object if this is a member.  This is
		// always populated if _objMemberInfo is populated.
		protected Type					  _objParentType;

		// The name of the object if we can figure it out, or if its
		// explicitly given to the object by the creator of the ObjectInfo
		protected String					_objName;

		// Indicates we have figured out the parameters.  We can't
		// do this in the constructor because there might be an exception
		// that occurs.  We have to calculate the parameters the first
		// time they are really needed, so that if there is an exception
		// it is shown through.
		protected bool					  _paramsCalculated;

		protected ParameterInfo[]		   _objParameters;
		protected bool					  _needsParamValues;


		// K(object) V(ObjectInfo)
		protected static Hashtable		  _objInfoHash;	   


		// Explaination of why you can't do something if there is
		// no parent object
		internal const String			   NULL_PARENT_TEXT = 
			"because the "
			+ "object (the parent node in the tree) "
			+ "on which it has "
			+ "been defined is null. "
			+ "Try invoking the object in the parent node "
			+ "to give it a non-null value.";


		// These are used to identify the presentation characteristics
		// of the different kind of types
		internal const String	   CLASS = "Class";
		internal const String	   ENUM = "Enum";
		internal const String	   INTERFACE = "Interface";
		internal const String	   STRUCT = "Struct";

		internal Object Obj {
			get {
				return _obj;
			}
		}

		internal Type ObjType {
			get {
				return _objType;
			}
		}

		internal Type ObjTypeStatic {
			get {
				return _objTypeStatic;
			}
		}

		internal MemberInfo ObjMemberInfo {
			get {
				return _objMemberInfo;
			}
		}

		internal Type ObjParentType {
			get {
				return _objParentType;
			}
		}

		internal ParameterInfo[] ObjParameters {
			get {
				if (!CalcParams(true))
					return null;
				return _objParameters;
			}
		}

		internal String ObjectName {
			get {
				return _objName;
			}
			set {
				_objName = value;
			}
		}

		static ObjectInfo()
		{
			_objInfoHash = Hashtable.Synchronized(new Hashtable());
		}

		internal ObjectInfo()
		{
		}

		internal ObjectInfo(MemberInfo mi, Type parentType)
		{
			_objMemberInfo = mi;
			_objParentType = parentType;
			SetStaticType();
		}

		internal ObjectInfo(Object obj)
		{
			if (obj == null)
				throw new NullReferenceException("obj param cannot be null");

			SetObject(obj);
		}

		internal static ObjectInfo GetObjectInfo(Object obj)
		{
			return (ObjectInfo)_objInfoHash[obj];
		}

		internal static void RemoveObjectInfo(Object obj)
		{
			_objInfoHash.Remove(obj);
		}

		protected void SetObject(Object obj)
		{
			if (TraceUtil.If(this, TraceLevel.Info))
				Trace.WriteLine("SetObject: " + obj);

			// Get rid of the old object if it was present
			if (obj != null && _obj != null &&
				obj != _obj && _objInfoHash[_obj] != null)
				_objInfoHash.Remove(_obj);

			_obj = obj;
			if (obj == null)
				return;

			// Reset the type information since the actual object changed
			_objType = _objTypeStatic;
			SetType();

			if (_objType != null)
				SetObjectName();

			if (_objInfoHash[obj] == null)
				_objInfoHash.Add(obj, this);
		}


		// See if we can find a name for this object
		protected void SetObjectName()
		{
			PropertyInfo nameProp;

			nameProp = NamePropCache.GetNameProp(_objType);
			if (nameProp != null && _obj != null) {
				try {
					_objName = (String)nameProp.GetValue(_obj, null);
				} catch (Exception ex) {
					TraceUtil.WriteLineWarning(this, "Exception invoking "
											   + "Name property: " + ex);
				}
			}
		}

		internal bool CalcParams(bool ignoreException)
		{
			if (_paramsCalculated)
				return true;

			try {
				_needsParamValues = false;

				if (_objMemberInfo == null) {
					_paramsCalculated = true;
					return true;
				}

				if (_objMemberInfo.MemberType == MemberTypes.Method ||
					_objMemberInfo.MemberType == MemberTypes.Property) {
					if (_objMemberInfo.MemberType == MemberTypes.Property) {
						// These are the index parameters
						_objParameters = ((PropertyInfo)_objMemberInfo).
							GetIndexParameters();
					} else {
						_objParameters = ((MethodInfo)_objMemberInfo).GetParameters();
					}
					if (_objParameters.Length > 0)
						_needsParamValues = true;
					_paramsCalculated = true;
					return true;
				} else {
					// Fields and events never need param values
					_paramsCalculated = true;
					return true;
				}

			} catch (Exception ex) {
				if (ignoreException)
					return false;
				ErrorDialog.Show(ex,
								 "Exception calculating parameters on " 
								 + _objMemberInfo,
								 MessageBoxIcon.Error);
				return false;
			}
		}

		internal class MemberCompare : IComparer
		{
			public int Compare(Object o1, Object o2)
			{
				MemberInfo m1 = (MemberInfo)o1;
				MemberInfo m2 = (MemberInfo)o2;
				PresentationInfo mt1 = PresentationMap.GetInfo(m1.MemberType);
				PresentationInfo mt2 = PresentationMap.GetInfo(m2.MemberType);
				if (mt1._sortOrder > mt2._sortOrder)
					return 1;
				if (mt1._sortOrder < mt2._sortOrder)
					return -1;

				return m1.Name.CompareTo(m2.Name);
			}
		}

		internal class TypeCompare : IComparer
		{
			public int Compare(Object o1, Object o2)
			{
				Type m1 = (Type)o1;
				Type m2 = (Type)o2;
				return m1.Name.CompareTo(m2.Name);
			}
		}

		internal bool DoesTypeHaveKids()
		{
			return ReflectionHelper.DoesTypeHaveKids(_objType);
		}

		protected virtual void SetType()
		{
			if (_obj != null)
				_objType = _obj.GetType();
		}

		internal virtual void SetStaticType()
		{
			// Don't do anything if we can't or have already done it
			if (_objTypeStatic != null ||
				_objMemberInfo == null)
				return;

			switch (_objMemberInfo.MemberType) {
				case MemberTypes.Method:
					_objTypeStatic = ((MethodInfo)_objMemberInfo).ReturnType;
					break;
				case MemberTypes.Property:
					_objTypeStatic = ((PropertyInfo)_objMemberInfo).PropertyType;
					break;
				case MemberTypes.Field:
					_objTypeStatic = ((FieldInfo)_objMemberInfo).FieldType;
					break;
				default:
					break;
			}

			if (_objTypeStatic != null)
				_objType = _objTypeStatic;
		}


		protected void InvokeInternal(ObjectInfo parent,
									  IObjectNode objectNode,
									  Object[] parameterValues,
									  Object fieldPropValue,
									  bool setMember)
		{
			if (TraceUtil.If(this, TraceLevel.Verbose)) {
				Trace.WriteLine("Start invoke: " + _objMemberInfo);
				if (fieldPropValue != null)
					Trace.WriteLine("  fieldPropVal: " + fieldPropValue);
				if (parameterValues != null) {
					foreach (Object pval in parameterValues)
						Trace.WriteLine("  param: " + pval);
				}
			}

			switch (_objMemberInfo.MemberType) {
				case MemberTypes.Method:
				{
					MethodInfo m = (MethodInfo)_objMemberInfo;
					SetObject(m.Invoke(parent._obj, parameterValues));
					if (TraceUtil.If(this, TraceLevel.Verbose)) {
						Trace.WriteLine("Called: " + m 
										+ " on " + parent._obj 
										+ " return: " + _obj);
					}   
				}
				break;
				case MemberTypes.Property:
				{
					PropertyInfo p = (PropertyInfo)_objMemberInfo;

					if (setMember) {
						p.SetValue(parent._obj, fieldPropValue, parameterValues);
					}

					if (p.CanRead){
						SetObject(p.GetValue(parent._obj, parameterValues));
					} else {
						// Update the new property value, if we can't
						// read it from the property (on a set)
						if (setMember)
							SetObject(fieldPropValue);
					}   

					// Save these in case we need to set the property, 
					// like with the property propogation code below
					objectNode.CurrentPropIndexValues = parameterValues;

					if (TraceUtil.If(this, TraceLevel.Verbose)) {
						Trace.WriteLine("Property: " + p 
										+ " of " + parent._obj 
										+ " value: " + _obj);
					}
				}
				break;
				case MemberTypes.Field:
				{
					FieldInfo f = (FieldInfo)_objMemberInfo;
						
					if (parent._objType.IsValueType) {
						// This is a struct, its handled a little
						// different because of the inline value type
						// issue
						if (setMember) {
							StructAccessor.SetValue(objectNode, fieldPropValue);
						}
						SetObject(StructAccessor.GetValue(objectNode));
					} else {
						if (setMember)
							f.SetValue(parent._obj, fieldPropValue);
						SetObject(f.GetValue(parent._obj));
					}
					if (TraceUtil.If(this, TraceLevel.Verbose)) {
						Trace.WriteLine("Field: " + f
										+ " of " + parent._obj 
										+ " value: " + _obj);
					}
				}
				break;
				default:
				break;
			}

			// Everything worked, now lets handle any changes that
			// superiors need.
			if (!setMember)
				return;

			// If the parent member is a property, we want to do 
			// a SetProperty on it with the current value of its object
			// so that the SetProperty can process the most recently
			// changed values.  We need to do this up the line.
			while (true) {
				IObjectNode parentObjNode;
				parentObjNode = objectNode.ParentObjectNode;
				if (parentObjNode == null)
					break;

				// We hit an object node that has no member, we are done
				if (parentObjNode.ObjectInfo.ObjMemberInfo == null)
					break;

				// Look at enclosing property members only
				if (!(parentObjNode.ObjectInfo.ObjMemberInfo is
					  PropertyInfo))
					break;

				PropertyInfo propInfo = (PropertyInfo)parentObjNode.ObjectInfo.ObjMemberInfo;

				// Set the property
				try {
					propInfo.SetValue(parentObjNode.ParentObjectNode.Obj,
									  parentObjNode.Obj,
									  parentObjNode.CurrentPropIndexValues);
				} catch (Exception ex) {
					// A parent property might not have a set method
					// for example.
					TraceUtil.WriteLineInfo
						(this, "Exception in SetValue in property "
						 + "propagation for prop: " + propInfo 
						 + " ex: " + ex);
					break;
				}
				parentObjNode.DoDisplayValue();
				objectNode = parentObjNode;
			}
		}


		// Returns true if it worked
		internal bool Invoke(ObjectInfo parent,
							 IObjectNode objectNode,
							 Object[] parameterValues,
							 Object fieldPropValue,
							 bool setMember,
							 bool autoInvoke,
							 bool ignoreException)
		{
			if (parent == null) {
				if (TraceUtil.If(this, TraceLevel.Info)) { 
					Trace.WriteLine("Invoke - No parent");
				}
				return false;
			}

			if (_objMemberInfo == null || parent._obj == null) {
				if (TraceUtil.If(this, TraceLevel.Info)) { 
					Trace.WriteLine(TraceLevel.Info, 
						 "Invoke - No parent object or MemberInfo");
				}
				// Don't show this message in the auto invoke case
				if (!ignoreException && !autoInvoke) {
					ErrorDialog.Show("This method/property/field cannot "
									+ "be invoked "
									+ NULL_PARENT_TEXT,
									"Parent Object Not Present",
									MessageBoxIcon.Error);
				}
				return false;
			}

			try {
				InvokeInternal(parent, objectNode, 
							   parameterValues, fieldPropValue,
							   setMember);
			} catch (Exception e) {
				if (TraceUtil.If(this, TraceLevel.Info))
					Trace.WriteLine("Exception on invoke: " + e);

				if (!ignoreException) {
					Exception showException = e;
					// Remove the useless wrapper exception
					if (showException is TargetInvocationException)
						showException = e.InnerException;

					String errorStr = 
						 "Exception invoking " + _objMemberInfo.MemberType 
						 + " " + _objMemberInfo.Name
						 + ".  ";
					if (!setMember && _objMemberInfo.MemberType != MemberTypes.Method) {
						errorStr += "This can occur when you click on "
						 + "an item in the object tree.  ";
					}

					errorStr += "It means that the "
						 + "underlying object raised an exception when the "
						 + _objMemberInfo.MemberType + " was invoked.";

					ErrorDialog.Show
						(showException, errorStr,
						 "Exception invoking " 
						 + _objMemberInfo.MemberType 
						 + " " + _objMemberInfo,
						 MessageBoxIcon.Error);
				}
				return false;
			}	

			return true;
		}


		internal bool NeedsParamValues(bool setMember)
		{
			if (!_paramsCalculated)
				throw new Exception("CalcParams() must be called first");

			if (setMember)
				return true;
			return _needsParamValues;
		}

		internal virtual String GetName()
		{
			if (_objName != null && !_objName.Equals(""))
				return _objName;

			// Don't set the object name in this case, it should be
			// set only when the object is explicitly named
			if (_obj != null)
				return _obj.ToString();

			return null;
		}

		internal virtual String GetStringValue()
		{
			if (_obj == null)
				return null;

			if (ComponentInspectorProperties.DisplayHex) {
				if (_obj is Int16 || 
					_obj is Int32 || 
					_obj is Int64 || 
					_obj is UInt16 || 
					_obj is UInt32 || 
					_obj is UInt64 || 
					_obj is Byte)
					return "0x" + ((IFormattable)_obj).ToString("X", null);
				if (_obj is IntPtr)
					return "0x" + ((IntPtr)_obj).ToInt64().ToString("X");
			}

			return _obj.ToString();
		}

		internal const bool SHOW_MEMBER = true;

		internal virtual void GetDetailText()
		{
			GetDetailText(SHOW_MEMBER);
		}

		protected bool ShowMethodType()
		{
			if (!(_objMemberInfo is MethodInfo) || 
				!typeof(void).IsAssignableFrom(_objType))
				return true;
			return false;
		}


		internal virtual void GetDetailText(bool showMember)
		{
			if (_objName != null && !_objName.Equals(String.Empty)) {
				DetailPanel.Add("Name",
								!ObjectBrowser.INTERNAL,
								14,
								_objName.ToString());
			}

			if (_obj != null) {
				DetailPanel.Add("Object Value", 
								!ObjectBrowser.INTERNAL,
								50,
								_obj.ToString());
			}

			// Events don't have types
			// Don't show for members that are Void
			if (_objType != null && ShowMethodType()) {
				String desc = "CLR Type";
				if (_objMemberInfo is MethodInfo)
					desc = "Method Return Type (CLR)";

				DetailPanel.AddLink(desc, 
									!ObjectBrowser.INTERNAL,
									40,
									TypeLinkHelper.TLHelper,
									_objType);
				// Get documentation for the type
				GetDesc(_objType, desc);
			}

			if (_objTypeStatic != null) {
				DetailPanel.AddLink("Assigned Static Type",
									ObjectBrowser.INTERNAL,
									70,
									TypeLinkHelper.TLHelper,
									_objTypeStatic);
			}

			if (showMember && _objMemberInfo != null)
				MemberDetailText(_objMemberInfo);
		}


		internal static void GetDesc(MemberInfo mi, String descType)
		{
			DescriptionAttribute doc = null;
			try {
				doc = (DescriptionAttribute)Attribute.GetCustomAttribute(mi, typeof(DescriptionAttribute));
			} catch {
				// Ignore
			}

			if (doc != null) {
				DetailPanel.Add(descType + " Description",
								!ObjectBrowser.INTERNAL,
								65,
								doc.Description);
			}
		}

		internal const bool PARENS = true;

		// This is the string that shows static, visibility, 
		// get/set, etc
		internal static String MemberInfoString(MemberInfo member, bool parens)
		{
			StringBuilder vis = new StringBuilder();

			if (parens)
				vis.Append(" (");
			if (member is MethodInfo) {
				MethodInfo methInfo = (MethodInfo)member;
				if (methInfo.IsStatic)
					vis.Append("static ");
				if (methInfo.IsAssembly)
					vis.Append("assembly");
				else if (methInfo.IsFamily)
					vis.Append("protected");
				else if (methInfo.IsFamilyAndAssembly)
					vis.Append("protected and assembly");
				else if (methInfo.IsFamilyOrAssembly)
					vis.Append("protected or assembly");
				else if (methInfo.IsPrivate)
					vis.Append("private");
				else if (methInfo.IsPublic)
					vis.Append("public");
			} else if (member is FieldInfo) {
				FieldInfo fieldInfo = (FieldInfo)member;
				if (fieldInfo.IsStatic)
					vis.Append("static ");
				if (fieldInfo.IsAssembly)
					vis.Append("assembly");
				else if (fieldInfo.IsFamily)
					vis.Append("protected");
				else if (fieldInfo.IsFamilyAndAssembly)
					vis.Append("protected and assembly");
				else if (fieldInfo.IsFamilyOrAssembly)
					vis.Append("protected or assembly");
				else if (fieldInfo.IsPrivate)
					vis.Append("private");
				else if (fieldInfo.IsPublic)
					vis.Append("public");
			} else if (member is PropertyInfo) {
				PropertyInfo propInfo = (PropertyInfo)member;
				MethodInfo getMeth = propInfo.GetGetMethod(true);
				MethodInfo setMeth = propInfo.GetSetMethod(true);
				
				if (getMeth != null)
					vis.Append(MemberInfoString(getMeth, !PARENS));
				else
					vis.Append(MemberInfoString(setMeth, !PARENS));
				vis.Append("; ");

				if (getMeth != null)
					vis.Append("get");

				if (setMeth != null) {
					if (getMeth != null)
						vis.Append("/");
					vis.Append("set");
				}
			}
			if (member is EventInfo || member is Type)
				return null;

			if (parens)
				vis.Append(")");
			return vis.ToString();
		}

		internal static String GetTypeKind(Type type)
		{
			if (type.IsEnum)
				return ENUM;
			else if (type.IsClass)
				return CLASS;
			else if (type.IsInterface)
				return INTERFACE;
			else if (type.IsValueType)
				return STRUCT;
			else if (type.IsPrimitive)
				return CLASS;
			else if (type.Equals(typeof(Enum)))
				return CLASS;
			else
				throw new Exception("What is this? " + type);
		}

		internal static void MemberDetailText(MemberInfo mi)
		{
			GetDesc(mi, "Member");

			String memberType;
			if (mi.MemberType == MemberTypes.TypeInfo)
				memberType = GetTypeKind((Type)mi);
			else
				memberType = mi.MemberType.ToString();

			String memInfo = MemberInfoString(mi, PARENS);
			if (memInfo != null)
				memberType += memInfo;

			ILinkTarget linkTarget = null;

			if (mi.DeclaringType != null) {
				TypeLibrary typeLib = 
					TypeLibrary.GetTypeLib(mi.DeclaringType);
				if (typeLib != null)
					linkTarget = ComLinkHelper.CLHelper;
			}

			// No AX type linkage available, link to the assy/types nodes
			if (linkTarget == null)
				linkTarget = TypeLinkHelper.TLHelper;

			DetailPanel.AddLink(memberType,
								!ObjectBrowser.INTERNAL,
								10,
								linkTarget,
								mi,
								HelpLinkHelper.
								MakeLinkModifier(mi));
		
			if (mi.DeclaringType != null) {
				DetailPanel.AddLink("Member Declaring Type",
									!ObjectBrowser.INTERNAL,
									61,
									TypeLinkHelper.TLHelper,
									mi.DeclaringType,
									HelpLinkHelper.
									MakeLinkModifier(mi.DeclaringType));
			}
		}

		public override String ToString()
		{
			return GetName();
		}
	}
}

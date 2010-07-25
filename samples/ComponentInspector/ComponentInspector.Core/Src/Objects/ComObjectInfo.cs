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
using System.Runtime.InteropServices;

using Microsoft.Win32;
using NoGoop.ObjBrowser;
using NoGoop.ObjBrowser.Dialogs;
using NoGoop.ObjBrowser.LinkHelpers;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.Obj
{
	internal class ComObjectInfo : ObjectInfo
	{
		protected UCOMIMoniker          _moniker;
		protected String                _monikerName;
		protected String                _monikerDisplayName;
		protected Guid                  _persistGuid;
		protected String                _CLSID;
		protected RegistryKey           _classIdKey;
		protected RegistryKey           _classNameKey;
		protected bool                  _isRunningObjectTable;
		protected TypeLibrary           _typeLib;
		protected UCOMITypeInfo         _typeInfo;
		protected String                _comTypeName;
		
		// Interfaces implemented by this object
		protected ArrayList             _interfaces;
		
		// Classes whose default interface is an interface in this object
		protected ArrayList             _classesDefaultInterfaces;
		
		// Classes that implement an interface in this object
		protected ArrayList             _classesInterfaces;
		
		internal TypeLibrary TypeLib {
			get {
				GetTypeLib();
				return _typeLib;
			}
		}
		
		internal ComObjectInfo()
		{
		}
		
		internal ComObjectInfo(MemberInfo mi, Type parentType) : base(mi, parentType)
		{
		}
		
		internal ComObjectInfo(Object obj) : base(obj)
		{
		}
		
		internal ComObjectInfo(IntPtr objPtr)
		{
			SetObject(Marshal.GetObjectForIUnknown(objPtr));
		}
		
		protected static String ExtractGuid(String name)
		{
			try {
				return name.Substring(name.IndexOf('{'), name.IndexOf('}'));
			} catch (Exception) {
				return null;
			}
		}
		
		public static int GetRunningObjectCount()
		{
			int count = 0;
			UCOMIBindCtx bc;
			UCOMIRunningObjectTable rot;
			UCOMIEnumMoniker em;
			UCOMIMoniker[] moniker = new UCOMIMoniker[1];
			int unused;
			
			ActiveX.CreateBindCtx(0, out bc);
			bc.GetRunningObjectTable(out rot);
			rot.EnumRunning(out em);
			while (0 == em.Next(1, moniker, out unused))
				count++;
			Marshal.ReleaseComObject(bc);
			Marshal.ReleaseComObject(rot);
			Marshal.ReleaseComObject(em);
			return count;
		}
		
		// Finds the COM running objects
		public static IList GetRunningObjects(ProgressDialog progress)
		{
			ArrayList runningObjects = new ArrayList();
			Hashtable runningHash = new Hashtable();
			UCOMIBindCtx bc;
			UCOMIRunningObjectTable rot;
			UCOMIEnumMoniker em;
			UCOMIMoniker[] monikers = new UCOMIMoniker[1];
			
			ActiveX.CreateBindCtx(0, out bc);
			bc.GetRunningObjectTable(out rot);
			rot.EnumRunning(out em);
			// Look at each Moniker in the ROT
			int unused;
			while (0 == em.Next(1, monikers, out unused))
			{
				try
				{
					UCOMIMoniker moniker = monikers[0];
					Object obj;
					rot.GetObject(moniker, out obj);
					String monikerName;
					moniker.GetDisplayName(bc, null, out monikerName);
					
					ComObjectInfo comObjInfo;
					// Check for duplicates against the other running objects
					Object runObj = runningHash[obj];
					if (runObj != null)
					{
						// Get the existing object's moniker
						comObjInfo = (ComObjectInfo)
							ObjectInfo.GetObjectInfo(obj);
						if (monikerName.Equals(comObjInfo._monikerName))
						{
							TraceUtil.WriteLineInfo
								(typeof(ComObjectInfo),
								 "ROT - Skipping duplicate: " + monikerName);
							progress.UpdateProgress(1);
							continue;
						}
					}
					else
					{
						runningHash.Add(obj, obj);
					}
					comObjInfo = (ComObjectInfo)
						ObjectInfoFactory.GetObjectInfo(true, obj);
					// Need moniker name before update progress
					comObjInfo.CalcRunningObjName(rot, bc, 
												  moniker, monikerName);
					progress.UpdateProgressText(comObjInfo.GetMonikerName());
					progress.UpdateProgress(1);
					runningObjects.Add(comObjInfo);
					TraceUtil.WriteLineIf(typeof(ComObjectInfo), 
										  TraceLevel.Info,
										  "ROT - added: " 
										  + comObjInfo.GetName()
										  + " " 
										  + comObjInfo.ObjType
										  + " " 
										  + Win32Utils.RegKeyToString
										  (comObjInfo._classIdKey)
										  + Win32Utils.RegKeyToString
										  (comObjInfo._classNameKey));
				}
				catch (Exception ex)
				{
					TraceUtil.WriteLineIf(typeof(ComObjectInfo), 
										  TraceLevel.Info,
										  "ROT - Exception processing ROT entry: "
										  + ex);
					progress.UpdateProgress(1);
					continue;
				}
			}
			Marshal.ReleaseComObject(em);
			Marshal.ReleaseComObject(bc);
			Marshal.ReleaseComObject(rot);
			return runningObjects;
		}
		
		protected void CalcRunningObjName(UCOMIRunningObjectTable rot,
										  UCOMIBindCtx bc, 
										  UCOMIMoniker moniker,
										  String monikerName)
		{
			_moniker = moniker;
			_monikerName = monikerName;
			// FIXME - Does not work because IsRunning is defined
			// incorrectly in the IRunningObjectTable mapping
			//IsRunning = rot.IsRunning(Moniker);
				
			// Get the object from the table
			_isRunningObjectTable = true;
			// Persist Guid is probably not used for anything, but
			// let's get it anyway
			Guid persistGuid = new Guid();
			_moniker.GetClassID(out persistGuid);
			_persistGuid = persistGuid;
			// GUID is the *real* CLSID for the object
			String rotProgId = null;
			if (_monikerName.StartsWith("!{"))
			{
				int guidStart = _monikerName.IndexOf('{');
				int guidEnd = _monikerName.IndexOf('}');
				if (guidStart != -1 && guidEnd != -1)
				{
					_CLSID = 
						_monikerName.Substring(guidStart, guidEnd);
					_classIdKey = Windows.KeyCLSID.OpenSubKey(_CLSID);
					if (_classIdKey != null)
					{
						String progId = (String)
							_classIdKey.OpenSubKey("ProgID").
							GetValue(null);
						_classNameKey = 
							Windows.KeyClassRoot.OpenSubKey(progId);
						rotProgId = progId
							+ _monikerName.Substring(guidEnd + 1);
					}
				}
				// Use the translated progId if available
				if (rotProgId != null)
					_monikerDisplayName = rotProgId;
				else
					_monikerDisplayName = _monikerName;
			}
			else
			{
				// The moniker is something else, just use that
				_monikerDisplayName = _monikerName;
			}
		}
		
		// Gets the best moniker for this object
		protected String GetMonikerName()
		{
			if (_monikerDisplayName != null)
				return _monikerDisplayName;
			return null;
		}
		
		// Returns the index of this object's type in the typelib
		protected int GetTypeLib()
		{
			if (_typeLib != null)
				return -1;
			IntPtr dispPtr = IntPtr.Zero;
			int retVal = -1;
			// Get the IDispatch ptr
			try
			{
				dispPtr = Marshal.GetIDispatchForObject(_obj);
				Marshal.AddRef(dispPtr);
				retVal = GetTypeLib(dispPtr);
			}
			finally
			{
				if (dispPtr != IntPtr.Zero)
					Marshal.Release(dispPtr);
			}
			return retVal;
		}
		
		// Returns the index of this object's type in the typelib
		protected int GetTypeLib(IntPtr dispPtr)
		{
			if (TraceUtil.If(this, TraceLevel.Info))
			{
				Trace.WriteLine("ComObjInfo::GetTypeLib: " 
								+ _obj + " type: " + _obj.GetType());
			}
			if (_typeLib != null)
				return -1;
			UCOMITypeLib iTypeLib;
			int index = -1;
			try
			{
				IDispatch idisp = (IDispatch)
					Marshal.GetObjectForIUnknown(dispPtr);
				int count;
				int result = idisp.GetTypeInfoCount(out count);
				if (result != 0)
				{
					TraceUtil.WriteLineWarning
						(typeof(ComObjectInfo),
						 "ComObjInfo - "
						 + "GetTypeInfoCount failed: 0x" 
						 + result.ToString("x") + " obj: " + _obj);
					throw new COMException("(probably a bug, please report) "
										   + "Failed on GetTypeInfoCount", 
										   result);
				}
				if (count == 0)
				{
					TraceUtil.WriteLineWarning
						(typeof(ComObjectInfo),
						 "ComObjInfo - "
						 + " typeinfo count = 0: " + _obj);
					throw new Exception("This object has no type information "
										+ "(GetTypeInfoCount returned 0).  ");
				}
				result = idisp.GetTypeInfo(0, 0, out _typeInfo);
				if (result != 0)
				{
					TraceUtil.WriteLineWarning(typeof(ComObjectInfo),
											   "ComObjInfo - "
											   + "typeInfo not found:" + _obj);
					throw new COMException("(probably a bug, please report) "
										   + "Failed to get ITypeInfo", 
										   result);
				}
				if (_typeInfo == null)
				{
					TraceUtil.WriteLineWarning(typeof(ComObjectInfo),
											   "ComObjInfo - "
											   + "typeInfo not found:" + _obj);
					throw new Exception("(probably a bug, please report) "
										+ "Null TypeInfo pointer returned");
				}
				// Now we can get the type library information using these
				// nice interfaces provided as part of the interop services
				_typeInfo.GetContainingTypeLib(out iTypeLib, out index);
				_typeLib = TypeLibrary.GetTypeLib(iTypeLib);
			}
			catch (Exception ex)
			{
				if (_typeInfo != null)
				{
					Guid guid = BasicInfo.GuidFromTypeInfo(_typeInfo);
					TraceUtil.WriteLineWarning(typeof(ComObjectInfo),
											   "ComObjInfo (type " 
											   + guid + ")");
				}
				TraceUtil.WriteLineWarning(typeof(ComObjectInfo),
										   "Containing typelib not found:" 
										   + ex);
				throw new Exception("Cannot get TypeLib for object.  "
									+ "Getting the TypeLib information for "
									+ "an object is required as this contains "
									+ "the type information used to display "
									+ "the object. ", ex);
			}
			if (TraceUtil.If(this, TraceLevel.Info))
			{
				Trace.WriteLine("ComObjInfo - containing typelib index: " 
								+ index);
			}
			return index;
		}
		
		protected bool TypeIsGoodEnough(Type type)
		{
			bool isGood = false;
			if (type != null)
			{
				// If this is the generic COM wrapper type, then its not
				// good enough
				if (!ActiveX.TypeEqualsComRoot(type) &&
					!ReflectionHelper.TypeEqualsObject(type))
					isGood = true;
			}
			
			if (TraceUtil.If(this, TraceLevel.Info))
			{
				Trace.WriteLine("ComObjInfo - good enough: " 
									+ type + " " + isGood);
			}
			return isGood;
		}
		
		// We have an object and we want to figure out the best
		// type for it
		protected override void SetType()
		{
			if (TraceUtil.If(this, TraceLevel.Info))
			{
				Trace.WriteLine("ComObjInfo - SetType: " 
									+ _obj);
			}
			if (_obj != null)
			{
				if (!_obj.GetType().IsCOMObject &&
					!_obj.GetType().Equals(Windows.COM_ROOT_TYPE))
				{
					base.SetType();
					return;
				}
				if (TypeIsGoodEnough(_obj.GetType()))
				{
					_objType = _obj.GetType();
					return;
				}
			}
			if (TypeIsGoodEnough(_objType))
				return;
			// Get the type library so we can convert it only once.  If 
			// we don't do this, the GetTypeForITypeInfo code will 
			// try to convert it multiple times.  This also sets the _typeInfo
			// pointer.
			if (_typeLib == null)
			{
				IntPtr dispPtr;
				// Get the IDispatch ptr
				try
				{
					dispPtr = Marshal.GetIDispatchForObject(_obj);
				}
				catch
				{
					// This could just be a COM object, see if it 
					// implements any of the interfaces we know about
					_interfaces = 
						ComInterfaceInfo.GetImplementedInterfacesKnown(_obj);
					if (_interfaces.Count == 0)
					{
						throw new Exception
							("Unable to determine type of object, "
							 + "IDispatch not implemented, and it implements "
							 + "no interfaces associated with known type "
							 + "libraries.");
					}
					ComInterfaceInfo ciInfo = PickBestInterface();
					
					AssignType(ciInfo.GetCLRType());
					return;
				}
				// Get the type library from the dispPtr
				try
				{
					Marshal.AddRef(dispPtr);
					GetTypeLib(dispPtr);
				}
				finally
				{
					Marshal.Release(dispPtr);
				}
			}
			if (_typeLib != null)
			{
				// Get the COM pointer to the ITypeInfo so we can call
				// GetTypeForITypeInfo below.
				//IntPtr iTypeInfo = Marshal.GetIUnknownForObject(_typeInfo);
				// This figures out the actual class of the object, by hand
				// based on what interfaces it implements.
				//Type newType = FigureOutClass();
				// This returns the type of the interface this object implements,
				// but is done completely automatically.  This is probably the
				// better solution (since is less of my code).
				// The problem this this one is that it converts any 
				// necessary type libraries automatically without
				// having my hooks so I can properly find out about the
				// converted type libraries
				//Type newType = Marshal.GetTypeForITypeInfo(iTypeInfo);
				// Just look up the type by name from the type info's name,
				// this will get an interface type, which is fine.  A class type
				// might be better, but that takes more work to figure out.
				// FIXME - we may want to go to find the class type for those
				// objects that implement multiple interfaces, see about that.
				String typeName = ComClassInfo.GetTypeName(_typeInfo);
				_comTypeName = _typeLib.Name + "." + typeName;
				if (TraceUtil.If(this, TraceLevel.Info))
					Trace.WriteLine("TypeName: " + typeName);
				Type newType = 
					_typeLib.FindTypeByName(typeName,
											!TypeLibrary.FIND_CLASS);
				AssignType(newType);
			}
			// Can't figure it out, let the superclass deal with it
			if (_objType == null)
				base.SetType();
		}
		
		protected ComInterfaceInfo PickBestInterface()
		{
			ComInterfaceInfo bestIf = null;
			
			// Pick the one with the highest number of parents
			foreach (ComInterfaceInfo ifInfo in _interfaces)
			{
				if (bestIf == null ||
					ifInfo.ParentCount > bestIf.ParentCount)
					bestIf = ifInfo;
			}
			return bestIf;
		}
		
		protected void AssignType(Type newType)
		{
			if (TraceUtil.If(this, TraceLevel.Info))
				Trace.WriteLine("COM newType: " + newType);
			// Only wrap objects that are not already wrapped
			if (Windows.COM_ROOT_TYPE.IsAssignableFrom(newType))
			{
				Object wrapper = Marshal.CreateWrapperOfType(_obj, newType);
				if (TraceUtil.If(this, TraceLevel.Info))
					Trace.WriteLine("COM create wrapper: " + wrapper);
				_obj = wrapper;
			}
			_objType = newType;
		}
		
		// Figures out the class Type of an object based on the
		// information in this type library
		// This is presently not used, we call GetTypeForITypeInfo
		// instead.  But let's keep this around since it might
		// be valuable to get a COM class instead of interface
		internal Type FigureOutClass()
		{
			_interfaces = new ArrayList();
			IntPtr unkPtr = Marshal.GetIUnknownForObject(_obj);
			Marshal.AddRef(unkPtr);
			// Find out all if the interfaces in the library implemented
			// by the object
			foreach (ComInterfaceInfo intInfo in _typeLib.Interfaces)
			{
				Guid tempGuid = intInfo._guid;
				IntPtr implPtr;
				Marshal.QueryInterface(unkPtr, ref tempGuid, out implPtr);
				if (implPtr != IntPtr.Zero)
				{
					if (TraceUtil.If(this, TraceLevel.Info))
					{
						Trace.WriteLine("ComObjectInfo - impl IID: " 
											+ intInfo);
					}
					// Makes sure we have the type information for this interface
					// This method has been removed from ComInterfaceInfo
					// since its only used here
					//intInfo.ResolveInterface();
					_interfaces.Add(intInfo);
				}
			}                                                    
			Marshal.Release(unkPtr);
			// Find out if any classes have default interfaces of any
			// of the interfaces implemented, hopefully there will be
			// only one
			_classesDefaultInterfaces = new ArrayList();
			_classesInterfaces = new ArrayList();
			foreach (ComInterfaceInfo intInfo in _interfaces)
			{
				if (TraceUtil.If(this, TraceLevel.Info))
				{
					Trace.WriteLine("ComObjectInfo - checking IID: " 
										+ intInfo);
				}
				//foreach (ComClassInfo classInfo in intInfo._typeLib.Classes)
				foreach (ComClassInfo classInfo in 
						 ComClassInfo.GetClassInfos(intInfo))
				{
					if (TraceUtil.If(this, TraceLevel.Info))
					{
						Trace.WriteLine("TypeLib - checking class: "
											+ classInfo);
					}
					if (classInfo._defaultInterface == null)
						continue;
					if (classInfo._defaultInterface.Equals(intInfo))
					{
						_classesDefaultInterfaces.Add(classInfo);
					}
					// Note that this does not consider the inherited
					// interfaces, but that's probably for the best now
					// We want to consider those only after we have
					// exhausted all over possibilities
					foreach (ComInterfaceInfo implIface in 
							 classInfo._interfaces)
					{
						if (implIface.Equals(intInfo))
							_classesInterfaces.Add(classInfo);
					}
				}
			}
			
			/*****
				  FIXME - stuff to do to finish this:
1) make a hash table that has all classes that implement a given interface.  
Probably make a class/interface into structure that is a list from this
hash table.  This has an indication if its the classes default interface
or not.  Sometimes you will get an interface in one typelib and the class
is in another type lib.  This will help that, and it will also make 
the resolution of the classes faster.
2) deal with inherited interfaces.  After we have exhausted all 
possbilities with the actual interfaces, when we should do checking 
using the inherited interfaces so at least we will find something.
Of course, the inherited interfaces will show up on the query interface,
so we need to make sure we prefer the lower level interfaces.  Maybe
they will always show up first, but we need to see about this.
			 *****/
			// If no default interface, then just take the first class
			// that implements this interface
			BasicInfo chosenClass;
			if (_classesDefaultInterfaces.Count > 0)
			{
				chosenClass = (BasicInfo)_classesDefaultInterfaces[0];
				if (TraceUtil.If(this, TraceLevel.Info))
				{
					Trace.WriteLine("ComObjectInfo - FOUND default i/f: " 
										+ chosenClass);
				}
			}
			else if (_classesInterfaces.Count > 0)
			{
				chosenClass = (BasicInfo)_classesInterfaces[0];
				if (TraceUtil.If(this, TraceLevel.Info))
				{
					Trace.WriteLine("ComObjectInfo - FOUND non-default i/f: " 
										+ chosenClass);
				}
			}
			else if (_interfaces.Count == 1)
			{
				chosenClass = (BasicInfo)_interfaces[0];
				if (TraceUtil.If(this, TraceLevel.Info))
				{
					Trace.WriteLine("ComObjectInfo - FOUND only one interface: " 
										+ chosenClass);
				}
			}
			else
			{
				if (TraceUtil.If(this, TraceLevel.Info))
				{
					Trace.WriteLine("ComObjectInfo - NO CLASS FOUND "
										+ " (using static): " + this);
				}
				SetStaticType();
				return _objTypeStatic;
			}
			String className = chosenClass._container.Name 
				+ "." + chosenClass.Name;
			if (chosenClass is ComClassInfo)
				className += "Class";
			Type t = Type.GetType(className + "," + _typeLib.Name, 
								  true, true);
			
			if (TraceUtil.If(this, TraceLevel.Info))
			{
				Trace.WriteLine("ComObjectInfo - FOUND type: " + t);
			}
			return t;
			// This does not seem to work
			//return Type.GetTypeFromCLSID
			//    (((ComClassInfo)_classesDefaultInterfaces[0])._guid, true);
		}
		
		internal override String GetName()
		{
			String name = base.GetName();
			// Don't just show the useless com type name
			if (name != null &&
				name.Equals(Windows.COM_ROOT_TYPE_NAME) && 
				_objType != null)
				return _objType.FullName;
			return name;
		}
		
		internal override String GetStringValue()
		{
			String val = base.GetStringValue();
			// See if we can do better, just get the com type name
			if (val != null && 
				val.Equals(Windows.COM_ROOT_TYPE_NAME) &&
				_objType != null)
			{
				val = GetMonikerName();
				if (val == null)
					val = _objType.ToString();
			}
			return val;
		}
		
		internal static void GetDetailAxType(Type type,
											 MemberInfo memberInfo,
											 String desc,
											 int position)
		{
			TypeLibrary typeLib = TypeLibrary.GetTypeLib(type);
			if (typeLib != null)
			{
				if (desc == null)
					desc = "ActiveX Type";
				if (memberInfo is MethodInfo)
					desc = "Method Return Type (AX)";
				if (position == 0)
					position = 15;
				DetailPanel.AddLink(desc,
									!ObjectBrowser.INTERNAL,
									position,
									ComLinkHelper.CLHelper,
									type, 
									HelpLinkHelper.
									MakeLinkModifier(type));
				DetailPanel.AddLink("Type Library",
									!ObjectBrowser.INTERNAL,
									180,
									ComLinkHelper.CLHelper,
									typeLib,
									HelpLinkHelper.
									MakeLinkModifier(typeLib));
			}
		}
		
		internal override void GetDetailText(bool showMember)
		{
			base.GetDetailText(showMember);
			if (_isRunningObjectTable)
			{
				DetailPanel.Add("ROT Moniker",
								!ObjectBrowser.INTERNAL,
								200,
								GetMonikerName());
			}
			if (_objType != null && ShowMethodType())
				GetDetailAxType(_objType, _objMemberInfo, null, 0);
			if (_classIdKey != null)
			{
				DetailPanel.Add("CLSID (from ROT)",
								ObjectBrowser.INTERNAL,
								215,
								_classIdKey.Name);
			}
			if (_classNameKey != null)
			{
				DetailPanel.Add("Class Name (from CLSID)", 
								ObjectBrowser.INTERNAL,
								220,
								_classNameKey.Name);
			}
			// These last three don't appear any more because they
			// are never populated, they should probably go away
			// FIXME
			if (_interfaces != null)
			{
				foreach (ComInterfaceInfo i in _interfaces)
				{
					DetailPanel.Add("Interface",
									!ObjectBrowser.INTERNAL,
									300,
									i.ToString());
				}    
			}
			if (_classesDefaultInterfaces != null)
			{
				foreach (ComClassInfo cl in _classesDefaultInterfaces)
					{
						DetailPanel.Add("Class w/default",
										ObjectBrowser.INTERNAL,
										280,
										cl.ToString());
					}    
			}
			if (_classesInterfaces != null)
			{
				foreach (ComClassInfo cl in _classesInterfaces)
					{
						DetailPanel.Add("Class w/impl i/f",
										ObjectBrowser.INTERNAL,
										280,
										cl.ToString());
					}    
			}
		}
	}
}

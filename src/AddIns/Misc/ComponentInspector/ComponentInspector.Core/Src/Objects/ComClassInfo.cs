// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32;
using NoGoop.ObjBrowser;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.Obj
{
	internal class ComClassInfo : BasicInfo
	{
		internal ComInterfaceInfo       _defaultInterface;
		internal ArrayList              _interfaces;

		internal int                    _cImplTypes;

		internal ArrayList              _progIds;

		// FIXME - figure out what to do with this one
		internal String                 _progId;

			
		internal String                 _typeLibString;
		internal String                 _typeLibVersion;

		internal TYPEFLAGS              _typeFlags;

		// Indicates the type information has been provided
		protected bool                  _hasTypeLibInfo;

		protected Exception             _typeFailedException;

		// K(InterfaceInfo) V(ArrayList of ComClassInfo)
		protected static Hashtable      _classesByInterface;

		// K(CLSID) V(ComClassInfo)
		protected static Hashtable      _classesByCLSID;


		public Type Type
		{
			get	{
				if (_container is TypeLibrary)
					return GetTypeFromTypeLib((TypeLibrary)_container);

				// We have already failed, remember that
				if (_typeFailedException != null)
					throw _typeFailedException;

				// This is a class without a type library.
				return GetTypeForClass();
			}
		}

		internal ArrayList Interfaces {
			get {
				return _interfaces;
			}
		}

		internal ComInterfaceInfo DefaultInterface {
			get {
				return _defaultInterface;
			}
		}

		static ComClassInfo()
		{
			_classesByInterface = Hashtable.Synchronized(new Hashtable());
			_classesByCLSID = Hashtable.Synchronized(new Hashtable());
		}

		protected void Init()
		{
			_infoType = "CoClass";
			_progIds = new ArrayList();
			_presInfo = PresentationMap.GetInfo(TYPEKIND.TKIND_COCLASS);
		}

		// Constructor when reading from the registry "classes" section
		protected ComClassInfo(RegistryKey classKey,
							   String guidStr) :
				base(classKey, guidStr)
		{
			Init();

			// The registry name is not really the class name (in terms
			// of the type library) but really the documentation string.
			String name = (String)classKey.GetValue(null);

			RegistryKey progIdKey = 
				classKey.OpenSubKey("ProgId");
			if (progIdKey != null)
				_progId = (String)progIdKey.GetValue(null);

			// Give it a meaningful name if there is no class name
			// value in the registry
			if (name == null || name.Equals(""))
			{
				if (_progId != null)
					DocString = _progId + " (ProgId)";
				else
					DocString = guidStr;
			}
			else
			{
				DocString = name;
			}

			// See if there is type lib information
			RegistryKey key = _regKey.OpenSubKey("TypeLib");
			if (key != null)
				_typeLibString = (String)key.GetValue(null);

			key = _regKey.OpenSubKey("Version");
			if (key != null)
				_typeLibVersion = (String)key.GetValue(null);

			_typeKind = TYPEKIND.TKIND_COCLASS;
			_classesByCLSID.Add(_guid, this);

		}

		// Constuctor when creating from withing a typelib
		protected ComClassInfo(TypeLibrary typeLib, 
							   TYPEKIND typeKind,
							   int index,
							   UCOMITypeInfo typeInfo,
							   Guid guid)
		{
			Init();
			SetupTypeLibInfo(typeLib, typeKind, index, typeInfo, guid);
			_classesByCLSID.Add(_guid, this);
		}


		protected void SetupTypeLibInfo(TypeLibrary typeLib, 
										TYPEKIND typeKind,
										int index,
										UCOMITypeInfo typeInfo,
										Guid guid)
		{
			if (_hasTypeLibInfo)
				return;

			base.Setup(typeLib, typeKind, index, typeInfo, guid);

			TYPEATTR typeAttr;
			IntPtr typeAttrPtr;
			_typeInfo.GetTypeAttr(out typeAttrPtr);
			typeAttr = 
				(TYPEATTR)Marshal.PtrToStructure(typeAttrPtr, 
												 typeof(TYPEATTR));

			_cImplTypes = typeAttr.cImplTypes;
			_typeFlags = typeAttr.wTypeFlags;

			_typeInfo.ReleaseTypeAttr(typeAttrPtr);

			_hasTypeLibInfo = true;
		}


		// Returns a ComClassInfo for the requested class
		// Used for creating from within a typelib
		internal static ComClassInfo GetClassInfo(TypeLibrary typeLib, 
												  TYPEKIND typeKind,
												  int index)
		{
			UCOMITypeInfo typeInfo;
			typeLib.ITypeLib.GetTypeInfo(index, out typeInfo);
			Guid guid = GuidFromTypeInfo(typeInfo);
				
			ComClassInfo clsInfo = (ComClassInfo)_classesByCLSID[guid];
			if (clsInfo != null)
			{
				// Add the type lib information if we have seen this
				// class before
				clsInfo.SetupTypeLibInfo(typeLib, typeKind, index, 
										 null, Guid.Empty);
				return clsInfo;
			}
				
			return new ComClassInfo(typeLib, typeKind, index, 
									typeInfo, guid);
		}


		// Get a classinfo when reading from the registry
		internal static ComClassInfo GetClassInfo(RegistryKey classKey,
												  String guidStr)
		{
			Guid guid = new Guid(guidStr);
			ComClassInfo clsInfo = (ComClassInfo)_classesByCLSID[guid];
			if (clsInfo != null)
				return clsInfo;
				
			return new ComClassInfo(classKey, guidStr);
		}

		// When creating a class from just a class Id
		internal static ComClassInfo GetClassInfo(Guid clsId)
		{
			String clsIdString = Utils.MakeGuidStr(clsId);

			// Find the registry key 
			RegistryKey classKey = 
				Windows.KeyCLSID.OpenSubKey(clsIdString);

			if (classKey == null)
			{
				TraceUtil.WriteLineWarning
					(null, "No CLSID found for " + clsIdString);
				return null;
			}

			return GetClassInfo(classKey, clsIdString);
		}


		public override void AddDomTo(IList parent)
		{
			//return;

			// Do the class
			CodeTypeDeclaration codeDom = 
				ComInterfaceInfo.CreateTypeCodeDom
				(this, _interfaces, ComInterfaceInfo.DOCLASS);
			parent.Add(codeDom);

			// Do the interface associated with the class
			codeDom = ComInterfaceInfo.CreateTypeCodeDom
				(this, _interfaces, !ComInterfaceInfo.DOCLASS);
			parent.Add(codeDom);
		}

		internal static String GetTypeName(UCOMITypeInfo typeInfo)
		{
			String name;
			String dummy1 = null;
			int dummy2 = 0;
			String dummy3 = null;
		
			typeInfo.GetDocumentation(-1, out name,
									  out dummy1, out dummy2, out dummy3);
			return name;
		}


		internal override BasicInfo FindMemberByName(String name)
		{
			if (name == null)
				return null;

			// Classes have no members, get the member name from
			// on of the interfaces.
			foreach (BasicInfo interfaceInfo in _interfaces)
			{
				BasicInfo memberInfo = 
					interfaceInfo.FindMemberByName(name);
				if (memberInfo != null)
					return memberInfo;
			}
			return null;
		}


		internal void AddProgId(String progId)
		{
			_progIds.Add(progId);
		}

		protected Type GetTypeFromTypeLib(TypeLibrary typeLib)
		{
			if (typeLib == null)
			{
				_typeFailedException =
					new Exception("Unable to determine TypeLib "
									+ "from CLSID: " + _guidStr);
				throw _typeFailedException;
			}

			// Set this information because we might have read the
			// class from a source other than the type library
			_container = typeLib;
			_typeLib = typeLib;

			// Get the type associated with the CLSId from the typelib
			if (Name == null)
			{
				ComClassInfo clsInfo = typeLib.GetClassInfoFromCLSID(_guid);
				Name = clsInfo.Name;
			}

			Type type = typeLib.FindTypeByName(Name, 
											   TypeLibrary.FIND_CLASS);
			if (type == null)
			{
				_typeFailedException =
					new Exception("CLR type not found in " 
									+ _container 
									+ " for ActiveX type "
									+ this
									+ ".\n\nThis is likely caused by "
									+ "the assembly corresponding to "
									+ "this type library not being "
									+ "available.");
				throw _typeFailedException;
			}

			if (TraceUtil.If(this, TraceLevel.Info))
				Trace.WriteLine("ComClassInfo - type: " + type);
			return type;
		}


		// This is used for the case of a class outside of a type
		// libarary
		protected Type GetTypeForClass()
		{
			Type type = null;

			// See if we have type lib information from the registry
			if (_typeLibString != null)
			{
				return GetTypeFromTypeLib
					(TypeLibrary.GetTypeLib(new Guid(_typeLibString),
											_typeLibVersion));
			}


			// Have to try and create the object and get the type lib
			// information from the created object

			if (TraceUtil.If(this, TraceLevel.Info))
			{
				TraceUtil.WriteLineInfo(this,
										"Attempting to create obj for: "
										+ this);
			}


			IntPtr comObj;
			int result = ActiveX.CoCreateInstance(ref _guid,
												  (IntPtr)0,
												  ActiveX.CLSCTX_SERVER,
												  ref ActiveX.IUnknownIID,
												  out comObj);
			if (result == 0)
			{
				ComObjectInfo objInfo = null;
				try
				{
					// Wrap our object info stuff and get the type
					objInfo = new ComObjectInfo(comObj);
					type = GetTypeFromTypeLib(objInfo.TypeLib);
				}
				catch (Exception ex)
				{
					_typeFailedException = 
						new Exception("Unable to determine CLR type for "
									  + GetName(), ex);
					throw _typeFailedException;
				}
				finally
				{
					try
					{
						// Clean up the object info, if we made it that
						// far
						if (objInfo != null)
						{
							Object o = objInfo.Obj;
							ObjectInfo.RemoveObjectInfo(o);
							while (true)
							{
								int count = Marshal.ReleaseComObject(o);
								TraceUtil.WriteLineInfo
									(this,
									 "final Marshal.ReleaseComObject count: " 
									 + count);
								if (count <= 0)
									break;
							}
						}
						while (true)
						{
							int count = Marshal.Release(comObj);
							TraceUtil.WriteLineInfo
								(this, "final Marshal.Release count: " 
								 + count);

							if (count <= 0)
								break;
						}
					}
					catch (Exception ex)
					{
						TraceUtil.WriteLineWarning
							(this, "error on cleanup: " + ex);
						// We tried...
					}
					ActiveX.CoFreeUnusedLibraries();
				}

				
				return type;
			}

			_typeFailedException = 
				new Exception("Unable to determine CLR type because "
								+ "I can't create a COM object (0x"
								+ result.ToString("X")
								+ ") from CLSID: " + _guidStr);
			throw _typeFailedException;

		}



		internal const bool DEFAULT = true;

		internal void AddInterface(ComInterfaceInfo intInfo,
								   bool isDefault)
		{
			_interfaces.Add(intInfo);
			if (isDefault)
				_defaultInterface = intInfo;

			// Add to the global hash
			lock (_classesByInterface)
			{
				ArrayList classList = (ArrayList)_classesByInterface[intInfo];
				if (classList == null)
				{
					classList = new ArrayList();
					_classesByInterface.Add(intInfo, classList);
				}
				classList.Add(this);                
			}
		}


		// Finds the best class that implements this interface
		internal static ArrayList GetClassInfos(ComInterfaceInfo intInfo)
		{
			// Add to the global hash
			lock (_classesByInterface)
			{
				ArrayList ret = (ArrayList)_classesByInterface[intInfo];
				if (ret == null)
					return new ArrayList();
				return ret;
			}
		}


		public override void GetDetailText()
		{
			base.GetDetailText();

			foreach (String progId in _progIds) {
				DetailPanel.Add("ProgID",
								!ObjectBrowser.INTERNAL,
								90,
								progId);
			}

			if (_defaultInterface != null) {
				DetailPanel.AddLink("Default Interface",
									!ObjectBrowser.INTERNAL,
									100,
									_defaultInterface,
									null);
			}

			if (_interfaces != null) {
				foreach (ComInterfaceInfo interfaceInfo in _interfaces) {
					DetailPanel.AddLink("Implemented Interface",
					                    !ObjectBrowser.INTERNAL,
					                    110,
					                    interfaceInfo,
					                    null);
				}
			}
		}
	}
}

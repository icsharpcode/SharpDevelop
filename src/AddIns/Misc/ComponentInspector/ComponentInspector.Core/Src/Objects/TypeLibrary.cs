// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Microsoft.Win32;
using NoGoop.Controls;
using NoGoop.ObjBrowser;
using NoGoop.ObjBrowser.Dialogs;
using NoGoop.ObjBrowser.LinkHelpers;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.Obj
{
	// Represents a type library
	internal class TypeLibrary : BasicInfo, ITypeLibImporterNotifySink, AxImporter.IReferenceResolver
	{
		protected String                _fileName;
		protected String                _helpDir;
		protected uint                  _flags;
		protected TypeLibKey            _typeLibKey;
		protected AssemblyInfo          _assyInfo;
		protected AssemblyInfo          _axAssyInfo;
		
		// The assembly for this type is its loaded
		protected Assembly              _assy;
		protected Assembly              _axAssy;
		
		// K(UCOMITypeInfo) V(InterfaceInfo)
		protected Hashtable             _interfaces;
		
		// Array of ClassInfo
		protected ArrayList             _classes;
		
		// K(String of aliased type) V(String of underlying type)
		protected Hashtable             _typeDefHash;
		protected Assembly              _primaryInteropAssy;
		protected String                _primaryInteropAssyName;
		
		// The full name of the assembly this type library was
		// translated from.  This is contained as a custom attribute
		// in the assembly
		protected String                _translatedAssyFullName;
		
		// This is set if the user has been offered the chance to
		// find the assembly for a given type library but has declined,
		// so we should not ask again
		protected bool                  _dontFindAssy;
		protected bool                  _typeLibInfoRead;
		protected bool                  _openedFile;
		protected bool                  _registered;
		protected bool                  _converted;
		
		// Indicates this has been remembed in the registry
		protected bool                  _remembered;
		
		// Indicates we are searching type type library and it should
		// not be remembered.
		protected static bool           _searchMode;
		
		internal bool Registered {
			get {
				return _registered;
			}
			set {
				_registered = value;
			}
		}
		
		internal bool Converted {
			get {
				return _converted;
			}
		}
		
		internal String HelpFile {
			get {
				return _helpFile;
			}
		}
		
		internal int HelpContext {
			get {
				return _helpContext;
			}
		}
		
		internal ICollection Interfaces {
			get {
				ReadTypeLibInfo();
				return _interfaces.Values;
			}
		}
		
		internal IList Classes {
			get {
				ReadTypeLibInfo();
				return _classes;
			}
		}
		
		internal override ArrayList Members {
			get {
				ReadTypeLibInfo();
				return _members;
			}
		}
		
		internal Assembly Assy {
			get {
				return _assy;
			}
		}
		
		internal Assembly AxAssy {
			get {
				return _axAssy;
			}
		}
		
		internal String AssyName {
			get {
				return _assyInfo._name;
			}
		}
		
		internal UCOMITypeLib ITypeLib {
			get {
				ReadTypeLibFile();
				return _iTypeLib;
			}
		}
		
		internal String FileName {
			get {
				ReadTypeLibFile();
				return _fileName;
			}
		}
		
		internal Hashtable TypeDefHash {
			get {
				// Allocate on demand
				if (_typeDefHash == null) {
					_typeDefHash = new Hashtable();
				}
				return _typeDefHash;
			}
		}
		
		internal TypeLibKey Key {
			get {
				return _typeLibKey;
			}
			set	{
				_typeLibKey = value;
				_guid = _typeLibKey._guid;
			}
		}
		// K(TypeLibKey) V(TypeLibrary)
		protected static Hashtable      _openedTypeLibs = new Hashtable();
		
		// K(AssemblyName.ToString()) V(TypeLibrary)
		protected static Hashtable      _primaryInteropTypeLibs = new Hashtable();
		protected static SortedList      _registeredTypeLibs = new SortedList();
		protected static bool           _registeredTypeLibsValid;
		
		// K(Assembly.FullName) V(TypeLib)
		protected static Hashtable      _assyToTypeLibMap = new Hashtable();
		
		protected static String         TYPELIB = "typelib";
		protected static String         ASSEMBLY_FILE = "assemblyFile";
		
		protected TypeLibrary()
		{
			_members = new ArrayList();
			_memberNames = new Hashtable();
			_classes = new ArrayList();
			_interfaces = new Hashtable();
			_assyInfo = new AssemblyInfo();
			_axAssyInfo = new AssemblyInfo();
			_presInfo = PresentationMap.GetInfo(PresentationMap.COM_TYPELIB);
			_infoType = "TypeLib";
		}
		
		protected TypeLibrary(TypeLibKey key) : this()
		{
			_typeLibKey = key;
		}
		
		public override String GetSearchNameString()
		{
			// Gets the print name which is what a user will probably
			// want to see for a typelib
			return GetName();
		}
		
		public override bool HasSearchChildren(ISearcher searcher)
		{
			searcher.ReportStatus("Reading: ", GetName(PREFER_DESCRIPTION));
			try {
				_searchMode = true;
				ReadTypeLibInfo();
				_searchMode = false;
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning
					(this, "Exception reading typelib info during find: " + 
					this + " ex: " + ex);
				return false;
			}
			return base.HasSearchChildren(searcher);
		}
		
		public override ICollection GetSearchChildren()
		{
			_searchMode = true;
			ReadTypeLibInfo();
			_searchMode = false;
			return base.GetSearchChildren();
		}
		
		protected static void GetTypeLibsFromRegistry(String guidStr)
		{
			TypeLibrary lib = null;
			RegistryKey regKey = Windows.KeyTypeLib.OpenSubKey(guidStr);
			String[] subKeyNames = regKey.GetSubKeyNames();
			foreach (String versionStr in subKeyNames) {
				RegistryKey versionKey = regKey.OpenSubKey(versionStr);
				try {
					if (versionKey == null) {
						throw new Exception("Version entry not "
											+ "found for typelib: " 
											+ guidStr);
					}
					TypeLibKey key = new TypeLibKey(new Guid(guidStr), versionStr);
					// See if we already know about it
					TypeLibrary convLib = (TypeLibrary)_openedTypeLibs[key];
					if (convLib == null) {
						lib = new TypeLibrary();
						lib.Key = key;
						lib.PopulateFromRegistry(versionKey);
						// We don't read the type lib info until the user
						// wants the detail text or something else 
						// that requires it
					} else {
						lib = convLib;
					}
					_registeredTypeLibs.Add(lib, lib);
				} catch (Exception ex) {
					TraceUtil.WriteLineInfo(null,
											"TypeLib - failure to read: "
											+ versionKey + " " + ex);
				}
			}
		}
		
		internal static int GetRegisteredTypeLibsCount()
		{
			// Why is this different, and it seems to be different
			// by a lot 541 vs 519 (registry)
			// FIXME
			//Console.WriteLine("internal reg count: " 
			//                  + _registeredTypeLibs.Values.Count
			//                  + "registry count: " 
			//                  + Windows.KeyTypeLib.GetSubKeyNames().Length);
			if (_registeredTypeLibsValid)
				return _registeredTypeLibs.Values.Count;
			return Windows.KeyTypeLib.GetSubKeyNames().Length;
		}
		
		internal static ICollection GetRegisteredTypeLibs(ProgressDialog progress)
		{
			lock (typeof(TypeLibrary)) {
				if (_registeredTypeLibsValid) {
					if (progress != null) {
						progress.UpdateProgress(_registeredTypeLibs.Values.Count);
					}
					return _registeredTypeLibs.Values;
				}
				String[] keys = Windows.KeyTypeLib.GetSubKeyNames();
				foreach (String str in keys) {
					if (progress != null)
						progress.UpdateProgress(1);
					GetTypeLibsFromRegistry(str);
				}    
				_registeredTypeLibsValid = true;
			}
			return _registeredTypeLibs.Values;
		}
		
		// Returns the TypeLibrary if it has been opened, for use
		// when we don't need the library translated nor to see its
		// contents
		internal static TypeLibrary GetTypeLibOpened(Guid guid,
													String version)
		{
			TypeLibKey typeLibKey = new TypeLibKey(guid,
												  version);
			lock (typeof(TypeLibrary)) {
				TypeLibrary lib = (TypeLibrary)_openedTypeLibs[typeLibKey];
				TraceUtil.WriteLineInfo(null, 
										"GetTypeLibOpened: " 
										+ ((lib == null) ? "false " : "true ")
										+ guid + " " + version);
				return lib;
			}
		}
		
		// Called when a type library is loaded from the remembered
		// type libraries upon start up
		internal static void RestoreTypeLib(String fileName,
											Guid guid,
											string version)
		{
			TypeLibrary lib = null;
			try {
				lock (typeof(TypeLibrary)) {
					TraceUtil.WriteLineInfo(null, 
											"RestoreTypeLib - guid/version: " 
											+ guid + " " + version);
						
					TypeLibKey typeLibKey = new TypeLibKey(guid, version);
					// We should not have already seen this one, but
					// if we have, we will be decent about it
					lib = (TypeLibrary)_openedTypeLibs[typeLibKey];
					if (lib == null) {
						lib = new TypeLibrary(typeLibKey);
						try {
							lib.PopulateFromRegistry(typeLibKey);
						}
						catch (Exception ex) {
							// Get the file name we recorded since its not
							// registered
							lib._fileName = fileName;
							// Might not be registered, ignore error
							TraceUtil.WriteLineInfo
								(typeof(TypeLibrary),
								"Unregistered typelib restore: " 
								+ fileName 
								+ " exception: " + ex);
						}
					}
					lib.ReadTypeLibFile();
					if (lib._openedFile) {
						if (!_searchMode) {
							ComSupport.AddTypeLib(lib);
						}
						TraceUtil.WriteLineInfo(null, 
												"TypeLib - loaded/restored: " 
												+ lib);
					} else {
						// Something's wrong, don't try and open again
						lib.ForgetMe();
						TraceUtil.WriteLineInfo(null, 
												"TypeLib - failed to open "
												+ "- forgotten: " 
												+ lib);
					}
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning
					(null, "TypeLib - deleting bad typelib entry: " 
					+ lib + " " + ex);
				// Something's wrong, don't try and open again
				lib.ForgetMe();
			}
		}
		
		// Called when an assembly that has a typelib associated
		// with it is restored
		internal static void RestoreAssembly(Assembly assy, Guid guid, string version)
		{
			AssociateAssyWithTypeLib(assy, guid, version, false);
		}
		
		// Associates the specified assembly with the type library.
		// This is called when assemblies are restored, or when
		// they are loaded and must be hooked up to a type library
		internal static void AssociateAssyWithTypeLib(Assembly assy,
													 Guid guid,
													 string version,
													 bool primaryInterop)
		{
			try {
				lock (typeof(TypeLibrary)) {
					TraceUtil.WriteLineInfo(null, 
											"AssocateAssy - "
											+ "guid/version: " 
											+ guid + " " + version);
						
					TypeLibKey typeLibKey = new TypeLibKey(guid,
														  version);
					// We should already know about it, if not, 
					// just ignore it
					TypeLibrary lib =
						(TypeLibrary)_openedTypeLibs[typeLibKey];
					if (lib == null) {
						TraceUtil.WriteLineInfo(typeof(TypeLibrary),
												"no typelib found for assy: "
												+ assy);
						return;
					}
					if (primaryInterop)
						lib._primaryInteropAssy = assy;
					lib.SetAssemblyPointer(assy);
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(null, "TypeLib - deleting bad assy entry: " 
					+ assy + " " + ex);
				// Remove the typelib information associated with the
				// assembly
				AssemblySupport.ForgetAssemblyTypeLib(assy);
			}
		}
		
		// Called when an assembly is loaded
		// Hooks the assy to a typelib if enough information in the
		// assembly is available
		internal static void HandleAssyLoad(Assembly assy)
		{
			try  {
				Guid guid = Guid.Empty;
				IList attrs = Attribute.GetCustomAttributes(assy, typeof(ImportedFromTypeLibAttribute));
				if (attrs.Count > 0) {
					attrs = Attribute.GetCustomAttributes(assy, typeof(GuidAttribute));
					if (attrs.Count > 0) {
						guid = new Guid(((GuidAttribute)attrs[0]).Value);
					}
				}
				// This assembly was not created from a typelib,
				// we don't care about it
				if (guid.Equals(Guid.Empty))
					return;
				// Get each version where this assembly supports a typelib
				attrs = Attribute.GetCustomAttributes(assy, typeof(PrimaryInteropAssemblyAttribute));
				foreach (Attribute attr in attrs) {
					PrimaryInteropAssemblyAttribute pi = (PrimaryInteropAssemblyAttribute)attr;
					TraceUtil.WriteLineInfo(null, "RestoreTypeLib - loading primary interop");
					// Load each primary interop assy version
					AssociateAssyWithTypeLib(assy, guid,
						pi.MajorVersion + "." + pi.MinorVersion,
						true);
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(null, "RestoreTypelib - exception" + ex);
				
				throw new Exception("Unable to restore typelib", ex);
			}
		}
		
		// Returns a translated type library given the specified COM typelib
		internal static TypeLibrary GetTypeLib(UCOMITypeLib iTypeLib)
		{
			return GetTypeLib(GetTypeLibKey(iTypeLib), iTypeLib);
		}
		
		// Get the COM type library, if any, corresponding to the
		// specified type
		internal static TypeLibrary GetTypeLib(Type type)
		{
			lock (_assyToTypeLibMap) {
				TypeLibrary typeLib = (TypeLibrary)_assyToTypeLibMap[type.Assembly.FullName];
				return typeLib;
			}
		}
		
		internal static TypeLibrary GetTypeLib(TypeLibKey key)
		{
			return GetTypeLib(key, null);
		}
		
		internal static TypeLibrary GetTypeLib(Guid guid, String versionStr)
		{
			return GetTypeLib(new TypeLibKey(guid, versionStr), null);
		}
		
		// If the UCOMITypeLib pointer is good, then remember
		// that, because the type library may not be in the registry
		// or a file we can open.  But we can still convert it and
		// read its information
		internal static TypeLibrary GetTypeLib(TypeLibKey key, UCOMITypeLib iTypeLib)
		{
			TraceUtil.WriteLineInfo(null, "TypeLib - GetTypeLib: " + key);
			lock (typeof(TypeLibrary)) {
				TypeLibrary lib = (TypeLibrary)_openedTypeLibs[key];
				if (lib != null && lib._converted)
					return lib;
				try {
					if (lib == null) {
						// Have to create one
						lib = new TypeLibrary(key);
						lib._iTypeLib = iTypeLib;
						try {
							lib.PopulateFromRegistry(key);
						} catch (Exception ex) {
							// This could be ok, sometimes a typelib is not
							// in the registry
							TraceUtil.WriteLineWarning
								(typeof(TypeLibrary),
								"GetTypeLib exception (not in registry)  "
								+ ex);
							if (lib._iTypeLib == null) {
								throw new Exception("Failed to find type library information in registry", ex);
							}
						}
					}
					lib.TranslateTypeLib();
					// Generate the C# source after the translation for
					// now because the source generation requires the 
					// definitions to be read which can cause a call
					// to GetTypeLib(), don't want to be re-entered
					// FIXME
					//lib.GenerateSource();
					return lib;
				} catch (Exception ex) {
					TraceUtil.WriteLineWarning(typeof(TypeLibrary), "GetTypeLib exception " + ex);
					throw new Exception("Error getting type library: ", ex);
				}
			}
		}
		
		// Gets a type library given a file name, this returns
		// an existing type library if it is already known
		internal static TypeLibrary GetTypeLib(String fileName)
		{
			TraceUtil.WriteLineInfo(null,  "TypeLib - GetTypeLib (file): " + fileName);
			// Key is assigned in ReadTypeLibFile
			TypeLibrary lib = new TypeLibrary();
			lib._fileName = fileName;
			lib.ReadTypeLibFile();
			if (lib._iTypeLib == null)
				return null;
			// Find the type library by guid just in case it was
			// already opened.  This will also translate it.
			return GetTypeLib(lib._iTypeLib);
		}
		
		// Reads the type library information
		internal void ReadTypeLibFile()
		{
			if (_openedFile)
				return;
			TraceUtil.WriteLineInfo(this,
									"Opening: " + _fileName);
			// We may have received a pointer to this typelibrary
			// without having to open the file
			if (_iTypeLib == null) {
				try {
					ActiveX.LoadTypeLibEx(_fileName, 
										 RegKind.RegKind_None, 
										 out _iTypeLib); 
				} catch (Exception ex) {
					TraceUtil.WriteLineWarning(this, 
											  "Failed to load typelib file: " 
											  + _fileName + " " + ex);
					return;
				}
			}
	
			if(_iTypeLib == null) {
				TraceUtil.WriteLineWarning(this, 
										  "Failed to load typelib file: " 
										  + _fileName);
				return;
			}
			_typeLibKey = GetTypeLibKey(_iTypeLib);
			GetDocumentation(-1);
			// When a assembly is exported to a type lib, the assembly
			// FullName is stored as this custom attribute in the type
			// library.  We save this name here and then we need to
			// get an assembly for this typelib, we just load it with 
			// this name.
			if (_iTypeLib is ITypeLib2) {
				Guid assyDataGuid = new Guid("90883F05-3D28-11D2-8F17-00A0C9A6186D");
				Object custData;
				int ret = ((ITypeLib2)_iTypeLib).GetCustData(ref assyDataGuid, out custData);
				if (ret == 0) {
					_translatedAssyFullName = (String)custData;
					TraceUtil.WriteLineInfo(this, 
											"TypeLib was translated from: " 
											+ _translatedAssyFullName);
				}
			}
			// Create the names if we will/have converted the typelib
			if (_translatedAssyFullName == null)
				CreateAssyNames();
			lock (typeof(TypeLibrary)) {
				// We may have already seen this one, just forget
				// the new one
				if (_openedTypeLibs[_typeLibKey] == null)
					_openedTypeLibs.Add(_typeLibKey, this);
			}
			_openedFile = true;
			TraceUtil.WriteLineInfo(this, "Opened: " + this);
		}
		
		// Reads the type library information
		// This is not supposed to throw
		internal void ReadTypeLibInfo()
		{
			if (_typeLibInfoRead)
				return;
			ReadTypeLibFile();
			ProgressDialog progress = new ProgressDialog();
			progress.Setup("Reading " + GetName(),
						  "Please wait while I read the definitions for " 
						  + GetName(),
						  ProgressDialog.NO_PROGRESS_BAR,
						   !ProgressDialog.HAS_PROGRESS_TEXT,
						  ProgressDialog.FINAL);
			progress.ShowIfNotDone();
			try {
				ReadTypeLibInfoInternal();
			} finally {
				progress.Finished();
			}
		}

		// This is not supposed to throw unless something is really wrong
		internal void ReadTypeLibInfoInternal()
		{
			if (_iTypeLib == null)
				return;
			// Get the list of classes and interfaces implemented by this library
			// First do only the interfaces because we need to hook the default
			// interface to the class
			int typeInfoCount = _iTypeLib.GetTypeInfoCount();
			TYPEKIND typeKind;
			BasicInfo basicInfo;
			for (int i = 0; i < typeInfoCount; i++) {
				basicInfo = null;
				_iTypeLib.GetTypeInfoType(i, out typeKind);
				if (typeKind == TYPEKIND.TKIND_INTERFACE ||	typeKind == TYPEKIND.TKIND_DISPATCH) {
					basicInfo = ComInterfaceInfo.GetInterfaceInfo(this, typeKind, i);
					_interfaces.Add(basicInfo._typeInfo, basicInfo);
				} else if (typeKind == TYPEKIND.TKIND_ALIAS) {
					basicInfo =  new ComTypeDefInfo(this, typeKind, i);
				} else if (typeKind != TYPEKIND.TKIND_COCLASS) {
					basicInfo = new ComStructInfo(this, typeKind, i);
				}
				if (basicInfo == null)
					continue;
				basicInfo._container = this;
				// Dispatch and regular interfaces can have the same
				// name, just make sure we point to one of them
				if (_memberNames[basicInfo.Name] == null)
					_memberNames.Add(basicInfo.Name, basicInfo);
				_members.Add(basicInfo);
				if (TraceUtil.If(null, TraceLevel.Verbose)) {
					Trace.WriteLine("TypeLib - has type: " + basicInfo + " " + typeKind);
				}
			}
			// Now do the classes
			for (int i = 0; i < typeInfoCount; i++) {
				_iTypeLib.GetTypeInfoType(i, out typeKind);
				if (typeKind == TYPEKIND.TKIND_COCLASS) {
					ComClassInfo classInfo = ComClassInfo.GetClassInfo(this, typeKind, i);
					classInfo._container = this;
					classInfo._interfaces = new ArrayList();
					for (int j = 0; j < classInfo._cImplTypes; j++) {
						int intImpl;
						int implTypeFlags;
						UCOMITypeInfo intType;
						classInfo._typeInfo.GetRefTypeOfImplType(j, out intImpl);
						classInfo._typeInfo.GetRefTypeInfo(intImpl, out intType);
						classInfo._typeInfo.GetImplTypeFlags(j, out implTypeFlags);
						ComInterfaceInfo intInfo = (ComInterfaceInfo)_interfaces[intType];
						if (intInfo != null) {
							// j == 0 indicates the default interface
							classInfo.AddInterface(intInfo, j == 0);
							if ((((IMPLTYPEFLAGS)implTypeFlags) & 
								IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE) != 0)
								intInfo.IsSource = true;
						}
					}
					_classes.Add(classInfo);
					_members.Add(classInfo);
					_memberNames.Add(classInfo.Name, classInfo);
					if (TraceUtil.If(null, TraceLevel.Verbose)) {
						Trace.WriteLine("TypeLib - has type: " + classInfo + " " + typeKind);
					}
				}
			}
			_typeLibInfoRead = true;
		}
		
		internal ComClassInfo GetClassInfoFromCLSID(Guid clsId)
		{
			ReadTypeLibInfo();
			if (TraceUtil.If(this, TraceLevel.Info))
				Trace.WriteLine("GetClassInfo: " + clsId);
			foreach (ComClassInfo clsInfo in _classes) {
				if (clsInfo._guid.Equals(clsId)) {
					if (TraceUtil.If(this, TraceLevel.Info)) {
						TraceUtil.WriteLineInfo(this, "GetClassInfo: found " 
												+ clsInfo);
					}
					return clsInfo;
				}
			}
			throw new Exception("CLSID: " + clsId 
								+ " not found in TypeLib "
								+ this);
		}
		
		// Finds the type of the specified interface or class within
		// the given type library
		internal const bool FIND_CLASS = true; 
		
		internal Type FindTypeByName(String ifClassName, bool findClass)
		{
			Type retType = null;
			TraceUtil.WriteLineInfo(this,
									"FindTypeByName: " + this 
									+ ": " + ifClassName);
			// Prefer the control if it exists
			if (_axAssy != null) {
				String name = _axAssyInfo._nameSpaceName + ".Ax" + ifClassName;
				retType = _axAssy.GetType(name);
				if (retType != null)
					return retType;
			}
			if (_assy != null) {
				String name;
				if (findClass)
					name = _assyInfo._nameSpaceName + "." + ifClassName + "Class";
				else
					name = _assyInfo._nameSpaceName + "." + ifClassName;
				retType = _assy.GetType(name);
			}
			return retType;
		}
		
		// Gets the name from the type and determines the member
		// name that it would be in a type library
		internal String GetMemberName(Type type)
		{
			String typeName = type.Name;
			if (_axAssy == type.Assembly) {
				// Remove the "Ax" prefix
				return typeName.Substring(2);
			} else {
				// FIXME - of course this is gonna cause a problem if
				// an interface happens to end with the word "Class"...  sigh
				if (typeName.EndsWith("Class"))
					typeName = typeName.Substring(0, typeName.Length - 5);
				return typeName;
			}
		}
		
		internal const bool WARN = true;
		
		// Convience method to get the class info corresponding
		// to a given type
		internal static BasicInfo FindMemberByType(Type type, bool warn)
		{
			TypeLibrary typeLib = GetTypeLib(type);
			if (typeLib != null) {
				BasicInfo basicInfo = typeLib.FindMemberByName(typeLib.GetMemberName(type));
				if (warn && basicInfo == null) {
					// This is really a bug, but we don't want to
					// die in an ugly manner
					TraceUtil.WriteLineWarning
						(typeof(HelpLinkHelper),
						"TypeLib - member not found: " 
						+ type.FullName 
						+ " typeLib: " + typeLib);
				}
				return basicInfo;
			}
			return null;
		}
		
		internal override BasicInfo FindMemberByName(String name)
		{
			if (name == null)
				return null;
			// FIXME - of course this is gonna cause a problem if
			// an interface happens to end with the word "_Event"...  sigh
			// Events have a generated interface with the word _Event
			// stuck on the end.
			if (name.EndsWith("_Event"))
				name = name.Substring(0, name.Length - 6);
			ReadTypeLibInfo();
			return base.FindMemberByName(name);
		}
		
		// Returns the underlying type if this is a typedef
		internal String ResolveTypeDef(String typeName, bool comType)
		{
			if (_typeDefHash == null)
				return typeName;
			ComTypeDefInfo typeDef =
				(ComTypeDefInfo)_typeDefHash[typeName];
			if (typeDef == null)
				return typeName;
			if (comType)
				return typeDef._varComType;
			return typeDef._varClrType;
		}
		
		internal void Register()
		{
			ActiveX.RegisterTypeLib(_iTypeLib, _fileName, null);
			PopulateFromRegistry(_typeLibKey);
			_registered = true;
			lock (typeof(TypeLibrary)) {
				_registeredTypeLibs.Add(this, this);
			}
		}
		
		internal void Unregister()
		{
			TraceUtil.WriteLineInfo(this, "Unregister: " + this); 
			ActiveX.UnRegisterTypeLib(ref _typeLibKey._guid,
									  (short)_typeLibKey._majorVersion,
									  (short)_typeLibKey._minorVersion,
									 0,
									 SYSKIND.SYS_WIN32);
			lock (typeof(TypeLibrary)) {
				_registeredTypeLibs.Remove(this);
			}
			_registered = false;
		}
		
		internal static TypeLibKey GetTypeLibKey(TYPELIBATTR typeLibAttr)
		{
			TypeLibKey typeLibKey = new TypeLibKey(typeLibAttr.guid,
				typeLibAttr.wMajorVerNum + "." + typeLibAttr.wMinorVerNum);
			TraceUtil.WriteLineInfo(null, "TypeLibKey: " + typeLibKey);
			return typeLibKey;
		}
		
		internal static TypeLibKey GetTypeLibKey(UCOMITypeLib typeLib)
		{
			TypeLibKey typeLibKey;
			IntPtr typeAttrPtr;
			typeLib.GetLibAttr(out typeAttrPtr);
			TYPELIBATTR typeLibAttr = (TYPELIBATTR)Marshal.PtrToStructure(typeAttrPtr, 
				typeof(TYPELIBATTR));
			typeLibKey = GetTypeLibKey(typeLibAttr);
			// Release it only after we are not going to touch
			// even the TYPELIBATTR structure any more
			typeLib.ReleaseTLibAttr(typeAttrPtr);
			return typeLibKey;
		}
		
		// Creates the versioned names for the assemblies associated
		// with the type library.  
		protected void CreateAssyNames()
		{
			if (_assyInfo._name == null) {
				String dir = ComponentInspectorProperties.ConvertedAssemblyDirectory;
				TraceUtil.WriteLineInfo(this, "CreateAssyNames - using name: " + _name
					+ " _typeLibKey: " + _typeLibKey);
				String baseName = _name + "_"
					+ _typeLibKey._version.Replace(".", "_");
				
				// Fix up the name so it can work as a filename
				baseName = baseName.Replace(":", "_");
				baseName = baseName.Replace(".", "_");
				baseName = baseName.Replace("\\", "_");
				TraceUtil.WriteLineInfo(this, "CreateAssyNames - baseName: " + baseName);
				_assyInfo.SetName(dir, baseName);
				_axAssyInfo.SetName(dir, "Ax" + baseName);
			}
		}
		
		internal void CheckForPrimaryInteropAssy()
		{
			try {
				ReadTypeLibFile();
				// If there is a primary interop assy, just use that and
				// we are done, we assume there are no activeX controls
				// in this assembly - FIXME that might be a bad assumption
				if (_primaryInteropAssyName != null && _primaryInteropAssy == null) {
					_primaryInteropAssy = Assembly.LoadWithPartialName(_primaryInteropAssyName);
					RecordTranslatedAssy(_primaryInteropAssy);
					_assyInfo._name = _name;
					TraceUtil.WriteLineInfo(this,
											"TypeLib - found PrimaryInteropAssy "
											+ _primaryInteropAssy);
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(this,
										  "TypeLib - failed to load " 
										  + "PrimaryInteropAssy: " 
										  + _primaryInteropAssyName
										  + ": " + ex);
				// Continue to try other things
			}
		}
		
		protected void GenerateSource()
		{
			String fileName = _assyInfo._name + ".cs";
			try {
				File.OpenRead(fileName);
				// It worked, so the souce file is there, don't generate
				// another one
				return;
			} catch (FileNotFoundException) {
				// Not found, we create it
			}
			CreateAssyNames();
			ReadTypeLibInfo();
			FileStream outStr = File.Create(fileName);
			Console.WriteLine("Generating source file: " + fileName);
			TextWriter writer = new StreamWriter(outStr);
			CodeCompileUnit compileUnit = new CodeCompileUnit();
			
			// Add TypeLib attribute
			CodeAttributeArgument typeLibArg = new CodeAttributeArgument();
			typeLibArg.Value = new CodePrimitiveExpression(Name);
			compileUnit.AssemblyCustomAttributes.Add
				(new CodeAttributeDeclaration
					("System.Runtime.InteropServices.ImportedFromTypeLib",
					new CodeAttributeArgument[] { typeLibArg }));
			// Add Guid attribute
			CodeAttributeArgument guidArg = new CodeAttributeArgument();
			guidArg.Value = new CodePrimitiveExpression(_typeLibKey._guid.ToString());
			compileUnit.AssemblyCustomAttributes.Add
				(new CodeAttributeDeclaration
					("System.Runtime.InteropServices.Guid",
					new CodeAttributeArgument[] { guidArg }));
			// Key file
			/***
			// FIXME - this is only for testing
			CodeAttributeArgument keyFileArg = new CodeAttributeArgument();
			keyFileArg.Value = new CodePrimitiveExpression
				("\\d\\src\\mstools\\key\\nogoop.snk");
			compileUnit.AssemblyCustomAttributes.Add
				(new CodeAttributeDeclaration
					("System.Reflection.AssemblyKeyFile",
					 new CodeAttributeArgument[] { keyFileArg }));
			***/
			CodeNamespace ns = new CodeNamespace(_assyInfo._name);
			compileUnit.Namespaces.Add(ns);
			foreach (Object cls in _members)
			{
				if (cls is ICodeDom)
					((ICodeDom)cls).AddDomTo(ns.Types);
			}
			CodeDomProvider provider = 
				new Microsoft.CSharp.CSharpCodeProvider();
			ICodeGenerator gen = provider.CreateGenerator();
			gen.GenerateCodeFromCompileUnit(compileUnit,
											writer,
											null);
			writer.Flush();
			writer.Close();
		}
		
		// Checks to see if the translated version is present and
		// current, if its not current it is removed.
		internal static bool IsAssyCurrent(String assyFileName, String typeLibFileName)
		{
			TraceUtil.WriteLineInfo(null, "IsAssyCurrent assy: "
									+ assyFileName
									+ "  TypeLib: "
									+ typeLibFileName);
			// See if there is a translated version already present.
			// We don't know if there should be an Ax version or not,
			// try the normal one and if it succeeds, try the Ax version.
			// If the normal version succeeds however, we can consider
			// that we are done.
			try {
				// Make sure any translated assembly found is newer
				// that the type library.
				DateTime assyDate = File.GetLastWriteTime(assyFileName);
				DateTime typeLibDate = File.GetLastWriteTime(typeLibFileName);
				if (assyDate < typeLibDate) {
					TraceUtil.WriteLineInfo(null, "TypeLib - converted assy "
											+ "expired (normal): " 
											+ assyFileName
											+ " Assembly written: " + assyDate
											+ "  typelib: " + typeLibDate);
					return false;
				}
				return true;
			} catch (Exception ex) {
				TraceUtil.WriteLineInfo(null, "TypeLib - Translate not "
										+ "found/expired (normal): " 
										+ assyFileName
										+ "ex: " + ex);
				// Fall through, we have to do the translation
			}                
			return false;
		}
		
		protected Assembly FindAssemblyFile()
		{
			ErrorDialog.Show
				("You are opening a type library that was generated "
				+ "from the assembly '" + _translatedAssyFullName
				+ "' and I am unable to find this assembly.\n\n"
				+ "A file dialog will appear when you close this "
				+ "dialog to allow you to find it.  If you can't or "
				+ "don't wish to find it, just cancel the file "
				+ "dialog and the open will proceed without the "
				+ "assembly information.",
				"Unable to find " + _translatedAssyFullName);
			bool done = false;
			Assembly assy = null;
			while (!done) {  
				OpenFileDialog ofd = new OpenFileDialog();
				String filterString =
					"Assembly Files (*.exe, *.dll)|*.exe;*.dll";
				filterString += "|All Files (*.*)|*.*";
				ofd.Filter = filterString;
				ofd.FilterIndex = 1 ;
				ofd.RestoreDirectory = true ;
				ofd.Title = "Find Assembly File for " + _name;
	
				DialogResult dr = ofd.ShowDialog();
				if (dr == DialogResult.OK) {
					String fileName = ofd.FileName.ToLower();
					try {
						assy = Assembly.LoadFrom(fileName);
						done = true;
					} catch (Exception ex) {
						ErrorDialog.Show(ex,
							"Error opening file " + fileName,
							"Error opening file " + fileName,
							MessageBoxIcon.Error);
					}
				} else if (dr == DialogResult.Cancel) {
					done = true;
					_dontFindAssy = true;
				}
			}
			return assy;
		}
		
		// This translates the type library into an assembly
		internal void TranslateTypeLib()
		{
			TraceUtil.WriteLineInfo(this, "TranslateTypeLib");
			// Already associated with an assembly
			if (_assy != null)
				return;
			ReadTypeLibFile();
			if (_iTypeLib == null)
				throw new Exception("Failed to open file: " + _fileName);
			// If this is present, we can't translate the assembly,
			// we just have to try and load it using this name.  This 
			// is the case for things like mscorlib.tlb 
			if (_translatedAssyFullName != null) {
				Assembly assy = null;
				try {
					String fileName;
					// We will have the file name if we are coming through
					// here after the type library has been opened previously
					// (when it is being restored from the registry).
					if (_assyInfo._fileName != null)
						fileName = _assyInfo._fileName;
					else
						fileName = _translatedAssyFullName;
					TraceUtil.WriteLineInfo
						(null,
						"TypeLib - translated assy looking for: " 
						+ fileName);
					assy = Assembly.Load(fileName);
				} catch (FileNotFoundException) {
					// Don't ask the user if we have already asked
					// the user
					if (!_dontFindAssy)
						assy = FindAssemblyFile();
				}
			
				TraceUtil.WriteLineInfo(null,
										"TypeLib - source assy is: "
										+ assy
										+ " for translated lib "
										+ _translatedAssyFullName);
				RecordTranslatedAssy(assy);
				return;
			}
			CheckForPrimaryInteropAssy();
			if (_primaryInteropAssyName != null)
				return;
			// Only create the versioned names if the primary interop assy
			// is not used, since that's not versioned.
			CreateAssyNames();
			TraceUtil.WriteLineIf(null, TraceLevel.Info,
								 "TypeLib - converting - url: " 
								 + _assyInfo._url
								 + " file: " + _assyInfo._fileName);
			String dir = ComponentInspectorProperties.ConvertedAssemblyDirectory;
			Directory.CreateDirectory(dir);
			Directory.SetCurrentDirectory(dir);
			if (IsAssyCurrent(_assyInfo._fileName, _fileName)) {
				// See if there is a translated version already present.
				// We don't know if there should be an Ax version or not,
				// try the normal one and if it succeeds, try the Ax version.
				// If the normal version succeeds however, we can consider
				// that we are done.
				RecordTranslatedAssy(Assembly.LoadFrom(_assyInfo._url));
				TraceUtil.WriteLineInfo(null,
										"TypeLib - found previous xlated "
										+ _assyInfo._name);
				try {
					RecordTranslatedAssy(Assembly.LoadFrom(_axAssyInfo._url));
				} catch {
					// Ignore
				}
				return;
			}
			// Convert the type library to an assembly
				
			// The assembly load event happens in here, it is going to
			// be unable to add the assembly to the registry since there
			// is no CodeBase associated with a dynamically created 
			// assembly, but that's not a problem since we add it below
			// when adding the GUID for the typelib
			ProgressDialog progress = new ProgressDialog();
			progress.Setup("Converting " + GetName() 
						   + " to .NET Assembly",
						  "Please wait while I convert " 
						  + GetName()
						   + " to a .NET Assembly",
						  ProgressDialog.NO_PROGRESS_BAR,
						   !ProgressDialog.HAS_PROGRESS_TEXT,
						  ProgressDialog.FINAL);
			progress.ShowIfNotDone();
			try {
				TypeLibConverter converter = new TypeLibConverter();
				AssemblyBuilder asm = converter.ConvertTypeLibToAssembly
					(_iTypeLib, 
					_assyInfo._fileName,
					// Can't do this because we can't strong name these
					 //TypeLibImporterFlags.PrimaryInteropAssembly, 
					0,
					this,
					null, null, null, null);
				// For the problem associated with Q316653
				//FixEventSinkHelpers(asm);
				asm.Save(_assyInfo._fileName);
				RecordTranslatedAssy(Assembly.LoadFrom(_assyInfo._url));
				// Generate the ActiveX wrapper for the typelib, 
				// if that's necessary
				AxImporter.Options impOptions = new AxImporter.Options();
				impOptions.outputDirectory = dir;
				impOptions.outputName = _axAssyInfo._fileName;
				impOptions.references = this;
				impOptions.genSources = true;
				AxImporter axImport = new AxImporter(impOptions);
				try {
					// This converts the type library and generates the
					// wrapper for any ActiveX controls.  It produces 
					// a list of the assemblies that it created.  If there
					// are no ActiveX controls this will throw.
					String wrapper = axImport.
						GenerateFromTypeLibrary(_iTypeLib);
				} catch (Exception ex) {
					if (ex.Message.StartsWith("Did not find a")) {
						TraceUtil.WriteLineInfo(this, 
												"TypeLib - no controls found");
						return;
					}
					TraceUtil.WriteLineWarning(this, 
											  "TypeLib - AxImporter error "
											  + _assyInfo._url + " " 
											  + ex);
					throw new Exception("Error in AxImporter for: "
										+ _assyInfo._url, ex);
				}
				foreach (String aname in axImport.GeneratedAssemblies) {
					TraceUtil.WriteLineInfo(this, "TypeLib - AX gen assy: " 
											+ aname);
				}
				foreach (String sname in axImport.GeneratedSources) {
					TraceUtil.WriteLineInfo(this, "TypeLib - AX gen source: " 
											+ sname);
				}
				RecordTranslatedAssy(Assembly.LoadFrom(_axAssyInfo._url));
				TraceUtil.WriteLineIf(null, TraceLevel.Info,
									 "TypeLib - conversion done: "
									 + _assyInfo._url);
			} finally {
				progress.Finished();
			}
		}

		
		protected void SetAssemblyPointer(Assembly assy)
		{
			if (assy.GetName().Name.Equals("Ax" + _assyInfo._name)) {
				_axAssy = assy;
				_axAssyInfo.SetNamespace(assy);
			} else {
				_assy = assy;
				_assyInfo.SetNamespace(assy);
			}
			lock (_assyToTypeLibMap) {
				if (_assyToTypeLibMap[assy.FullName] == null)
					_assyToTypeLibMap.Add(assy.FullName, this);
			}
			if (_primaryInteropAssy != null) {
				String key = _primaryInteropAssy.GetName().ToString();
				lock (_primaryInteropTypeLibs) {
					if (_primaryInteropTypeLibs[key] == null)
						_primaryInteropTypeLibs.Add(key, this);
				}
			}
		}
		
		protected void MarkConverted()
		{
			lock (typeof(TypeLibrary)) {
				// Record as converted if not already done
				if (!_converted) {
					TraceUtil.WriteLineInfo(this,
											"TypeLib - MarkConverted " 
											+ this);
					_converted = true;
				}
			}
		}
		
		// This records a translated assembly.  There may be multiple
		// assemblies translated for a given type library.
		protected void RecordTranslatedAssy(Assembly assy)
		{
			TraceUtil.WriteLineInfo(this, "TypeLib - RecordTranslatedAssy: "
									+ (assy != null ? assy.CodeBase : "null"));
			if (assy != null) {
				MarkConverted();
				SetAssemblyPointer(assy);
				// Don't remember this stuff if searching
				if (!_searchMode) {
					// Make sure this is in the assembly tree and hooked to the
					// type lib
					AssemblySupport.AddAssy(assy, this);
				}
			}    
			// Don't remember this stuff if searching
			if (!_searchMode) {
				// We have to record the version of any typelib
				// the user has opened since that's not recorded
				// anywhere in the assembly
				RememberMe();
				ComSupport.AddTypeLib(this);
			}
		}
		
		// Saves the information about this typelib in the registr
		// so it will get restored
		internal void RememberMe()
		{
			if (!_remembered) {
				if (Assy == null || _fileName == null) {
					TraceUtil.WriteLineWarning
						(this, "Cant remember " + this +
						" when either there is no file, or not assembly");
					return;
				}
				try	{
					ComSupport.RememberTypeLib(this);
					// Associate the assembly with the typelib information
					AssemblySupport.RememberAssembly
						(Assy, 
						Utils.MakeGuidStr(Key._guid),
						Key._version);
					_remembered = true;
					TraceUtil.WriteLineInfo(this, "Remebered: " + this);
				} catch (Exception ex) {
					TraceUtil.WriteLineWarning
						(this, "Cant remember " + this + " because: " + ex);
				}                    
			}
		}
		
		internal void ForgetMe()
		{
			// Always force forgetting even if things get messed up
			if (Assy != null)
				AssemblySupport.ForgetAssembly(Assy);
			if (AxAssy != null)
				AssemblySupport.ForgetAssembly(AxAssy);
			ComSupport.ForgetTypeLib(this);
			_remembered = false;
			TraceUtil.WriteLineInfo(this, "Forgotten: " + this);
		}
		
		public void ReportEvent(ImporterEventKind eventKind, int eventCode, string eventMsg)
		{
			// FIXME - handle warning event here...
			TraceUtil.WriteLineWarning(this, 
									  "Typelib Convert warning: " + this
									  + " " + eventKind + " " + eventMsg);
		}
		
		public String ResolveActiveXReference(UCOMITypeLib typeLib)
		{
			TraceUtil.WriteLineInfo(this,
								 "TypeLib - ResolveActiveXRef: " 
								 + typeLib);
			return null;
		}   
		
		public String ResolveComReference(AssemblyName assyName)
		{
			String refStr = assyName.Name + ".dll";
			TypeLibrary typeLib = 
				(TypeLibrary)_primaryInteropTypeLibs[assyName.ToString()];
			if (typeLib != null)
				refStr = typeLib._assy.Location;
			TraceUtil.WriteLineInfo(this,
								 "TypeLib - ResolveCOMRef (name): " 
								 + assyName + " ret: " + refStr);
			return refStr;
		}   
		
		public String ResolveComReference(UCOMITypeLib typeLib)
		{
			String refStr = ResolveRef(typeLib)._assyInfo._fileName;
			TraceUtil.WriteLineInfo(this,
								 "TypeLib - ResolveCOMRef (typeLib): " 
								 + typeLib + " ret: " + refStr);
			return refStr;
		}   
		
		public String ResolveManagedReference(String assyName)
		{
			String refStr = assyName + ".dll";
			TraceUtil.WriteLineInfo(this,
								 "TypeLib - ResolveManagedRef: " 
								 + assyName + " ret: " + refStr);
			return refStr;
		}   
		
		public Assembly ResolveRef(Object typeLib)
		{
			return ResolveRef((UCOMITypeLib)typeLib)._assy;
		}   
		
		protected TypeLibrary ResolveRef(UCOMITypeLib iTypeLib)
		{
			TraceUtil.WriteLineIf(null, TraceLevel.Info,
								 "TypeLib - resolving: " + iTypeLib);
			TypeLibrary lib = GetTypeLib(iTypeLib);
			
			TraceUtil.WriteLineInfo(null,
								 "TypeLib - resolved: " + lib 
									+ " assy: " + lib._assy);
			return lib;
		}   
		
		protected void PopulateFromRegistry(TypeLibKey typeLibKey)
		{
			TraceUtil.WriteLineInfo(null, "TypeLib - PopulateFromRegistry");
			String guidStr = Utils.MakeGuidStr(typeLibKey._guid);
			_regKey = Windows.KeyTypeLib.OpenSubKey(guidStr);
			if (_regKey == null) {
				throw new Exception("Type lib not found in registry for guid: "
									+ guidStr);
			}
			RegistryKey versionKey;
			if (typeLibKey._version != null) {
				versionKey = _regKey.OpenSubKey(typeLibKey._version);
				if (versionKey == null) {
					throw new Exception("Version entry " 
								 + typeLibKey._version
								 + " not found for typelib: " 
								 + guidStr);
				}
			} else {
				String[] versions = _regKey.GetSubKeyNames();
				if (versions.Length == 0) {
					throw new Exception("No version keys found for typelib: " + guidStr);
				}
				// Just use the first version
				versionKey = _regKey.OpenSubKey(versions[0]);
			}
			PopulateFromRegistry(versionKey);
		}
		
		protected void PopulateFromRegistry(RegistryKey versionKey)
		{
			DocString = (String)versionKey.GetValue(null);
			try	{
				_primaryInteropAssyName = (String)versionKey.GetValue("PrimaryInteropAssemblyName");
			} catch { }
				
			// Locate is under the version key
			String[] localeNames = versionKey.GetSubKeyNames();
			if (localeNames.Length == 0) {
				throw new Exception("No locales for typelib: " + versionKey);
			}
			// Just use the first locale
			// FIXME - we should handle more than one locale
			RegistryKey localeKey = versionKey.OpenSubKey(localeNames[0]);
			if (localeKey == null) {
				throw new Exception("Locale entry: " + localeNames[0]
									+ " could not be read for typelib");
			}
			// FIXME - win64, etc
			RegistryKey detailKey = localeKey.OpenSubKey("win32");
			if (detailKey == null) {
				throw new Exception("win32 key not found for typelib");
			}
			_fileName = (String)detailKey.GetValue(null);
			detailKey = versionKey.OpenSubKey("FLAGS");
			if (detailKey != null) {
				_flags = (uint)Int32.Parse((String)detailKey.GetValue(null));
			}
			detailKey = versionKey.OpenSubKey("HELPDIR");
			if (detailKey != null) {
				_helpDir = (String)detailKey.GetValue(null);
			}
			_registered = true;
		}
		
		internal void Close()
		{
			// FIXME - close the file?
			if (_assy != null) {
				AssemblySupport.CloseAssembly(_assy);
			}
			if (_axAssy != null) {
				AssemblySupport.CloseAssembly(_axAssy);
			}
			lock (typeof(TypeLibrary)) {
				_registeredTypeLibs.Remove(this);
				if (_openedTypeLibs[_typeLibKey] != null) {
					_openedTypeLibs.Remove(_typeLibKey);
				}
				_openedFile = false;
				_converted = false;
			}
			ComSupport.ForgetTypeLib(this);
		}
		
		public const bool SHOW_ASSY = true;
		
		public void GetDetailText(bool showAssy)
		{
			// We defer reading the type lib information 
			// until its really needed
			ReadTypeLibFile();
			DetailPanel.AddLink(_infoType,
								!ObjectBrowser.INTERNAL,
								10,
								ComLinkHelper.CLHelper,
								this,
								HelpLinkHelper.MakeLinkModifier
								(_helpFile, 0));
			GetBasicDetailText();
			if (showAssy) {
				if (_assy != null) {
					AssemblySupport.GetDetailText(_assy);
				}
				if (_axAssy != null) {
					DetailPanel.Add("Ax Assembly Full Name", 
									!ObjectBrowser.INTERNAL,
									112,
									_axAssy.FullName);
					try	{
						DetailPanel.Add("Ax Assembly Location", 
										!ObjectBrowser.INTERNAL,
										114,
										_axAssy.Location);
					} catch {
						// This is not supported for dynamic assemblies
						// and it throws (sadly)
					}
				}
			}
			DetailPanel.Add("TypeLib Version", 
							!ObjectBrowser.INTERNAL,
							15,
							_typeLibKey._version);
			DetailPanel.Add("TypeLib Guid",
							!ObjectBrowser.INTERNAL,
							21,
							Utils.MakeGuidStr(Key._guid));
			if (_primaryInteropAssy != null) {
				DetailPanel.Add("Primary Interop Assy", 
								!ObjectBrowser.INTERNAL,
								120,
								_primaryInteropAssy.FullName);
			}
			if (_translatedAssyFullName != null) {
				DetailPanel.Add("Exported from Assy", 
								!ObjectBrowser.INTERNAL,
								121,
								_translatedAssyFullName);
			}
			DetailPanel.Add("TypeLib File", 
							!ObjectBrowser.INTERNAL,
							180,
							_fileName);
			DetailPanel.Add("Assembly Name", 
							ObjectBrowser.INTERNAL,
							190,
							_assyInfo._name);
			if (_helpDir != null) {
				DetailPanel.Add("Help Dir", 
								ObjectBrowser.INTERNAL,
								220,
								_helpDir);
			}
		}
		
		internal override String GetName()
		{
			return base.GetName(PREFER_DESCRIPTION);
		}
		
		public override String ToString()
		{
			if (_typeLibKey != null) {
				return GetName() + " " + _typeLibKey._guid + " " + _typeLibKey._version;
			}
			return GetName();
		}
		
		public override int CompareTo(Object other)
		{
			if (!(other is TypeLibrary)) {
				return -1;
			}
			TypeLibrary b = (TypeLibrary)other;
			if (DocString != null && b.DocString != null)
			{
				int compVal = DocString.CompareTo(b.DocString);
				if (compVal != 0) {
					return compVal;
				}
			}
			// Key is null only when this is a virgin object
			if (_typeLibKey == null)
			{
				if (b._typeLibKey == null) {
					return 0;
				}
				return -1;
			}
			return _typeLibKey.CompareTo(b._typeLibKey);
		}
		
		public override int GetHashCode()
		{
			if (_typeLibKey == null) {
				return 0;
			}
			return _typeLibKey.GetHashCode();
		}
		
		public override bool Equals(Object other)
		{
			int result;
			if (!(other is TypeLibrary)) {
				return false;
			}
			result = CompareTo(other);
			if (result == 0) {
				return true;
			}
			return false;
		}
	}
}

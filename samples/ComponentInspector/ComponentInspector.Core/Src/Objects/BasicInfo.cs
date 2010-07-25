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
using NoGoop.ObjBrowser.LinkHelpers;
using NoGoop.ObjBrowser.Panels;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.Obj
{
	internal class BasicInfo : IComparable, ILinkTarget, ISearchNode, ICodeDom
	{
		internal Guid               _guid;
		internal String             _guidStr;
		
		// The official name of the object; this is the name to be
		// used when looking this object up.  For example, for a CoClass
		// its the name of the class in the type library
		protected String             _name;
		
		// The name that is preferred to be displayed as the name
		// in a tree view.  This is typically the official name with some
		// additional annotation.
		protected String             _printName;
		
		// A description of the object, this is normally obtained from
		// the registry or from GetDocumenation(), which probably just
		// gets it from the registry
		protected String             _docString;
		internal TYPEKIND           _typeKind;
		internal UCOMITypeInfo      _typeInfo;
		internal UCOMITypeLib       _iTypeLib;
		internal int                _helpContext;
		internal String             _helpFile;
		internal RegistryKey        _regKey;
		
		// The containing object (TypeLibrary contains Classes, Interfaces, etc)
		internal BasicInfo          _container;
		
		// The containing type library object
		protected TypeLibrary        _typeLib;
		
		// A string that identifies the name of the subclass
		// of BasicInfo, similar in principal to the member type;
		// Used for the Name in GetDetailText to show what kind
		// of thing this is
		internal String             _infoType;
		
		// The tree node that corresponds to this information node;
		// this is used to provide link functionality
		protected BrowserTreeNode    _treeNode;
		
		// The presentation info, at this level because we need
		// the icon information for a search (GetImageIndex)
		protected PresentationInfo   _presInfo;
		protected ArrayList          _members;
		
		// K(name of member) V(BasicInfo of member)
		protected Hashtable          _memberNames;
		protected CodeObject        _codeDom;
		
		// Used during GetDetailText() to indicate the identifying
		// information associated with this type has been displayed.
		protected bool              _displayedIdentifier;
		
		internal virtual String Name {
			get {
				return _name;
			}
			set {
				if (value == null)
					throw new NullReferenceException();
				_name = value;
			}
		}      
		
		internal virtual String PrintName {
			get {
				return _printName;
			}
			set {
				if (value == null)
					throw new NullReferenceException();
				_printName = value;
			}
		}      
		
		internal String DocString {
			get {
				return _docString;
			}
			set {
				if (value == null && _name == null)
					throw new NullReferenceException();
				_docString = value;
			}
		}       
		
		internal BrowserTreeNode TreeNode {
			get {
				return _treeNode;
			}
			set {
				_treeNode = value;
			}
		}       
		
		internal TypeLibrary TypeLib {
			get {
				return _typeLib;
			}
			set {
				_typeLib = value;
			}
		}      
		
		internal virtual PresentationInfo PresInfo {
			get {
				return _presInfo;
			}
		}
		
		internal virtual ArrayList Members {
			get {
				return _members;
			}
		}
		
		internal virtual Hashtable MemberNames {
			get {
				return _memberNames;
			}
		}
		
		internal BasicInfo()
		{
			_members = new ArrayList();
		}
		
		internal BasicInfo(String name) : this()
		{
			Name = name;
		}
		
		internal BasicInfo(RegistryKey regKey) : this()
		{
			_regKey = regKey;
		}
		
		internal BasicInfo(RegistryKey regKey, String guidStr) : this(regKey)
		{
			InitGuid(guidStr, new Guid(guidStr));
		}
		
		// Used for member subclasses
		internal BasicInfo(UCOMITypeInfo typeInfo) : this()
		{
			_typeInfo = typeInfo;
		}
		
		// Used for classes that are classes, interfaces
		// where the type library information is not yet known
		internal BasicInfo(String name, 
						   String infoType,
						   String guidStr) : this(name)
		{
			_infoType = infoType;
			InitGuid(guidStr, new Guid(guidStr));
		}
		
		// Used for subclassses that are classes, objects and interfaces
		// Where the type library information is known
		internal BasicInfo(TypeLibrary typeLib, 
						   TYPEKIND typeKind,
						   int index) : this()
		{
			Setup(typeLib, typeKind, index, null, Guid.Empty);
		}
		
		// Used for subclassses that are classes, objects and interfaces
		// Where the type library information is known
		internal BasicInfo(TypeLibrary typeLib, 
						   TYPEKIND typeKind,
						   int index,
						   UCOMITypeInfo typeInfo,
						   Guid guid) : this()
		{
			Setup(typeLib, typeKind, index, typeInfo, guid);
		}
		
		~BasicInfo()
		{
			if (_typeInfo != null)
				Marshal.ReleaseComObject(_typeInfo);
			if (_iTypeLib != null)
				Marshal.ReleaseComObject(_iTypeLib);
		}
		
		// ICodeDom
		public virtual void AddDomTo(IList parent)
		{
			// On demand
			if (_codeDom == null)
				_codeDom = GenerateCodeDom();
			if (_codeDom != null)
				parent.Add(_codeDom);
		}
		
		protected virtual CodeObject GenerateCodeDom()
		{
			// Subclassed
			return null;
		}
		
		// ISearchNode interfaces
		public virtual ISearchMaterializer GetSearchMaterializer
			(ISearcher searcher)
		{
			return new ComSearchMaterializer((Stack)searcher.SearchStack.Clone());
		}
		
		public virtual int GetImageIndex()
		{
			if (_presInfo != null)
				return _presInfo._iconIndex;
			return -1;
		}
		
		public virtual String GetSearchNameString()
		{
			// Prefer name in a search, if available
			if (Name != null)
				return Name;
			if (PrintName != null)
				return PrintName;
			return DocString;
		}
		
		public virtual String GetSearchValueString()
		{
			return null;
		}
		
		public virtual bool HasSearchChildren(ISearcher searcher)
		{
			if (_members != null && _members.Count > 0)
				return true;
			return false;
		}
		
		public virtual ICollection GetSearchChildren()
		{
			return _members;
		}
		
		protected void InitGuid(String guidStr, Guid guid)
		{
			if (guidStr != null && 
				!guidStr.Equals("") &&
				!guidStr.Equals("{00000000-0000-0000-0000-000000000000}"))
			{
				_guidStr = guidStr;
				_guid = guid;
			}
			else
			{
				_guidStr = null;
				_guid = Guid.Empty;
			}
		}
		
		protected void InitGuid(Guid guid)
		{
			InitGuid(Utils.MakeGuidStr(guid), guid);
		}
		
		public String GetLinkName(Object linkModifier)
		{
			return Name;
		}
		
		// When a link is clicked to this
		public void ShowTarget(Object linkModifier)
		{
			if (_treeNode != null)
				_treeNode.PointToNode();
		}
		
		internal virtual void Setup(TypeLibrary typeLib, 
									TYPEKIND typeKind,
									int index)
		{
			Setup(typeLib, typeKind, index, null, Guid.Empty);
		}
		
		internal virtual void Setup(TypeLibrary typeLib, 
									TYPEKIND typeKind,
									int index,
									UCOMITypeInfo typeInfo,
									Guid guid)
		{
			_typeLib = typeLib;
			_iTypeLib = typeLib.ITypeLib;
			if (typeInfo != null)
				_typeInfo = typeInfo;
			else
				_iTypeLib.GetTypeInfo(index, out _typeInfo);
			if (!guid.Equals(Guid.Empty))
				InitGuid(guid);
			else
				InitGuid(GuidFromTypeInfo(_typeInfo));
				
			_typeKind = typeKind;
			_presInfo = PresentationMap.GetInfo(_typeKind);
			GetDocumentation(index);
			if (TraceUtil.If(this, TraceLevel.Info))
				Trace.WriteLine(this, "Basic: " + typeKind + " " + this);
		}
		
		protected void GetDocumentation(int index)
		{
			String docString;
			_iTypeLib.GetDocumentation(index, out _name, 
									   out docString, out _helpContext, 
									   out _helpFile);
			// Set doc string using the property so that it is checked
			// properly for null
			DocString = docString;
		}
		
		internal virtual BasicInfo FindMemberByName(String name)
		{
			if (name == null)
				return null;
			if (_memberNames == null)
				return null;
			return (BasicInfo)_memberNames[name];
		}
		
		// Return the CLR type name associated with this type,
		// if known.  It can only be known if the interface is associated
		// with a type library
		internal String GetCLRTypeName()
		{
			if (TypeLib != null)
				return TypeLib.AssyName + "." + Name;
			return null;
		}
		
		// Can't return the TYPEATTR struct because there is no
		// way to free it, better to have methods get the parts that
		// are needed.
		internal static Guid GuidFromTypeInfo(UCOMITypeInfo typeInfo)
		{
			Guid guid;
			TYPEATTR typeAttr;
			IntPtr typeAttrPtr;
			typeInfo.GetTypeAttr(out typeAttrPtr);
			typeAttr = 
				(TYPEATTR)Marshal.PtrToStructure(typeAttrPtr, 
												 typeof(TYPEATTR));
			guid = typeAttr.guid;
			typeInfo.ReleaseTypeAttr(typeAttrPtr);
			return guid;
		}
		
		public virtual void GetDetailText()
		{
			String displayName = _name;
			if (displayName == null)
				displayName = DocString;
			GetBasicDetailText();
			if (!_displayedIdentifier) {
				DetailPanel.Add(_infoType,
								!ObjectBrowser.INTERNAL,
								10,
								displayName);
			}
		}
		
		protected void AddRegistryValue(String key, string value)
		{
			// This is covered in ComClassInfo
			if (key.Equals("ProgID"))
				return;
			// Don't show a key that refers to a typelib if we
			// already have a link to it
			if (_container is TypeLibrary && 
				key.Equals("TypeLib"))
				return;
			DetailPanel.Add(key,
							!ObjectBrowser.INTERNAL,
							80,
							value);
		}
		
		internal void GetBasicDetailText()
		{
			// The type library Guid comes from its key
			if (!(this is TypeLibrary) && _guidStr != null) {
				DetailPanel.Add(_infoType + " Guid",
								!ObjectBrowser.INTERNAL,
								21,
								_guidStr);
			}
			
			// Get the CLR type if its available
			_displayedIdentifier = false;
			if (_container is TypeLibrary) {
				TypeLibrary typeLib = (TypeLibrary)_container;
				bool findClass;
				if (this is ComClassInfo)
					findClass = TypeLibrary.FIND_CLASS;
				else
					findClass = !TypeLibrary.FIND_CLASS;
				Type type = typeLib.FindTypeByName(_name, 
												   findClass);
				if (type != null) {
					DetailPanel.AddLink("CLR Type",
										!ObjectBrowser.INTERNAL,
										15,
										TypeLinkHelper.TLHelper,
										type,
										HelpLinkHelper.
										MakeLinkModifier(type));
					ComObjectInfo.GetDetailAxType(type, null, _infoType, 10);
					_displayedIdentifier = true;
				} else {
					DetailPanel.AddLink("Type Library",
										!ObjectBrowser.INTERNAL,
										25,
										ComLinkHelper.CLHelper,
										_container,
										HelpLinkHelper.MakeLinkModifier
										(_container._helpFile, 0));
				}
			}
			
			if (_helpFile != null) {
				DetailPanel.AddLink("Help File",
									!ObjectBrowser.INTERNAL,
									225,
									HelpLinkHelper.HLHelper,
									HelpLinkHelper.MakeLinkModifier
									(_helpFile, 0));
				if (_helpContext != 0) {
					DetailPanel.AddLink("Help Context",
										!ObjectBrowser.INTERNAL,
										227,
										HelpLinkHelper.HLHelper,
										HelpLinkHelper.MakeLinkModifier
										(_helpFile, _helpContext));
				}
			}
			
			// Type library description is displayed in the type library stuff
			if (DocString != null && !(this is TypeLibrary)) {
				DetailPanel.Add("Description",
								!ObjectBrowser.INTERNAL,
								23,
								DocString);
			}
			
			// Show everything in the registry
			if (!(this is TypeLibrary) && _regKey != null) {
				try {
					string[] keys = _regKey.GetSubKeyNames();
					foreach (String str in keys) {
						RegistryKey key = _regKey.OpenSubKey(str);
						string value = (string)key.GetValue(null);
						// If there is no value, see if there are any subkeys
						if (value == null || value.Equals("")) {
							String[] subKeys = key.GetSubKeyNames();
							if (subKeys.Length > 0) {
								foreach (string subStr in subKeys)
									AddRegistryValue(str, subStr);
							} else {
								AddRegistryValue(str, null);
							}
						} else {
							AddRegistryValue(str,(string)key.GetValue(null));
						}
					}
				} catch {
					// ignore errors
				}
			}
		}
		
		internal virtual Object GetSortKey()
		{
			return this;
		}
		
		internal virtual String GetName()
		{
			return GetName(!PREFER_DESCRIPTION);
		}
		
		internal const bool PREFER_DESCRIPTION = true;
		
		// Gets the name to be used when for a tree view
		internal virtual String GetName(bool preferDescription)
		{
			if (preferDescription && DocString != null)
				return DocString;
			if (PrintName != null)
				return PrintName;
			if (Name != null)
				return Name;
			if (DocString != null)
				return DocString;
			return String.Empty;
		}
		
		public override string ToString()
		{
			return _name + " " + _guidStr;
		}
		
		public virtual int CompareTo(Object other)
		{
			if (!(other is BasicInfo))
				return -1;
			BasicInfo b = (BasicInfo)other;
			if (_name != null && 
				b != null &&
				b._name != null)
				return _name.CompareTo(b._name);
			return -1;
		}
	}
}

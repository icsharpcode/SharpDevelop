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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Win32;
using NoGoop.Controls;
using NoGoop.ObjBrowser;
using NoGoop.Util;

namespace NoGoop.Obj
{
	// There is a single object per interface, which is maintained
	// in a global hash.
	internal class ComInterfaceInfo : BasicInfo
	{
		// This is either a dispatch interface or a dual interfac
		protected bool                  _dispatch;
		protected bool                  _dual;
		
		// Indicates this is has the source flag set by some class
		// which means its effectively an event interface
		protected bool                  _isSource;
		
		// This flag is present to indicate that the interface
		// has been resolved, even though the type library
		// may not have been found
		//protected bool                  _resolved;
		// The number of parent interfaces this interface has.
		protected int                   _parentCount;
		
		// This is used to make sure we only have one copy of the
		// interface definiton per interface.  These are created
		// as type libraries are read.  They are later resolved
		// when the interfaces are being accessed
		// K(Guid) V(InterfaceInfo)
		protected static Hashtable      _interfacesByGuid;
		internal String                 _typeLibString;
		internal String                 _typeLibVersion;
		
		internal bool IsDispatch
		{
			get
				{
					return _dispatch;
				}
		}    
		
		internal bool IsSource
		{
			get
				{
					return _isSource;
				}
			set
				{
					_isSource = value;
					if (_isSource)
					{
						_infoType = "Event Interface";
						_presInfo = PresentationMap.
							GetInfo(PresentationMap.COM_EVENTINT);
					}
				}
		}   
		
		internal int ParentCount
		{
			get
				{
					return _parentCount;
				}
		}
		
		internal static ICollection AllInterfaces
		{
			get
				{
					return _interfacesByGuid.Values;
				}
		}
		
		static ComInterfaceInfo()
		{
			_interfacesByGuid = new Hashtable();
		}
		
		protected void Init()
		{
			_infoType = "Interface";
			_memberNames = new Hashtable();
		}
		
		internal ComInterfaceInfo()
		{
			// Setup must be called if this one is used
		}
		
		// Constructor when reading from the registry "classes" section
		internal ComInterfaceInfo(RegistryKey classKey,
								 String guidStr) : 
				base(classKey, guidStr)
		{
			Init();
			Name = (String)classKey.GetValue(null);
			_typeKind = TYPEKIND.TKIND_INTERFACE;
			_presInfo = PresentationMap.GetInfo(_typeKind);
			// See if there is type lib information (note this 
			// is different than the way TypeLibs work in the class node)
			RegistryKey key = _regKey.OpenSubKey("TypeLib");
			if (key != null)
			{
				_typeLibString = (String)key.GetValue(null);
				_typeLibVersion = (String)key.GetValue("Version");
			}
		}
		
		// Constructor from creating withing a typelib
		internal override void Setup(TypeLibrary typeLib, 
									TYPEKIND typeKind,
									int index)
		{
			base.Setup(typeLib, typeKind, index);
			Init();
			_container = typeLib;
			TYPEATTR typeAttr;
			IntPtr typeAttrPtr;
			_typeInfo.GetTypeAttr(out typeAttrPtr);
			typeAttr = 
				(TYPEATTR)Marshal.PtrToStructure(typeAttrPtr, 
												typeof(TYPEATTR));
			if (typeKind == TYPEKIND.TKIND_DISPATCH)
			{
				_infoType = "Dispatch Interface";
				_dispatch = true;
			}
			else
			{
				_infoType = "Interface";
			}
			if ((typeAttr.wTypeFlags & TYPEFLAGS.TYPEFLAG_FDUAL) != 0)
			{
				_infoType = "Dual Interface";
				_dispatch = true;
				_dual = true;
			}
			// Members
			for (int i = 0; i < typeAttr.cFuncs; i++)
			{
				// Some members for a dispatch interface are not added
				// because they are the inherited members from the
				// IDispatch interface
				ComMemberInfo mi = ComMemberInfo.
					MakeComMemberInfo(this,
									 typeKind,
									 _typeInfo,
									 i,
									 _dispatch,
									 _dual);
				if (mi != null)
				{
					_members.Add(mi);
					_memberNames.Add(mi.NameKey, mi);
				}
			}
			// Inherited interfaces
			for (int i = 0; i < typeAttr.cImplTypes; i++)
			{
				int href;
				int refTypeIndex;
				UCOMITypeInfo refTypeInfo;
				UCOMITypeLib refITypeLib;
				TypeLibrary refTypeLib;
				try
				{
					_typeInfo.GetRefTypeOfImplType(i, out href);
					_typeInfo.GetRefTypeInfo(href, out refTypeInfo);
					refTypeInfo.GetContainingTypeLib(out refITypeLib,
													out refTypeIndex);
					refTypeLib = TypeLibrary.GetTypeLib(refITypeLib);
					ComInterfaceInfo mi = 
						ComInterfaceInfo.GetInterfaceInfo(refTypeLib,
														 typeAttr.typekind,
														 refTypeIndex);
					if (TraceUtil.If(this, TraceLevel.Verbose))
						Trace.WriteLine("  inherit: " + mi);
					_members.Add(mi);
					_parentCount += 1 + mi.ParentCount;
					// Don't set the typelib on the member as multiple
					// typelibs may refer to the same interface
					mi._container = this;
				}
				catch (Exception ex)
				{
					ErrorDialog.Show
						(ex,
						"Warning - this error was detected when attempting "
						+ "to find an ancestor of the interface "
						+ _name
						+ ".  This is normal in the case where the type "
						+ "library containing that interface "
						+ "is not available.  "
						+ "In other situations this might be a bug and "
						+ "should be reported.",
						"Warning - Cannot Access Inherited Interface",
						MessageBoxIcon.Warning);
				}
			}
			if (_dual)
			{
				_printName = (String)_name.Clone();
				_printName += " (Dual)";
			}
			else
			{
				_printName = _name;
			}
			_typeInfo.ReleaseTypeAttr(typeAttrPtr);
		}
		
		protected override CodeObject GenerateCodeDom()
		{
			CodeTypeDeclaration codeDom = 
				CreateTypeCodeDom(this, _members, !DOCLASS);
			return codeDom;
		}
		
		internal const bool DOCLASS = true;
		
		// Applies to classes and interface, and since we don't have
		// a common superclass, just put it here and make it static
		internal static CodeTypeDeclaration CreateTypeCodeDom
			(BasicInfo basicInfo,
			ArrayList interfaces,
			bool doClass)
		{
			CodeTypeDeclaration codeDom = new CodeTypeDeclaration();
			Guid useGuid = basicInfo._guid;
			if (basicInfo is ComClassInfo && doClass)
			{
				codeDom.Name = basicInfo.Name + "Class";
				codeDom.IsInterface = false;
			}
			else
			{
				// Use the default interface's class if we are generating
				// the interface for the class
				if (basicInfo is ComClassInfo)
					useGuid = ((ComClassInfo)basicInfo).DefaultInterface._guid;
				codeDom.Name = basicInfo.Name;
				codeDom.IsInterface = true;
				if (basicInfo is ComInterfaceInfo)
				{
					ComInterfaceInfo ifaceInfo = (ComInterfaceInfo)basicInfo;
					ComInterfaceType ifaceType;
					if (ifaceInfo._dual)
						ifaceType = ComInterfaceType.InterfaceIsDual;
					else if (ifaceInfo._dispatch)
						ifaceType = ComInterfaceType.InterfaceIsIDispatch;
					else 
						ifaceType = ComInterfaceType.InterfaceIsIUnknown;
					// Add InterfaceType attribute
					CodeAttributeArgument ifaceTypeArg = new CodeAttributeArgument();
					ifaceTypeArg.Value = new CodeSnippetExpression
						("(System.Runtime.InteropServices.ComInterfaceType)" 
						+ (int)ifaceType);
					codeDom.CustomAttributes.Add
						(new CodeAttributeDeclaration
							("System.Runtime.InteropServices.InterfaceType",
							new CodeAttributeArgument[] { ifaceTypeArg }));
				}
			}
			// Add Guid attribute
			CodeAttributeArgument guidArg = new CodeAttributeArgument();
			// The _guidStr has braces which the attribute can't handle
			guidArg.Value = new CodePrimitiveExpression
				(useGuid.ToString());
			codeDom.CustomAttributes.Add
				(new CodeAttributeDeclaration
					("System.Runtime.InteropServices.Guid",
					new CodeAttributeArgument[] { guidArg }));
			if (basicInfo is ComClassInfo)
			{
				ComClassInfo classInfo = (ComClassInfo)basicInfo;
				if (doClass)
				{
					codeDom.TypeAttributes |= TypeAttributes.Abstract;
					// Class inherits from the interface of the same name
					codeDom.BaseTypes.Add
						(new CodeTypeReference(basicInfo.Name));
					// Add TypeLibType attribute
					CodeAttributeArgument typeLibTypeArg = new CodeAttributeArgument();
					typeLibTypeArg.Value = new CodePrimitiveExpression
						((int)classInfo._typeFlags);
					codeDom.CustomAttributes.Add
						(new CodeAttributeDeclaration
							("System.Runtime.InteropServices.TypeLibType",
							new CodeAttributeArgument[] { typeLibTypeArg }));
					// Add ClassInterface attribute
					CodeAttributeArgument classIfaceArg = new CodeAttributeArgument();
					classIfaceArg.Value = new CodeSnippetExpression("(short)0");
					codeDom.CustomAttributes.Add
						(new CodeAttributeDeclaration
							("System.Runtime.InteropServices.ClassInterface",
							new CodeAttributeArgument[] { classIfaceArg }));
					// Make the no-arg constructor public (default cons 
					// visibility is family)
					CodeConstructor cons = new CodeConstructor();
					cons.Attributes = MemberAttributes.Public;
					codeDom.Members.Add(cons);
				}
				else
				{
					// Add CoClass attribute
					CodeAttributeArgument typeArg = new CodeAttributeArgument();
					typeArg.Value = new CodeTypeReferenceExpression
						(new CodeTypeReference("typeof("
											  + basicInfo.GetCLRTypeName()
											   + ")"));
					codeDom.CustomAttributes.Add
						(new CodeAttributeDeclaration
							("System.Runtime.InteropServices.CoClass",
							new CodeAttributeArgument[] { typeArg }));
				}
			}
			if (TraceUtil.If(basicInfo, TraceLevel.Info))
			{
				Trace.WriteLine(basicInfo, "CG - " 
								+ basicInfo.Name);
			}
			bool firstTime = true;
			// Add inherited interfaces
			foreach (BasicInfo member in interfaces)
			{
				if (member is ComInterfaceInfo)
				{
					// FIXME - should probably compare these by GUID
					if (member.Name.Equals("IUnknown") ||
						member.Name.Equals("IDispatch"))
						continue;
					codeDom.BaseTypes.Add
						(new CodeTypeReference(member.Name));
					// Add the members for the class or interface, 
					// its all of the members
					// of each implemented interface
					if ((!codeDom.IsInterface && doClass) || 
						codeDom.IsInterface)
					{
						// Qualify subsequent interfaces with the
						// interface name
						// Unless this is an interface, then just
						// leave the name alone
						if (firstTime || codeDom.IsInterface)
						{
							AddMembers(codeDom, member.Members, null);
						}
						else
						{
							AddMembers(codeDom, member.Members, 
									  member.Name + "_");
						}
					}
					firstTime = false;
				}
			}
			// Don't add the members for the class interface
			if (basicInfo is ComClassInfo && !doClass)
				return codeDom;
			// Add members for this interface (a class has none)
			AddMembers(codeDom, basicInfo.Members, null);
			return codeDom;
		}
		
		// Add the list of members to the code type
		protected static void AddMembers(CodeTypeDeclaration codeDom,
										ArrayList members,
										String prefix)
										
		{
			// Add methods/properties
			foreach (Object member in members)
			{
				if (member is ComMemberInfo)
				{
					// Add the child to this DOM
					((ComMemberInfo)member).AddDomTo(codeDom.Members);
					// Get the member just added
					CodeTypeMember domMember = (CodeTypeMember)
						codeDom.Members[codeDom.Members.Count - 1];
					if (prefix != null)
						domMember.Name = prefix + domMember.Name;
					if (!codeDom.IsInterface &&
						domMember is CodeMemberMethod)
					{
						domMember.Attributes |= MemberAttributes.Abstract;
					}
				}
			}
		}
		
		// Get or create a interface info for the specified interface
		internal static ComInterfaceInfo GetInterfaceInfo
			(TypeLibrary typeLib,
			TYPEKIND typeKind,
			int index)
		{
			UCOMITypeInfo typeInfo;
			Guid guid;
			typeLib.ITypeLib.GetTypeInfo(index, out typeInfo);
			guid = GuidFromTypeInfo(typeInfo);
			// Use the TypeLibrary lock to prevent deadlocks
			lock (typeof(TypeLibrary))
			{
				// Never heard of it, get the defining type library
				ComInterfaceInfo intInfo = 
					(ComInterfaceInfo)_interfacesByGuid[guid];
				if (intInfo == null)
				{
					// Add the interface to the table before we call
					// setup because setup will try to create the
					// inherited interfaces, and it should find
					// this one (otherwise it will stack overflow)
					intInfo = new ComInterfaceInfo();
					_interfacesByGuid.Add(guid, intInfo);
					intInfo.Setup(typeLib, typeKind, index);
				} 
				return intInfo;
			}
		}
		
		// Return the CLR type associated with this interface,
		// if known.  It can only be known if the interface is associated
		// with a type library
		internal Type GetCLRType()
		{
			if (_typeLib != null)
			{
				return _typeLib.FindTypeByName(Name,
											   !TypeLibrary.FIND_CLASS);
			}
			return null;
		}
		
		internal static ArrayList GetImplementedInterfacesKnown
			(Object obj)
		{
			return GetImplementedInterfaces(obj, _interfacesByGuid.Keys);
		}
		
		internal static ArrayList GetImplementedInterfacesAll()
		{
			// Not needed for now
			throw new Exception("not implemented");
		}
		
		// This determines the type of the COM object
		// by asked it which interfaces it supports.  This is
		// done by asking it from all of the interfaces guids in
		// the specified collection.
		protected static ArrayList GetImplementedInterfaces
			(Object obj,
			ICollection allInterfacesGuids)
		{
			IntPtr unkPtr = Marshal.GetIUnknownForObject(obj);
			ArrayList ret = new ArrayList();
			Marshal.AddRef(unkPtr);
			int i = 0;
			foreach (Guid guid in allInterfacesGuids)
			{
				Guid tempGuid = guid;
				IntPtr implPtr;
				Marshal.QueryInterface(unkPtr, ref tempGuid, out implPtr);
				if (implPtr != IntPtr.Zero)
				{
					ret.Add(_interfacesByGuid[guid]);
				}
				i++;
			}                                                    
			Marshal.Release(unkPtr);
			return ret;
		}
		
		internal override BasicInfo FindMemberByName(String name)
		{
			if (name == null)
				return null;
			// Need to try the variations for the properties
			BasicInfo bi = base.FindMemberByName(name);
			if (bi != null)
				return bi;
			bi = base.FindMemberByName(name + ComMemberInfo.NAMEKEY_GET);
			if (bi != null)
				return bi;
			bi = base.FindMemberByName(name + ComMemberInfo.NAMEKEY_SET);
			if (bi != null)
				return bi;
			return null;
		}
		
		internal override Object GetSortKey()
		{
			return Name + _guidStr;
		}
		
		public override void GetDetailText()
		{
			// See if we have type lib information from the registry
			if (_container == null &&
				_typeLibString != null &&
				_typeLibVersion != null)
			{
				try
				{
					_container = TypeLibrary.GetTypeLib
						(new Guid(_typeLibString),
						_typeLibVersion);
					_typeLib = (TypeLibrary)_container;
				}
				catch (Exception ex)
				{
					TraceUtil.WriteLineWarning
						(this,
						"Error getting typelib for " 
						+ this + " ex: " + ex);
					// Ignore
				}
			}
			base.GetDetailText();
		}
		
		public override String ToString()
		{
			if (_container != null)
				return base.ToString() + " typeLib: " + _container.ToString();
			return base.ToString();
		}
	}
}

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

using NoGoop.ObjBrowser;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;

namespace NoGoop.Obj
{
	internal class ComMemberInfo : BasicInfo
	{
		internal class ComParamInfo
		{
			internal String             _name;
			internal String             _type;
			internal PARAMFLAG          _paramFlags;
			
			internal ComParamInfo(String name, String typeName, PARAMFLAG paramFlags)
			{
				_name = name;
				_type = typeName;
				_paramFlags = paramFlags;
			}
		}
		
		internal const String NAMEKEY_GET = "_get";
		internal const String NAMEKEY_SET = "_set";
		internal const String NAMEKEY_SETREF = "_setref";
		
		// The key used for the member names, to be unique
		// For a property it has the suffix _get or _set
		protected String                _nameKey;
		internal bool                   _property;
		
		// If this is a property get method, otherwise, its
		// a property set method.  Only valid if _property
		// is set.
		//internal bool                   _propGet;
		internal ArrayList              _parameters;
		internal int                    _memberId;
		internal bool                   _dispatch;
		
		// Return type if a method, type if a property
		internal String                 _type;
		
		// Index of this member in its parent
		protected int                   _index;
		
		// The text interpretation of any FUNCFLAGS
		protected String                _flagsString;
		
		internal String NameKey {
			get {
				return _nameKey;
			}
		}
		
		internal static ComMemberInfo MakeComMemberInfo(BasicInfo parent,
			TYPEKIND typeKind,
			UCOMITypeInfo typeInfo,
			int index,
			bool dispatch,
			bool dual)
		{
			IntPtr funcDescPtr;
			FUNCDESC funcDesc;
			ComMemberInfo comMemberInfo = null;
			typeInfo.GetFuncDesc(index, out funcDescPtr);
			funcDesc = (FUNCDESC)Marshal.PtrToStructure(funcDescPtr, typeof(FUNCDESC));
			// from http://www.opengroup.org/onlinepubs/009899899/toc.htm
			// Table 25-43:  MEMBERID Format
			//
			// Bits Value
			// 0 - 15 Offset. Any value is permissible.
			// 16 - 21 The nesting level of this type information
			//   in the inheritance hierarchy. For example: 
			//   interface mydisp : IDispatch. 
			//   The nesting level of IUnknown() is 0, 
			//   IDispatch is 1, and MyDisp is 2.
			// 22 - 25 Reserved. Must be zero.
			// 26 - 28 Value of the DISPID.
			// 29 TRUE if this is the member ID for a FUNCDESC; otherwise FALSE.
			// 30 - 31 Must be 01.
			// For a dispatch interface, show only those members that are
			// part of the user's definition, which is bits 16-17 == 2 
			// (as above); also only if this is a MEMBERID format (bit 30 on)
			// (some members just use the low order bits and the rest 
			// are empty); also show any negative member Id.
			if (!dispatch || 
//				((Int16)funcDesc.memid < 0) ||
			    (funcDesc.memid & 0xFFFF) < 0 ||
				(funcDesc.memid & 0x40000000) == 0 ||
				(funcDesc.memid & 0x30000) == 0x20000) {
				comMemberInfo = new ComMemberInfo(parent,
												  typeKind,
												  typeInfo,
												  index,
												  dispatch,
												  funcDesc);
			} else {
				if (TraceUtil.If(null, TraceLevel.Verbose)) {
					Trace.WriteLine("MemberInfo: SKIPPING index: " 
									+ index + " memid: 0x" 
									+ funcDesc.memid.ToString("X"));
				}
			}
			typeInfo.ReleaseFuncDesc(funcDescPtr);
			return comMemberInfo;
		}
		
		internal ComMemberInfo(BasicInfo parent,
							   TYPEKIND typeKind,
							   UCOMITypeInfo typeInfo,
							   int index,
							   bool dispatch,
							   FUNCDESC funcDesc) :
				base(typeInfo)
		{
			int actLen;
			String[] memberNames = new String[100];
			_typeInfo.GetNames(funcDesc.memid, memberNames, 
							   memberNames.Length, 
							   out actLen);
						
			_memberId = funcDesc.memid;
			// the high order part of the memberId is flags
			// for a dispatch interface
			if (dispatch)
			{
				_memberId &= 0xffff;
				// Make sure we get a 16 bit integer so that negative
				// DISPIDs show up correctly
				_memberId = (Int16)_memberId;
			}
			String docString;
			// Note, use the original unmasked memberId
			_typeInfo.GetDocumentation(funcDesc.memid, out _name, 
									   out docString, out _helpContext, 
									   out _helpFile);
			// Set using property so that nulls are checked
			DocString = docString;
			_parameters = new ArrayList();
			_typeLib = parent.TypeLib;
			_index = index;
			_container = parent;
			_printName = _name;
			_nameKey = (String)_name;
			// Add the DISPID to the dispatch interfaces
			if (dispatch)
			{
				_dispatch = true;
				_printName += " - " + _memberId;
			}
			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("MemberInfo: " + _name);
			_type = TypeLibUtil.TYPEDESCToString
				(_typeLib,
				 _typeInfo,
				 funcDesc.elemdescFunc.tdesc,
				 TypeLibUtil.COMTYPE);
			if (funcDesc.invkind == INVOKEKIND.INVOKE_FUNC)
			{
				_infoType = "Function";
				_property = false;
				if (funcDesc.cParams > 0)
				{
					AddParams(funcDesc, memberNames, funcDesc.cParams);
				}
			}
			else
			{
				if (funcDesc.invkind == INVOKEKIND.INVOKE_PROPERTYGET)
				{
					_printName += " (get)";
					_nameKey += NAMEKEY_GET;
				}
				else if (funcDesc.invkind == INVOKEKIND.INVOKE_PROPERTYPUT)
				{
					_printName += " (put)";
					_nameKey += NAMEKEY_SET;
				}
				else if (funcDesc.invkind == INVOKEKIND.INVOKE_PROPERTYPUTREF)
				{
					_printName += " (put ref)";
					_nameKey += NAMEKEY_SETREF;
				}
				_infoType = "Property";
				_property = true;
			}
			_flagsString = FlagsString((FUNCFLAGS)funcDesc.wFuncFlags);
			if (_property)
			{
				_presInfo = PresentationMap.
					GetInfo(PresentationMap.COM_PROPERTY);
			}
			else
			{
				_presInfo = PresentationMap.
					GetInfo(PresentationMap.COM_METHOD);
			}
		}
		
		protected String FlagsString(FUNCFLAGS funcFlags)
		{
			String flags = "";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FBINDABLE) != 0)
				flags += " bindable";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FDEFAULTBIND) != 0)
				flags += " default_bind";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FDEFAULTCOLLELEM) != 0)
				flags += " defaultcollelem";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FDISPLAYBIND) != 0)
				flags += " display_bind";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FHIDDEN) != 0)
				flags += " hidden";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FIMMEDIATEBIND) != 0)
				flags += " immediate_bind";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FNONBROWSABLE) != 0)
				flags += " nonbrowsable";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FREPLACEABLE) != 0)
				flags += " replacable";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FREQUESTEDIT) != 0)
				flags += " request_edit";
			if ((funcFlags & FUNCFLAGS.FUNCFLAG_FRESTRICTED) != 0)
				flags += " restricted";
			// Get rid of initial leading space when returning
			if (!flags.Equals(""))
				return flags.Substring(1);
			return null;
		}
		
		protected void AddParams(FUNCDESC funcDesc,
								 String[] names,
								 int paramCount)
		{
			IntPtr elemPtr = funcDesc.lprgelemdescParam;
			for (int i = 0; i < paramCount; i++)
			{
				ELEMDESC elemDesc =
					(ELEMDESC)Marshal.PtrToStructure(elemPtr, 
													 typeof(ELEMDESC));
				ComParamInfo pi = new ComParamInfo
					(names[i + 1],
					 TypeLibUtil.TYPEDESCToString
					 (_typeLib,
					  _typeInfo,
					  elemDesc.tdesc,
					  TypeLibUtil.COMTYPE),
					 elemDesc.desc.paramdesc.wParamFlags);
				_parameters.Add(pi);
				// Point to the next one
				elemPtr = new IntPtr(elemPtr.ToInt64() + 
									 Marshal.SizeOf(elemDesc));
			}
		}
		
		protected override CodeObject GenerateCodeDom()
		{
			CodeTypeMember codeDom;
			IntPtr funcDescPtr;
			FUNCDESC funcDesc;
			// Have to get the FUNCDESC from the _typeInfo because
			// it cannot be saved.  The reason for this is that a TYPEDESC
			// may have a pointer to another TYPEDESC which is allocated
			// in the unmanaged memory and that pointer won't be any good
			// outside of the scope where the FUNCDESC is held.
			_typeInfo.GetFuncDesc(_index, out funcDescPtr);
			funcDesc = 
				(FUNCDESC)Marshal.PtrToStructure(funcDescPtr, 
												 typeof(FUNCDESC));
			if (funcDesc.invkind == INVOKEKIND.INVOKE_FUNC)
			{
				CodeMemberMethod meth;
				meth = new CodeMemberMethod();
				codeDom = meth;
				TYPEDESC retType = funcDesc.elemdescFunc.tdesc;
				if (_parameters.Count > 0)
				{
					int limit = _parameters.Count;
					// If the last parameter is a retval and the
					// function returns an HRESULT, then make
					// the function return the last parameter
					if ((VarEnum)funcDesc.elemdescFunc.tdesc.vt 
						== VarEnum.VT_HRESULT)
					{
						ComParamInfo lastParam = (ComParamInfo)
							_parameters[_parameters.Count - 1];
						if ((lastParam._paramFlags & 
							PARAMFLAG.PARAMFLAG_FRETVAL) != 0)
						{
							IntPtr elemPtr = funcDesc.lprgelemdescParam;
							ELEMDESC elemDesc = new ELEMDESC();
							// Point to the last one
							elemPtr = new IntPtr(elemPtr.ToInt64() + 
												 ((_parameters.Count - 1) * 
												  Marshal.SizeOf(elemDesc)));
							elemDesc = (ELEMDESC)
								Marshal.PtrToStructure(elemPtr, 
													   typeof(ELEMDESC));
							// Make the return type the last parameter's
							retType = elemDesc.tdesc;
							limit--;
						}
					}
					// Only add up to the limit
					// (may omit the last paremeter)
					AddDomParams(funcDesc, meth, limit);
				}
				// HRESULT becomes void because its handled by the exception
				// mechanism, we just leave the return type null
				if ((VarEnum)retType.vt != VarEnum.VT_HRESULT)
				{
					String typeName = TypeLibUtil.TYPEDESCToString
						(_typeLib,
						 _typeInfo,
						 retType,
						 !TypeLibUtil.COMTYPE);
					// Get rid of the ref since this is now a return type
					if (typeName.StartsWith("ref "))
						typeName = typeName.Substring(4);
					if (TraceUtil.If(this, TraceLevel.Info))
					{
						Trace.WriteLine(this, "CG -  " + Name 
										+ " return: " + typeName);
					}
					meth.ReturnType = new CodeTypeReference(typeName);
				}
			}
			else
			{
				CodeMemberProperty prop;
				prop = new CodeMemberProperty();
				codeDom = prop;
				prop.Type = new CodeTypeReference
					(TypeLibUtil.TYPEDESCToString
					 (_typeLib,
					  _typeInfo,
					  funcDesc.elemdescFunc.tdesc,
					  !TypeLibUtil.COMTYPE));
			}
			codeDom.Name = Name;
			codeDom.Attributes = MemberAttributes.Public;
			_typeInfo.ReleaseFuncDesc(funcDescPtr);
			return codeDom;
		}
		
		protected void AddDomParams(FUNCDESC funcDesc,
									CodeMemberMethod meth,
									int limit)
		{
			IntPtr elemPtr = funcDesc.lprgelemdescParam;
			for (int i = 0; i < limit; i++)
			{
				ELEMDESC elemDesc =
					(ELEMDESC)Marshal.PtrToStructure(elemPtr, 
													 typeof(ELEMDESC));
				ComParamInfo parameter = (ComParamInfo)
					_parameters[i];
				String paramType = TypeLibUtil.TYPEDESCToString
					(_typeLib,
					 _typeInfo,
					 elemDesc.tdesc,
					 !TypeLibUtil.COMTYPE);
				String paramInOut = null;
				if ((parameter._paramFlags & PARAMFLAG.PARAMFLAG_FIN) != 0)
				{
					paramInOut = "In";
				}
				else if 
					((parameter._paramFlags & PARAMFLAG.PARAMFLAG_FOUT) != 0)
				{
					paramInOut = "Out";
					// Ref becomes out for an output parameter
					if (paramType.StartsWith("ref "))
						paramType = "out " + paramType.Substring(4);
					else
						paramType = "out " + paramType;
				}
				CodeParameterDeclarationExpression paramExpr =
					new CodeParameterDeclarationExpression
						(paramType, parameter._name);
				if (paramInOut != null)
				{
					paramExpr.CustomAttributes.Add
						(new CodeAttributeDeclaration
							("System.Runtime.InteropServices." + paramInOut));
				}
				meth.Parameters.Add(paramExpr);
				// Point to the next one
				elemPtr = new IntPtr(elemPtr.ToInt64() + 
									 Marshal.SizeOf(elemDesc));
			}
		}
		
		internal String GetSignature()
		{
			String sig = _type + " " + _name; 
			sig += "("; 
			
			int i = 0;
			foreach (ComParamInfo pi in _parameters)
			{
				sig += pi._type + " " + pi._name;
				if (i < (_parameters.Count - 1))
					sig += ", ";
				i++;
			}
			sig += ")";
			return sig;
		}
		
		public override void GetDetailText()
		{
			base.GetBasicDetailText();
			DetailPanel.Add(_infoType,
							!ObjectBrowser.INTERNAL,
							10,
							GetSignature());
			if (_dispatch)
			{
				DetailPanel.Add("Dispatch Id",
								!ObjectBrowser.INTERNAL,
								20,
								_memberId.ToString());
			}
			DetailPanel.AddLink(_container._infoType,
								!ObjectBrowser.INTERNAL,
								15,
								_container,
								null);
			if (_flagsString != null)
			{
				DetailPanel.Add("Function Flags",
								!ObjectBrowser.INTERNAL,
								200,
								_flagsString);
			}
		}
		
		public override String ToString()
		{
			return base.ToString();
		}
	}
}

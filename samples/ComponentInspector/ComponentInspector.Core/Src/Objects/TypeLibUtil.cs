// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.Obj
{
	internal class TypeLibUtil
	{
		public const bool COMTYPE = true;
		
		protected static String GetTypeStr(int vtNum, bool comType)
		{
			int comInd;
			if (comType)
				comInd = 0;
			else
				comInd = 1;
			vtNum = vtNum & ActiveX.VT_TYPEMASK;
			/*
			if (vtNum < 64 && 
				((VarEnum)vtNum > VarEnum.VT_UINT_PTR) ||
				(VarEnum)vtNum > VarEnum.VT_VERSIONED_STREAM)
				return "TOOLARGE[0x" + vtNum.ToString("X") + "]"; 
			*/
			String ret;
			if (vtNum < 64)
				ret = ActiveX.VTypes0[vtNum, comInd];
			else
				ret = ActiveX.VTypes64[vtNum - 64, comInd];
			return ret;
		}
		
		// The TypeLibrary is used to resolve any typedefs
		public static String TYPEDESCToString(TypeLibrary typeLib,
											 UCOMITypeInfo typeInfo,
											 TYPEDESC typeDesc,
											 bool comType)
		{
			String ret =
				TYPEDESCToStringInt(typeLib, typeInfo, 
									typeDesc, comType, 0);
			return ret;
		}
		
		public static String TYPEDESCToStringInt(TypeLibrary typeLib,
												UCOMITypeInfo typeInfo,
												TYPEDESC typeDesc,
												bool comType,
												int level)
		{
			String ret;
			try
			{
				if ((VarEnum)typeDesc.vt == VarEnum.VT_PTR ||
					(VarEnum)(typeDesc.vt & ActiveX.VT_TYPEMASK) ==
					VarEnum.VT_SAFEARRAY)
				{
					TYPEDESC pTypeDesc =
						(TYPEDESC)Marshal.PtrToStructure(typeDesc.lpValue,
														typeof(TYPEDESC));
					ret = TYPEDESCToStringInt(typeLib,
											 typeInfo, pTypeDesc, 
											 comType, level + 1);
					if ((VarEnum)(typeDesc.vt & ActiveX.VT_TYPEMASK) ==
						VarEnum.VT_SAFEARRAY)
					{
						// FIXME - what about the non-comType
						return "SAFEARRAY(" + ret + ")";
					}
					if (comType)
					{
						ret += "*";
					}
					else
					{
						// void* become IntPtr
						if (ret.Equals("void"))
							ret = "System.IntPtr";
						else
						{
							// The first pointer is not a ref, its only
							// a ref if there are two
							// FIXME - what if there are more?
							if (level == 1)
								ret = "ref " + ret;
						}
					}
					return ret;
				}
				if ((VarEnum)(typeDesc.vt & ActiveX.VT_TYPEMASK) == 
					VarEnum.VT_CARRAY)
				{
					// typeDesc.lpValue in this case is really the laValue 
					// (since TYPEDESC is a contains a union of pointers)
					ARRAYDESC pArrayDesc =
						(ARRAYDESC)Marshal.PtrToStructure(typeDesc.lpValue,
														typeof(ARRAYDESC));
					ret = TYPEDESCToStringInt(typeLib, typeInfo,
											 pArrayDesc.tdescElem,
											 comType,
											 level + 1);
					// Just show the number of diminsions, don't worry about
					// showing the size of each since we don't want to 
					// get into marshalling the variable length ARRAYDESC
					// structure
					for (int i = 0; i < pArrayDesc.cDims; i++)
						ret += "[]";
					return ret;
				}
				if ((VarEnum)typeDesc.vt == VarEnum.VT_USERDEFINED)
				{
					UCOMITypeInfo uTypeInfo = null;
					// FIXME - sometimes this chokes and hangs due to a bad
					// handle value here, need to do something to prevent this
					int href = typeDesc.lpValue.ToInt32();
					typeInfo.GetRefTypeInfo(href, out uTypeInfo);
					if (uTypeInfo != null)
					{
						String docName;
						String docString;
						int helpContext;
						String helpFile;
						uTypeInfo.GetDocumentation(-1, out docName, 
												  out docString, 
												  out helpContext, 
												  out helpFile);
						// Fix up misc references
						if (docName.Equals("GUID"))
							docName = "System.Guid";
						// Present the user names for the types in COM
						// mode, but for the CLR types, get the real
						// underlying names
						if (!comType)
							return typeLib.ResolveTypeDef(docName, comType);
						return docName;
					}
					else
					{
						TraceUtil.WriteLineWarning(null, "USER: " + href 
												  + " 0x" + href.ToString("X") 
												   + " ***UNKNOWN***");
						return "(userDef unknown)";
					}
				}
				return GetTypeStr(typeDesc.vt, comType);
			}
			catch (Exception ex)
			{
				TraceUtil.WriteLineWarning
					(null, "ActiveX type conversion error: " + ex);
				return "(error)";
			}                
		}
	}
}

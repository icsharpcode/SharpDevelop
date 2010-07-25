// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NoGoop.Util;

namespace NoGoop.Obj
{
	internal class ComStructInfo : BasicInfo
	{
		internal ComStructInfo(TypeLibrary typeLib, 
							   TYPEKIND typeKind,
							   int index) : 
				base(typeLib, typeKind, index)
		{
			switch (typeKind)
			{
			case TYPEKIND.TKIND_ENUM:
				_infoType = "Enum";
				break;
			case TYPEKIND.TKIND_MODULE:
				_infoType = "Module";
				break;
			case TYPEKIND.TKIND_RECORD:
				_infoType = "Struct";
				break;
			case TYPEKIND.TKIND_UNION:
				_infoType = "Union";
				break;
			}

			TYPEATTR typeAttr;
			IntPtr typeAttrPtr;
			_typeInfo.GetTypeAttr(out typeAttrPtr);
			typeAttr = 
				(TYPEATTR)Marshal.PtrToStructure(typeAttrPtr, 
												 typeof(TYPEATTR));

			for (int i = 0; i < typeAttr.cVars; i++)
			{
				ComVariableInfo mi = new ComVariableInfo(this,
														 _typeKind,
														 _typeInfo,
														 i);
				_members.Add(mi);
			}

			_typeInfo.ReleaseTypeAttr(typeAttrPtr);

			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("Struct: " + _name);
		}

		protected override CodeObject GenerateCodeDom()
		{
			CodeTypeDeclaration codeDom = new CodeTypeDeclaration();
			codeDom.Name = _name;

			switch (_typeKind)
			{
			case TYPEKIND.TKIND_ENUM:
				codeDom.IsEnum = true;
				break;
			case TYPEKIND.TKIND_MODULE:
				// Modules don't convert
				return null;
			case TYPEKIND.TKIND_RECORD:
			case TYPEKIND.TKIND_UNION:
				codeDom.IsStruct = true;
				break;
			}

			foreach (Object member in _members)
			{
				if (member is ComVariableInfo)
				{
					((ComVariableInfo)member).AddDomTo(codeDom.Members);
				}
			}
			return codeDom;
		}
	}
}

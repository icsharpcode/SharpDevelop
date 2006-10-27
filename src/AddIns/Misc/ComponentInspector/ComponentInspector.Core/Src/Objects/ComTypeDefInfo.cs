// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NoGoop.ObjBrowser;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;

namespace NoGoop.Obj
{

	internal class ComTypeDefInfo : BasicInfo
	{
		internal String                 _varComType;
		internal String                 _varClrType;

		internal ComTypeDefInfo(TypeLibrary typeLib, 
								TYPEKIND typeKind,
								int index) : 
				base(typeLib, typeKind, index)
		{
			TYPEATTR typeAttr;
			IntPtr typeAttrPtr;
			_typeInfo.GetTypeAttr(out typeAttrPtr);
			typeAttr = 
				(TYPEATTR)Marshal.PtrToStructure(typeAttrPtr, 
												 typeof(TYPEATTR));

			_infoType = "TypeDef";
			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("TypeDefInfo: " + _name);
			_varComType = 
				TypeLibUtil.TYPEDESCToString(typeLib,
											 _typeInfo,
											 typeAttr.tdescAlias,
											 TypeLibUtil.COMTYPE);
			_varClrType = 
				TypeLibUtil.TYPEDESCToString(typeLib,
											 _typeInfo,
											 typeAttr.tdescAlias,
											 !TypeLibUtil.COMTYPE);
			_typeInfo.ReleaseTypeAttr(typeAttrPtr);

			// Add to the typelibrary for resolution
			typeLib.TypeDefHash.Add(_name, this);
		}

		public override void GetDetailText()
		{
			base.GetDetailText();

			DetailPanel.Add("Aliased Type",
							!ObjectBrowser.INTERNAL,
							100,
							_varComType);

		}

		public override String ToString()
		{
			return base.ToString();
		}
	}
}

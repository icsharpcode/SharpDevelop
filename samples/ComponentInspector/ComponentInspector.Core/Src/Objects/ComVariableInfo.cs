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

using NoGoop.ObjBrowser;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.Obj
{
	internal class ComVariableInfo : BasicInfo
	{
			
		internal String                 _varType;
		internal String                 _enumValue;

		// Index of this member in its parent
		protected int                   _index;

		internal override String PrintName
		{
			get
				{
					if (_typeKind == TYPEKIND.TKIND_ENUM && 
						_enumValue != null)
						return base.Name + " = " + _enumValue;
					return base.PrintName;
				}
		}            

		internal ComVariableInfo(BasicInfo parent,
								 TYPEKIND typeKind,
								 UCOMITypeInfo typeInfo,
								 int index) : base(typeInfo)
		{
			IntPtr varDescPtr;
			CORRECT_VARDESC varDesc;

			_typeKind = typeKind;
			_typeLib = parent.TypeLib;
			_container = parent;
			_index = index;

			_typeInfo.GetVarDesc(_index, out varDescPtr);
			varDesc = (CORRECT_VARDESC)
				Marshal.PtrToStructure(varDescPtr, 
									   typeof(CORRECT_VARDESC));

			int actLen;
			String[] memberNames = new String[100];
			_typeInfo.GetNames(varDesc.memid, memberNames, 
							   memberNames.Length, 
							   out actLen);
						
			Name = memberNames[0];

			if (TraceUtil.If(this, TraceLevel.Verbose))
				Trace.WriteLine("VariableInfo: " + _name);
			if (_typeKind == TYPEKIND.TKIND_ENUM)
			{
				_infoType = "Constant";
				try
				{
					Object value = Marshal.GetObjectForNativeVariant
						(varDesc.u.lpvarValue);
					_enumValue = value.ToString();
				}
				catch (Exception ex)
				{
					_enumValue = "Unknown variant: 0x" 
						+ varDesc.u.lpvarValue.ToInt32().ToString("X");
					TraceUtil.WriteLineWarning(this,
											   "Exception reading enum value: " + ex);
				}
			}
			else
			{
				_infoType = "Variable";
			}

			TYPEDESC typeDesc = varDesc.elemdescVar.tdesc;
			_varType = TypeLibUtil.TYPEDESCToString
				(_typeLib,
				 _typeInfo,
				 typeDesc,
				 TypeLibUtil.COMTYPE);

			_typeInfo.ReleaseVarDesc(varDescPtr);

			_presInfo = PresentationMap.
				GetInfo(PresentationMap.COM_VARIABLE);
		}

		protected override CodeObject GenerateCodeDom()
		{
			IntPtr varDescPtr;
			CORRECT_VARDESC varDesc;
			CodeMemberField codeDom;

			codeDom = new CodeMemberField();

			_typeInfo.GetVarDesc(_index, out varDescPtr);
			varDesc = (CORRECT_VARDESC)
				Marshal.PtrToStructure(varDescPtr, 
									   typeof(CORRECT_VARDESC));

			codeDom.Name = Name;
			codeDom.Attributes = MemberAttributes.Public;

			if (_typeKind == TYPEKIND.TKIND_ENUM)
			{
				try
				{
					Object value = Marshal.GetObjectForNativeVariant
						(varDesc.u.lpvarValue);
					codeDom.InitExpression = 
						new CodePrimitiveExpression(value);
				}
				catch
				{
					// Ignore, was handled earlier
				}
			}

			TYPEDESC typeDesc = varDesc.elemdescVar.tdesc;
			codeDom.Type = new CodeTypeReference
				(TypeLibUtil.TYPEDESCToString(_typeLib,
											  _typeInfo,
											  typeDesc,
											  !TypeLibUtil.COMTYPE));

			_typeInfo.ReleaseVarDesc(varDescPtr);
			return codeDom;
		}

		public override void GetDetailText()
		{
			base.GetDetailText();

			DetailPanel.Add("Type",
							!ObjectBrowser.INTERNAL,
							100,
							_varType);

			if (_typeKind == TYPEKIND.TKIND_ENUM && 
				_enumValue != null)
			{
				DetailPanel.Add("Value",
								!ObjectBrowser.INTERNAL,
								110,
								_enumValue);
			}

		}

		public override String ToString()
		{
			return base.ToString();
		}
	}
}

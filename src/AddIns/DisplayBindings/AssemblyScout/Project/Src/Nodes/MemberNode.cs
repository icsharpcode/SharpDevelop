// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

using ICSharpCode.Core;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class MemberNode : AssemblyTreeNode
	{
		bool special = false;
		bool isEnum = false;
		
		public MemberNode(IMethod methodinfo2) : base ("", methodinfo2, NodeType.Method)
		{
			SetNodeName();
		}
		
		public MemberNode(IProperty prop, bool Special) : base ("", prop, NodeType.Property)
		{
			SetNodeName();
			if(special = Special) CreateSpecialNodes(prop);
		}
		
		public MemberNode(IEvent evt, bool Special) : base ("", evt, NodeType.Event)
		{
			SetNodeName();
			if(special = Special) CreateSpecialNodes(evt);
		}
		
		public MemberNode(IField fld, bool IsEnum) : base ("", fld, NodeType.Field)
		{
			isEnum = IsEnum;
			SetNodeName();
		}
		
		void SetNodeName()
		{
			if (attribute == null) {
				Text = "no name";
				return;
			}
			
			Text = GetShortMemberName((IMember)attribute, isEnum);
			if (Text.EndsWith("[static]")) {
				this.NodeFont = new Font("Tahoma", 8, FontStyle.Italic);
			}
		}
		
		void CreateSpecialNodes(IProperty prop)
		{
			IMethod getm = prop.GetterMethod;
			IMethod setm = prop.SetterMethod;
			
			if (getm != null)
				Nodes.Add(new MethodNode(getm));
			if (setm != null)
				Nodes.Add(new MethodNode(setm));			
		}
		
		void CreateSpecialNodes(IEvent evt)
		{
			IMethod addm    = evt.AddMethod;
			IMethod raisem  = evt.RaiseMethod;
			IMethod removem = evt.RemoveMethod;
			
			if (addm != null)
				Nodes.Add(new MethodNode(addm));
			if (raisem != null)
				Nodes.Add(new MethodNode(raisem));
			if (removem != null)
				Nodes.Add(new MethodNode(removem));
		}
		
		protected override void SetIcon()
		{
			
			
			if (attribute == null)
				return;
			switch (type) {
				case NodeType.Method:
					IMethod methodinfo = (IMethod)attribute;
					ImageIndex = SelectedImageIndex = ClassBrowserIconService.GetIcon(methodinfo);
					break;
				
				case NodeType.Event:
					IEvent eventinfo = (IEvent)attribute;
					ImageIndex  = SelectedImageIndex = ClassBrowserIconService.GetIcon(eventinfo);
					break;
								
				case NodeType.Property:
					IProperty propertyinfo = (IProperty)attribute;
					ImageIndex  = SelectedImageIndex = ClassBrowserIconService.GetIcon(propertyinfo);
					break;
				
				case NodeType.Field:
					IField fieldinfo = (IField)attribute;
					ImageIndex = SelectedImageIndex = ClassBrowserIconService.GetIcon(fieldinfo);
					break;
				
			}
		}
		
		static 
		
		public static string GetShortMemberName(IMember mi, bool IsEnum) {
			string ret = "";
			
			ret = mi.Name;
			
			try {
				
				bool dispReturn = PropertyService.Get("AddIns.AssemblyScout.ShowReturnTypes", true);
				
				if (mi is IMethod) {
					IMethod mii = mi as IMethod;
					
					if (mii.IsConstructor) {
						dispReturn = false;
						ret = mi.DeclaringType.Name;
					}
					
					ret += GetParams(mii.Parameters, true);
					
				} else if (mi is IProperty) {
					IProperty ppi = mi as IProperty;
	
					ret += GetParams(ppi.Parameters, false);
				}
				
				if (dispReturn && !IsEnum) ret += " : " + GetNestedName(AssemblyTree.CurrentAmbience.GetIntrinsicTypeName(mi.ReturnType.FullyQualifiedName));
				
				if (mi.IsStatic && !IsEnum) ret += " [static]";
				
				if (IsEnum && mi is SharpAssemblyField) {
					SharpAssemblyField saField = mi as SharpAssemblyField;
					if (saField.InitialValue != null) {
						ret += " = " + saField.InitialValue.ToString();
					}
				}
				
			} catch {
				Console.WriteLine("GetShortMemberName: Error");
			}
			
			return ret;
		}

		public static string GetParams(ParameterCollection piarr, bool IncludeBrackets) {
			string param = "";
			foreach(IParameter pi in piarr) {
				param += GetNestedName(AssemblyTree.CurrentAmbience.GetIntrinsicTypeName(pi.ReturnType.FullyQualifiedName)) + ", ";
			}
			if (param.Length > 0) param = param.Substring(0, param.Length - 2);
			if (param != "" || IncludeBrackets) param = "(" + param + ")";
			return param;
		}

		public static string GetNestedName(string name) {
			int i = name.LastIndexOf(".");
			if (i == -1) return name;
			return name.Substring(i + 1);
		}

		
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using Debugger.Interop.CorDebug;
using System;
using System.Collections.Generic;
using Debugger.MetaData;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.TreeModel
{
	public class ICorDebug
	{
		public class InfoNode: TreeNode
		{
			List<TreeNode> children;
			
			public InfoNode(TreeNode parent, string name, string text)
				: this(parent, name, text, _ => null)
			{
				
			}
			
			public InfoNode(TreeNode parent, string name, string text, Func<TreeNode, List<TreeNode>> children)
				: base(parent)
			{
				this.Name = name;
				this.Text = text;
				this.children = children(this);
			}
			
			public override IEnumerable<TreeNode> ChildNodes {
				get { return children; }
			}
			
			public void AddChild(string name, string text)
			{
				if (children == null) {
					children = new List<TreeNode>();
				}
				children.Add(new InfoNode(this, name, text));
			}
			
			public void AddChild(string name, string text, Func<TreeNode, List<TreeNode>> subChildren)
			{
				if (children == null) {
					children = new List<TreeNode>();
				}
				children.Add(new InfoNode(this, name, text, p => subChildren(p)));
			}
		}
		
		public static InfoNode GetDebugInfoRoot(AppDomain appDomain, ICorDebugValue corValue)
		{
			return new InfoNode(null, "ICorDebug", "", p => GetDebugInfo(p, appDomain, corValue));
		}
		
		public static List<TreeNode> GetDebugInfo(TreeNode parent, AppDomain appDomain, ICorDebugValue corValue)
		{
			List<TreeNode> items = new List<TreeNode>();
			
			if (corValue is ICorDebugValue) {
				InfoNode info = new InfoNode(parent, "ICorDebugValue", "");
				info.AddChild("Address", corValue.GetAddress().ToString("X8"));
				info.AddChild("Type", ((CorElementType)corValue.GetTheType()).ToString());
				info.AddChild("Size", corValue.GetSize().ToString());
				items.Add(info);
			}
			if (corValue is ICorDebugValue2) {
				InfoNode info = new InfoNode(parent, "ICorDebugValue2", "");
				ICorDebugValue2 corValue2 = (ICorDebugValue2)corValue;
				string fullname;
				try {
					fullname = DebugType.CreateFromCorType(appDomain, corValue2.GetExactType()).FullName;
				} catch (DebuggerException e) {
					fullname = e.Message;
				}
				info.AddChild("ExactType", fullname);
				items.Add(info);
			}
			if (corValue is ICorDebugGenericValue) {
				InfoNode info = new InfoNode(parent, "ICorDebugGenericValue", "");
				try {
					byte[] bytes = ((ICorDebugGenericValue)corValue).GetRawValue();
					for(int i = 0; i < bytes.Length; i += 8) {
						string val = "";
						for(int j = i; j < bytes.Length && j < i + 8; j++) {
							val += bytes[j].ToString("X2") + " ";
						}
						info.AddChild("Value" + i.ToString("X2"), val);
					}
				} catch (ArgumentException) {
					info.AddChild("Value", "N/A");
				}
				items.Add(info);
			}
			if (corValue is ICorDebugReferenceValue) {
				InfoNode info = new InfoNode(parent, "ICorDebugReferenceValue", "");
				ICorDebugReferenceValue refValue = (ICorDebugReferenceValue)corValue;
				info.AddChild("IsNull", (refValue.IsNull() != 0).ToString());
				if (refValue.IsNull() == 0) {
					info.AddChild("Value", refValue.GetValue().ToString("X8"));
					if (refValue.Dereference() != null) {
						info.AddChild("Dereference", "", p => GetDebugInfo(p, appDomain, refValue.Dereference()));
					} else {
						info.AddChild("Dereference", "N/A");
					}
				}
				items.Add(info);
			}
			if (corValue is ICorDebugHeapValue) {
				InfoNode info = new InfoNode(parent, "ICorDebugHeapValue", "");
				items.Add(info);
			}
			if (corValue is ICorDebugHeapValue2) {
				InfoNode info = new InfoNode(parent, "ICorDebugHeapValue2", "");
				items.Add(info);
			}
			if (corValue is ICorDebugObjectValue) {
				InfoNode info = new InfoNode(parent, "ICorDebugObjectValue", "");
				ICorDebugObjectValue objValue = (ICorDebugObjectValue)corValue;
				info.AddChild("Class", objValue.GetClass().GetToken().ToString("X8"));
				info.AddChild("IsValueClass", (objValue.IsValueClass() != 0).ToString());
				items.Add(info);
			}
			if (corValue is ICorDebugObjectValue2) {
				InfoNode info = new InfoNode(parent, "ICorDebugObjectValue2", "");
				items.Add(info);
			}
			if (corValue is ICorDebugBoxValue) {
				InfoNode info = new InfoNode(parent, "ICorDebugBoxValue", "");
				ICorDebugBoxValue boxValue = (ICorDebugBoxValue)corValue;
				info.AddChild("Object", "", p => GetDebugInfo(p, appDomain, boxValue.GetObject()));
				items.Add(info);
			}
			if (corValue is ICorDebugStringValue) {
				InfoNode info = new InfoNode(parent, "ICorDebugStringValue", "");
				ICorDebugStringValue stringValue = (ICorDebugStringValue)corValue;
				info.AddChild("Length", stringValue.GetLength().ToString());
				info.AddChild("String", stringValue.GetString());
				items.Add(info);
			}
			if (corValue is ICorDebugArrayValue) {
				InfoNode info = new InfoNode(parent, "ICorDebugArrayValue", "");
				info.AddChild("...", "...");
				items.Add(info);
			}
			if (corValue is ICorDebugHandleValue) {
				InfoNode info = new InfoNode(parent, "ICorDebugHandleValue", "");
				ICorDebugHandleValue handleValue = (ICorDebugHandleValue)corValue;
				info.AddChild("HandleType", handleValue.GetHandleType().ToString());
				items.Add(info);
			}
			
			return items;
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using Debugger.MetaData;
using Debugger.Wrappers.CorDebug;

namespace Debugger.AddIn.TreeModel
{
	public class ICorDebug
	{
		public class InfoNode: AbstractNode
		{
			List<AbstractNode> children;
			
			public InfoNode(string name, string text): this(name, text, null)
			{
				
			}
			
			public InfoNode(string name, string text, List<AbstractNode> children)
			{
				this.Name = name;
				this.Text = text;
				this.ChildNodes = children;
				this.children = children;
			}
			
			public void AddChild(string name, string text)
			{
				if (children == null) {
					children = new List<AbstractNode>();
					this.ChildNodes = children;
				}
				children.Add(new InfoNode(name, text));
			}
			
			public void AddChild(string name, string text, List<AbstractNode> subChildren)
			{
				if (children == null) {
					children = new List<AbstractNode>();
					this.ChildNodes = children;
				}
				children.Add(new InfoNode(name, text, subChildren));
			}
		}
		
		public static InfoNode GetDebugInfoRoot(Process process, ICorDebugValue corValue)
		{
			return new InfoNode("ICorDebug", "", GetDebugInfo(process, corValue));
		}
		
		public static List<AbstractNode> GetDebugInfo(Process process, ICorDebugValue corValue)
		{
			List<AbstractNode> items = new List<AbstractNode>();
			
			if (corValue.Is<ICorDebugValue>()) {
				InfoNode info = new InfoNode("ICorDebugValue", "");
				info.AddChild("Address", corValue.Address.ToString("X8"));
				info.AddChild("Type", ((CorElementType)corValue.Type).ToString());
				info.AddChild("Size", corValue.Size.ToString());
				items.Add(info);
			}
			if (corValue.Is<ICorDebugValue2>()) {
				InfoNode info = new InfoNode("ICorDebugValue2", "");
				ICorDebugValue2 corValue2 = corValue.CastTo<ICorDebugValue2>();
				string fullname;
				try {
					fullname = DebugType.Create(process, corValue2.ExactType).FullName;
				} catch (DebuggerException e) {
					fullname = e.Message;
				}
				info.AddChild("ExactType", fullname);
				items.Add(info);
			}
			if (corValue.Is<ICorDebugGenericValue>()) {
				InfoNode info = new InfoNode("ICorDebugGenericValue", "");
				try {
					byte[] bytes = corValue.CastTo<ICorDebugGenericValue>().RawValue;
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
			if (corValue.Is<ICorDebugReferenceValue>()) {
				InfoNode info = new InfoNode("ICorDebugReferenceValue", "");
				ICorDebugReferenceValue refValue = corValue.CastTo<ICorDebugReferenceValue>();
				info.AddChild("IsNull", (refValue.IsNull != 0).ToString());
				if (refValue.IsNull == 0) {
					info.AddChild("Value", refValue.Value.ToString("X8"));
					if (refValue.Dereference() != null) {
						info.AddChild("Dereference", "", GetDebugInfo(process, refValue.Dereference()));
					} else {
						info.AddChild("Dereference", "N/A");
					}
					
				}
				items.Add(info);
			}
			if (corValue.Is<ICorDebugHeapValue>()) {
				InfoNode info = new InfoNode("ICorDebugHeapValue", "");
				items.Add(info);
			}
			if (corValue.Is<ICorDebugHeapValue2>()) {
				InfoNode info = new InfoNode("ICorDebugHeapValue2", "");
				items.Add(info);
			}
			if (corValue.Is<ICorDebugObjectValue>()) {
				InfoNode info = new InfoNode("ICorDebugObjectValue", "");
				ICorDebugObjectValue objValue = corValue.CastTo<ICorDebugObjectValue>();
				info.AddChild("Class", objValue.Class.Token.ToString("X8"));
				info.AddChild("IsValueClass", (objValue.IsValueClass != 0).ToString());
				items.Add(info);
			}
			if (corValue.Is<ICorDebugObjectValue2>()) {
				InfoNode info = new InfoNode("ICorDebugObjectValue2", "");
				items.Add(info);
			}
			if (corValue.Is<ICorDebugBoxValue>()) {
				InfoNode info = new InfoNode("ICorDebugBoxValue", "");
				ICorDebugBoxValue boxValue = corValue.CastTo<ICorDebugBoxValue>();
				info.AddChild("Object", "", GetDebugInfo(process, boxValue.Object.CastTo<ICorDebugValue>()));
				items.Add(info);
			}
			if (corValue.Is<ICorDebugStringValue>()) {
				InfoNode info = new InfoNode("ICorDebugStringValue", "");
				ICorDebugStringValue stringValue = corValue.CastTo<ICorDebugStringValue>();
				info.AddChild("Length", stringValue.Length.ToString());
				info.AddChild("String", stringValue.String);
				items.Add(info);
			}
			if (corValue.Is<ICorDebugArrayValue>()) {
				InfoNode info = new InfoNode("ICorDebugArrayValue", "");
				info.AddChild("...", "...");
				items.Add(info);
			}
			if (corValue.Is<ICorDebugHandleValue>()) {
				InfoNode info = new InfoNode("ICorDebugHandleValue", "");
				ICorDebugHandleValue handleValue = corValue.CastTo<ICorDebugHandleValue>();
				info.AddChild("HandleType", handleValue.HandleType.ToString());
				items.Add(info);
			}
			
			return items;
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace Debugger
{
	public class BaseTypeItem: ListItem
	{
		NamedValue val;
		DebugType type;
		
		public NamedValue Value {
			get {
				return val;
			}
		}
		
		public DebugType DebugType {
			get {
				return type;
			}
		}
		
		public override int ImageIndex {
			get {
				return -1;
			}
		}
		
		public override string Name {
			get {
				return StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}");
			}
		}
		
		public override string Text {
			get {
				return String.Empty;
			}
		}
		
		public override bool CanEditText {
			get {
				return false;
			}
		}
		
		public override string Type {
			get {
				return type.FullName;
			}
		}
		
		public override bool HasSubItems {
			get {
				return true;
			}
		}
		
		public override IList<ListItem> SubItems {
			get {
				return GetSubItems();
			}
		}
		
		public BaseTypeItem(NamedValue val, DebugType type)
		{
			this.val = val;
			this.type = type;
		}
		
		List<ListItem> GetMembers(BindingFlags flags)
		{
			List<ListItem> list = new List<ListItem>();
			foreach(NamedValue v in val.GetMembers(type, flags)) {
				list.Add(new ValueItem(v));
			}
			return list;
		}
		
		IList<ListItem> GetSubItems()
		{
			List<ListItem> list = new List<ListItem>();
			
			string privateRes = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateMembers}");
			string staticRes = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}");
			string privateStaticRes = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateStaticMembers}");
			
			List<ListItem> publicInstance  = GetMembers(BindingFlags.Public | BindingFlags.Instance);
			List<ListItem> publicStatic    = GetMembers(BindingFlags.Public | BindingFlags.Static);
			List<ListItem> privateInstance = GetMembers(BindingFlags.NonPublic | BindingFlags.Instance);
			List<ListItem> privateStatic   = GetMembers(BindingFlags.NonPublic | BindingFlags.Static);
			
			if (type.BaseType != null) {
				list.Add(new BaseTypeItem(val, type.BaseType));
			}
			
			if (publicStatic.Count > 0) {
				list.Add(new FixedItem(-1, staticRes, String.Empty, String.Empty, true, publicStatic));
			}
			
			if (privateInstance.Count > 0 || privateStatic.Count > 0) {
				List<ListItem> nonPublicItems = new List<ListItem>();
				if (privateStatic.Count > 0) {
					nonPublicItems.Add(new FixedItem(-1, privateStaticRes, String.Empty, String.Empty, true, privateStatic));
				}
				nonPublicItems.AddRange(privateInstance);
				list.Add(new FixedItem(-1, privateRes, String.Empty, String.Empty, true, nonPublicItems));
			}
			
			list.AddRange(publicInstance);
			
			return list;
		}
	}
}

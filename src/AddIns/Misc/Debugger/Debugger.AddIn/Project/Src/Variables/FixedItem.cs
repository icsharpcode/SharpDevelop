// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger
{
	public class FixedItem: ListItem
	{
		int imageIndex;
		string name;
		string text;
		string type;
		bool hasSubItems;
		IList<ListItem> subItems;
		
		public override int ImageIndex {
			get {
				return imageIndex;
			}
		}
		
		public override string Name {
			get {
				return name;
			}
		}
		
		public override string Text {
			get {
				return text;
			}
		}
		
		public override bool CanEditText {
			get {
				return false;
			}
		}
		
		public override string Type {
			get {
				return type;
			}
		}
		
		public override bool HasSubItems {
			get {
				return hasSubItems;
			}
		}
		
		public override IList<ListItem> SubItems {
			get {
				return subItems;
			}
		}
		
		public FixedItem(int imageIndex, string name, string text, string type, bool hasSubItems, IList<ListItem> subItems)
		{
			this.imageIndex = imageIndex;
			this.name = name;
			this.text = text;
			this.type = type;
			this.hasSubItems = hasSubItems;
			this.subItems = subItems;
		}
	}
}

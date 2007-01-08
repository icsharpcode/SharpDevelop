// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	public class ListItemEventArgs: EventArgs
	{
		ListItem listItem;
		
		public ListItem ListItem {
			get {
				return listItem;
			}
		}
		
		public ListItemEventArgs(ListItem listItem)
		{
			this.listItem = listItem;
		}
	}
}

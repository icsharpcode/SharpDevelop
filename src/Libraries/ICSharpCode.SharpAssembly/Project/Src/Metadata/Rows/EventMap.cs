// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class EventMap : AbstractRow
	{
		public static readonly int TABLE_ID = 0x12;
		
		uint   parent;    // index into the TypeDef table
		uint   eventList; // index into Event table
		
		public uint Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}
		public uint EventList {
			get {
				return eventList;
			}
			set {
				eventList = value;
			}
		}
		
		public override void LoadRow()
		{
			parent    = ReadSimpleIndex(TypeDef.TABLE_ID);
			eventList = ReadSimpleIndex(Event.TABLE_ID);
		}
	}
}

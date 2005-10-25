// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class EventPtr : AbstractRow
	{
		public static readonly int TABLE_ID = 19;
		
		uint eventPtr;
		
		public uint Event {
			get {
				return eventPtr;
			}
			set {
				eventPtr = value;
			}
		}
		
		public override void LoadRow()
		{
			eventPtr = ReadSimpleIndex(ICSharpCode.SharpAssembly.Metadata.Rows.Event.TABLE_ID);
		}
	}
}

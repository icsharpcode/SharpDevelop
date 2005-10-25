// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class FieldPtr : AbstractRow
	{
		public static readonly int TABLE_ID = 0x03;
		
		uint field;
		
		public uint Field {
			get {
				return field;
			}
			set {
				field = value;
			}
		}
		
		public override void LoadRow()
		{
			field = ReadSimpleIndex(ICSharpCode.SharpAssembly.Metadata.Rows.Field.TABLE_ID);
		}
	}
}

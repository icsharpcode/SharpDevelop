// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class FieldLayout : AbstractRow
	{
		public static readonly int TABLE_ID = 0x10;
		
		
		uint offset;
		uint field; // index into the field table
		
		public uint Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
		public uint FieldIndex {
			get {
				return field;
			}
			set {
				field = value;
			}
		}
		
		public override void LoadRow()
		{
			offset = binaryReader.ReadUInt32();
			field  = ReadSimpleIndex(Field.TABLE_ID);
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class FieldRVA : AbstractRow
	{
		public static readonly int TABLE_ID = 0x1D;
		
		uint rva;
		uint field; // index into Field table
		
		public uint RVA {
			get {
				return rva;
			}
			set {
				rva = value;
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
			rva    = binaryReader.ReadUInt32();
			field  = ReadSimpleIndex(Field.TABLE_ID);
		}
	}
}

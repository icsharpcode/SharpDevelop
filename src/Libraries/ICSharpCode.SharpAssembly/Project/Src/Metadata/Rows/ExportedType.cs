// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class ExportedType : AbstractRow
	{
		public static readonly int TABLE_ID = 0x27;
		
		uint flags;
		uint typeDefId; // 4 byte index into a TypeDef table of another module in this Assembly
		uint typeName;  // index into the String heap
		uint typeNamespace; // index into the String heap
		uint implementation; // index see 21.14
		
		public uint Flags {
			get {
				return flags;
			}
			set {
				flags = value;
			}
		}
		public uint TypeDefId {
			get {
				return typeDefId;
			}
			set {
				typeDefId = value;
			}
		}
		public uint TypeName {
			get {
				return typeName;
			}
			set {
				typeName = value;
			}
		}
		public uint TypeNamespace {
			get {
				return typeNamespace;
			}
			set {
				typeNamespace = value;
			}
		}
		public uint Implementation {
			get {
				return implementation;
			}
			set {
				implementation = value;
			}
		}
		
		
		public override void LoadRow()
		{
			flags         = binaryReader.ReadUInt32();
			typeDefId     = binaryReader.ReadUInt32();
			typeName      = LoadStringIndex();
			typeNamespace = LoadStringIndex();
			
			// todo 32 bit indices ?
			implementation = binaryReader.ReadUInt16();
		}
	}
}

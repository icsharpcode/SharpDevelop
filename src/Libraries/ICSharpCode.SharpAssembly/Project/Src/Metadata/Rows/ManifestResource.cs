// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class ManifestResource : AbstractRow
	{
		public static readonly int TABLE_ID = 0x28;
		
		public static readonly uint FLAG_VISIBILITYMASK = 0x0007;
		public static readonly uint FLAG_PUBLIC         = 0x0001;
		public static readonly uint FLAG_PRIVATE        = 0x0002;
		
		uint offset;
		uint flags;
		uint name; // index into String heap
		uint implementation; // index into File table, or AssemblyRef table, or  null; more precisely, an Implementation coded index
		
		public uint Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
		
		public uint Flags {
			get {
				return flags;
			}
			set {
				flags = value;
			}
		}
		
		public uint Name {
			get {
				return name;
			}
			set {
				name = value;
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
			offset         = binaryReader.ReadUInt32();
			flags          = binaryReader.ReadUInt32();
			name           = LoadStringIndex();
			implementation = ReadCodedIndex(CodedIndex.Implementation);
		}
	}
}

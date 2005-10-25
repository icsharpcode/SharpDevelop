// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class ImplMap : AbstractRow
	{
		public static readonly int TABLE_ID = 0x1C;
		
		public static readonly ushort FLAG_NOMANGLE          = 0x0001;
		public static readonly ushort FLAG_CHARSETMASK       = 0x0006;
		public static readonly ushort FLAG_CHARSETNOTSPEC    = 0x0000;
		public static readonly ushort FLAG_CHARSETANSI       = 0x0002;
		public static readonly ushort FLAG_CHARSETUNICODE    = 0x0004;
		public static readonly ushort FLAG_CHARSETAUTO       = 0x0006;
		public static readonly ushort FLAG_SUPPORTSLASTERROR = 0x0040;
		public static readonly ushort FLAG_CALLCONVMASK      = 0x0700;
		public static readonly ushort FLAG_CALLCONVWINAPI    = 0x0100;
		public static readonly ushort FLAG_CALLCONVCDECL     = 0x0200;
		public static readonly ushort FLAG_CALLCONVSTDCALL   = 0x0300;
		public static readonly ushort FLAG_CALLCONVTHISCALL  = 0x0400;
		public static readonly ushort FLAG_CALLCONVFASTCALL  = 0x0500;
		
		ushort mappingFlags;
		uint   memberForwarded; // index into the Field or Method table; more precisely, a MemberForwarded coded index.
		uint   importName; // index into the String heap
		uint   importScope; // index into the ModuleRef table
		
		public ushort MappingFlags {
			get {
				return mappingFlags;
			}
			set {
				mappingFlags = value;
			}
		}
		
		public uint MemberForwarded {
			get {
				return memberForwarded;
			}
			set {
				memberForwarded = value;
			}
		}
		
		public uint ImportName {
			get {
				return importName;
			}
			set {
				importName = value;
			}
		}
		
		public uint ImportScope {
			get {
				return importScope;
			}
			set {
				importScope = value;
			}
		}
		
		public override void LoadRow()
		{
			mappingFlags    = binaryReader.ReadUInt16();
			memberForwarded = ReadCodedIndex(CodedIndex.MemberForwarded);
			importName      = LoadStringIndex();
			importScope     = ReadSimpleIndex(ModuleRef.TABLE_ID);
		}
	}
}

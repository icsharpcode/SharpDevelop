// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class Event : AbstractRow
	{
		public static readonly int TABLE_ID = 0x14;
		
		public static readonly ushort FLAG_SPECIALNAME   = 0x0200;
		public static readonly ushort FLAG_RTSPECIALNAME = 0x0400;
		
		ushort eventFlags;
		uint   name;      // index into String heap
		uint   eventType; // index into TypeDef, TypeRef or TypeSpec tables; more precisely, a TypeDefOrRef coded index
		
		public ushort EventFlags {
			get {
				return eventFlags;
			}
			set {
				eventFlags = value;
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
		public uint EventType {
			get {
				return eventType;
			}
			set {
				eventType = value;
			}
		}
		
		public override void LoadRow()
		{
			eventFlags = binaryReader.ReadUInt16();
			name       = LoadStringIndex();
			eventType  = ReadCodedIndex(CodedIndex.TypeDefOrRef);
		}
	}
}

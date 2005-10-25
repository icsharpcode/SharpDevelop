// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class Property : AbstractRow
	{
		public static readonly int TABLE_ID = 0x17;
		
		public static readonly ushort FLAG_SPECIALNAME   = 0x0200;
		public static readonly ushort FLAG_RTSPECIALNAME = 0x0400;
		public static readonly ushort FLAG_HASDEFAULT    = 0x1000;
		public static readonly ushort FLAG_UNUSED        = 0xe9ff;
		
		ushort flags;
		uint   name; // index into String heap
		uint   type; // index into Blob heap
		
		public ushort Flags {
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
		
		public uint Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public bool IsFlagSet(uint flag)
		{
			return base.BaseIsFlagSet(this.flags, flag);
		}
		
		public override void LoadRow()
		{
			flags = binaryReader.ReadUInt16();
			name  = LoadStringIndex();
			type  = LoadBlobIndex();
		}
	}
}

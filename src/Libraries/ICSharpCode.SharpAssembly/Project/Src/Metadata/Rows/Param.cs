// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class Param : AbstractRow
	{
		public static readonly int    TABLE_ID = 0x08;
		
		public static readonly ushort FLAG_IN              = 0x0001;
		public static readonly ushort FLAG_OUT             = 0x0002;
		public static readonly ushort FLAG_OPTIONAL        = 0x0004;
		public static readonly ushort FLAG_HASDEFAULT      = 0x1000;
		public static readonly ushort FLAG_HASFIELDMARSHAL = 0x2000;
		public static readonly ushort FLAG_UNUSED          = 0xcfe0;
		
		ushort flags;
		ushort sequence;
		uint   name; // index into String heap
		
		public ushort Flags {
			get {
				return flags;
			}
			set {
				flags = value;
			}
		}
		
		public ushort Sequence {
			get {
				return sequence;
			}
			set {
				sequence = value;
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
		
		public bool IsFlagSet(uint flag)
		{
			return base.BaseIsFlagSet(this.flags, flag);
		}
		
		public override void LoadRow()
		{
			flags    = binaryReader.ReadUInt16();
			sequence = binaryReader.ReadUInt16();
			name     = LoadStringIndex();
		}
	}
}

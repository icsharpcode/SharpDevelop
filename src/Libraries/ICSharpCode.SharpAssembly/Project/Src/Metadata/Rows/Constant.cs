// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class Constant : AbstractRow
	{
		public static readonly int TABLE_ID = 0x0B;
		
		byte type;   // a 1 byte constant, followed by a 1-byte padding zero
		uint parent; // index into the Param or Field or Property table; more precisely, a HasConst coded index
		uint val;    // index into Blob heap
		
		public byte Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		public uint Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}
		public uint Val {
			get {
				return val;
			}
			set {
				val = value;
			}
		}
		
		public override void LoadRow()
		{
			type = binaryReader.ReadByte();
			byte paddingZero = binaryReader.ReadByte();
//			if (paddingZero != 0) {
//				Console.WriteLine("padding zero != 0");
//			}
			parent = ReadCodedIndex(CodedIndex.HasConstant);
			val    = LoadBlobIndex();
		}
	}
}

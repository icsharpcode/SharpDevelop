// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class ClassLayout : AbstractRow
	{
		public static readonly int TABLE_ID = 0x0F;
		
		ushort packingSize;
		uint   classSize;
		uint   parent; // index into TypeDef table
		
		public ushort PackingSize {
			get {
				return packingSize;
			}
			set {
				packingSize = value;
			}
		}
		public uint ClassSize {
			get {
				return classSize;
			}
			set {
				classSize = value;
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
		
		
		public override void LoadRow()
		{
			packingSize = binaryReader.ReadUInt16();
			classSize   = binaryReader.ReadUInt32();
			parent      = ReadSimpleIndex(TypeDef.TABLE_ID);
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class File : AbstractRow
	{
		public static readonly int TABLE_ID = 0x26;
		
		public static readonly uint FLAG_CONTAINSMETADATA   = 0x0000;
		public static readonly uint FLAG_CONTAINSNOMETADATA = 0x0001;

		uint flags;
		uint name; // index into String heap
		uint hashValue; // index into Blob heap
		
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
		public uint HashValue {
			get {
				return hashValue;
			}
			set {
				hashValue = value;
			}
		}
		
		public override void LoadRow()
		{
			flags     = binaryReader.ReadUInt32();
			name      = LoadStringIndex();
			hashValue = LoadBlobIndex();
		}
	}
}

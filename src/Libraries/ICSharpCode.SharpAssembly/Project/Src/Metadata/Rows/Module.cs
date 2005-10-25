// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class Module : AbstractRow
	{
		public static readonly int TABLE_ID = 0x00;
		
		ushort generation;
		uint   name;      // index into String heap
		uint   mvid;      // index into Guid heap
		uint   encid;     // index into Guid heap, reserved, shall be zero
		uint   encbaseid; // index into Guid heap, reserved, shall be zero
		
		public ushort Generation {
			get {
				return generation;
			}
			set {
				generation = value;
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
		public uint Mvid {
			get {
				return mvid;
			}
			set {
				mvid = value;
			}
		}
		public uint Encid {
			get {
				return encid;
			}
			set {
				encid = value;
			}
		}
		public uint Encbaseid {
			get {
				return encbaseid;
			}
			set {
				encbaseid = value;
			}
		}
		
		
		public override void LoadRow()
		{
			generation = binaryReader.ReadUInt16();
			name       = LoadStringIndex();
			mvid       = LoadGUIDIndex();
			encid      = LoadGUIDIndex();
			encbaseid  = LoadGUIDIndex();
		}
	}
}

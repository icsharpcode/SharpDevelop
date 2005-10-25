// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class MethodSemantics : AbstractRow
	{
		public static readonly int TABLE_ID = 0x18;
		
		public static readonly ushort SEM_SETTER   = 0x0001;
		public static readonly ushort SEM_GETTER   = 0x0002;
		public static readonly ushort SEM_OTHER    = 0x0004;
		public static readonly ushort SEM_ADDON    = 0x0008;
		public static readonly ushort SEM_REMOVEON = 0x0010;
		public static readonly ushort SEM_FIRE     = 0x0020;

		ushort semantics;
		uint   method;      // index into the Method table
		uint   association; // index into the Event or Property table; more precisely, a HasSemantics coded index
		
		public ushort Semantics {
			get {
				return semantics;
			}
			set {
				semantics = value;
			}
		}
		
		public uint Method {
			get {
				return method;
			}
			set {
				method = value;
			}
		}
		
		public uint Association {
			get {
				return association;
			}
			set {
				association = value;
			}
		}
		
		
		public override void LoadRow()
		{
			semantics   = binaryReader.ReadUInt16();
			method      = ReadSimpleIndex(ICSharpCode.SharpAssembly.Metadata.Rows.Method.TABLE_ID);
			association = ReadCodedIndex(CodedIndex.HasSemantics);
		}
	}
}

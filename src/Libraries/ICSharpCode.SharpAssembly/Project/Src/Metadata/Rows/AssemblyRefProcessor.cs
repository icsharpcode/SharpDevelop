// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class AssemblyRefProcessor : AbstractRow
	{
		public static readonly int TABLE_ID = 0x24;
		
		uint processor;
		uint assemblyRefIndex; // index into the AssemblyRef table
		
		public uint Processor {
			get {
				return processor;
			}
			set {
				processor = value;
			}
		}
		public uint AssemblyRefIndex {
			get {
				return assemblyRefIndex;
			}
			set {
				assemblyRefIndex = value;
			}
		}
		
		public override void LoadRow()
		{
			processor        = binaryReader.ReadUInt32();
			assemblyRefIndex = ReadSimpleIndex(AssemblyRef.TABLE_ID);
		}
	}
}

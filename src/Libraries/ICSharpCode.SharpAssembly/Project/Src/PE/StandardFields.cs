// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;

namespace ICSharpCode.SharpAssembly.PE {
	
	public class StandardFields
	{
		const ushort MAGIC = 0x10B;
		
		byte lMajor;
		byte lMinor;
		uint codeSize;
		uint initializedDataSize;
		uint uninitializedDataSize;
		uint entryPointRVA;
		uint baseOfCode;
		uint baseOfData;
		
		public void LoadFrom(BinaryReader binaryReader)
		{
			ushort magic = binaryReader.ReadUInt16();
			if (magic != MAGIC) {
				Console.WriteLine("Warning OptionalHeader.StandardFields != " + MAGIC + " was " + magic);
			}
			lMajor = binaryReader.ReadByte();
			Debug.Assert(lMajor == 6 || lMajor == 7);
			lMinor = binaryReader.ReadByte();
			Debug.Assert(lMinor == 0 || lMinor == 10);
			codeSize = binaryReader.ReadUInt32();
			initializedDataSize = binaryReader.ReadUInt32();
			uninitializedDataSize = binaryReader.ReadUInt32();
			entryPointRVA = binaryReader.ReadUInt32();
			baseOfCode = binaryReader.ReadUInt32();
			baseOfData = binaryReader.ReadUInt32();
		}
	}
	
}

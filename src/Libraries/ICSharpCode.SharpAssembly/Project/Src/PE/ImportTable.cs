// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.PE {
	
	public class ImportTable
	{
		const int UNUSED_SIZE = 20;
		
		uint importLookupTable  = 0;
		uint dateTimeStamp      = 0;
		uint forwarderChain     = 0;
		uint importTableName    = 0;
		uint importAddressTable = 0;
		byte[] unused           = new byte[UNUSED_SIZE];
		
		public void LoadFrom(BinaryReader binaryReader)
		{
			importLookupTable  = binaryReader.ReadUInt32();
			dateTimeStamp      = binaryReader.ReadUInt32();
			forwarderChain     = binaryReader.ReadUInt32();
			importTableName    = binaryReader.ReadUInt32();
			importAddressTable = binaryReader.ReadUInt32();
			binaryReader.Read(unused, 0, UNUSED_SIZE);
		}
	}
}

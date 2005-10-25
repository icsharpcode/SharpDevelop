// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.PE {
	
	public class NTSpecificFields
	{
		const uint IMAGE_BASE        = 0x400000;
		const uint SECTION_ALIGNMENT = 0x2000;
		
//		uint fileAlignment; // either 0x200 or 0x1000
//		ushort osMajor;
//		ushort osMinor;
//		ushort userMajor;
//		ushort userMinor;
//		ushort subSysMajor;
//		ushort subSysMinor;
//		uint   reserved;
//		uint   imageSize;
//		uint   headerSize;
//		uint   fileChecksum;
//		ushort subSystem;
//		ushort dllFlags;
//		uint   stackReserveSize;
//		uint   stackCommitSize;
//		uint   heapReserveSize;
//		uint   heapCommitSize;
//		uint   loaderFlags;
//		uint   numberOfDataDirectories;
		
		public void LoadFrom(BinaryReader binaryReader)
		{
			// TODO
			byte[] buffer = new byte[68];
			binaryReader.Read(buffer, 0, 68);
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.PE {
	
	public class StreamHeader
	{
		uint offset;
		uint size;
		string name;
		
		public uint Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
		
		public uint Size {
			get {
				return size;
			}
			set {
				size = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public void LoadFrom(BinaryReader binaryReader)
		{
			offset = binaryReader.ReadUInt32();
			size   = binaryReader.ReadUInt32();
			int bytesRead = 1;
			byte b = binaryReader.ReadByte();
			while (b != 0) {
				name += (char)b;
				b = binaryReader.ReadByte();
				++bytesRead;
			}
			// name is filled to 4 byte blocks
			int filler = bytesRead % 4 == 0 ? 0 :  4 - (bytesRead % 4);
			for (int i = 0; i < filler; ++i) {
				binaryReader.ReadByte();
			}
			
		}
	}
}

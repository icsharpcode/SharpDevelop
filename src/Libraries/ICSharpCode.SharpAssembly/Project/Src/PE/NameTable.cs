// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.PE {
	
	public class NameTable
	{
		ushort hint;
		string name;
		
		public ushort Hint {
			get {
				return hint;
			}
			set {
				hint = value;
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
			hint = binaryReader.ReadUInt16();
			
			name = String.Empty;
			byte b = binaryReader.ReadByte();
			while (b != 0) {
				name += (char)b;
				b = binaryReader.ReadByte();
			}
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.PE {
	
	public class IAT
	{
		uint nameTableRVA;
		uint empty;
		
		public uint NameTableRVA {
			get {
				return nameTableRVA;
			}
			set {
				nameTableRVA = value;
			}
		}
		public uint Empty {
			get {
				return empty;
			}
			set {
				empty = value;
			}
		}
		
		
		public void LoadFrom(BinaryReader binaryReader)
		{
			nameTableRVA = binaryReader.ReadUInt32();
			empty        = binaryReader.ReadUInt32();
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class AssemblyProcessor : AbstractRow
	{
		public static readonly int TABLE_ID = 0x21;
		
		uint processor;
		
		public uint Processor {
			get {
				return processor;
			}
			set {
				processor = value;
			}
		}
		
		public override void LoadRow()
		{
			processor = binaryReader.ReadUInt32();
		}
	}
}

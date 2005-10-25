// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class TypeSpec : AbstractRow
	{
		public static readonly int TABLE_ID = 0x1B;
		
		uint signature; // index into the Blob heap
		
		public uint Signature {
			get {
				return signature;
			}
			set {
				signature = value;
			}
		}
		
		public override void LoadRow()
		{
			signature = LoadBlobIndex();
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class NestedClass : AbstractRow
	{
		public static readonly int TABLE_ID = 0x29;
		
		uint nestedClass; // index into the TypeDef table
		uint enclosingClass; // index into the TypeDef table
		
		public uint NestedClassIndex {
			get {
				return nestedClass;
			}
			set {
				nestedClass = value;
			}
		}
		public uint EnclosingClass {
			get {
				return enclosingClass;
			}
			set {
				enclosingClass = value;
			}
		}
		
		public override void LoadRow()
		{
			nestedClass     = ReadSimpleIndex(TypeDef.TABLE_ID);
			enclosingClass  = ReadSimpleIndex(TypeDef.TABLE_ID);
		}
	}
}

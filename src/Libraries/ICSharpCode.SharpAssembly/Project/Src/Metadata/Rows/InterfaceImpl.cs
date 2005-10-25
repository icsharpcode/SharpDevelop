// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class InterfaceImpl : AbstractRow
	{
		public static readonly int TABLE_ID = 0x09;
		
		uint myClass; // index into the TypeDef table
		uint myInterface; // index into the TypeDef, TypeRef or TypeSpec table; more precisely, a TypeDefOrRef coded index
		
		public uint Class {
			get {
				return myClass;
			}
			set {
				myClass = value;
			}
		}
		
		public uint Interface {
			get {
				return myInterface;
			}
			set {
				myInterface = value;
			}
		}
		
		
		public override void LoadRow()
		{
			myClass     = ReadSimpleIndex(TypeDef.TABLE_ID);
			myInterface = ReadCodedIndex(CodedIndex.TypeDefOrRef);
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class MemberRef : AbstractRow
	{
		public static readonly int TABLE_ID = 0x0A;
		
		uint myClass;    // index into the TypeRef, ModuleRef, Method, TypeSpec or TypeDef tables; more precisely, a MemberRefParent coded index
		uint name;      // index into String heap
		uint signature; // index into Blob heap
		
		public uint Class {
			get {
				return myClass;
			}
			set {
				myClass = value;
			}
		}
		public uint Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
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
			myClass   = ReadCodedIndex(CodedIndex.MemberRefParent);
			name      = LoadStringIndex();
			signature = LoadBlobIndex();
		}
	}
}

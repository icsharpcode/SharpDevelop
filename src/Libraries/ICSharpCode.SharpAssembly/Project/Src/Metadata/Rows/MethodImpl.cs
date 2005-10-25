// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class MethodImpl : AbstractRow
	{
		public static readonly int TABLE_ID = 0x19;
		
		uint myClass;           // index into TypeDef table
		uint methodBody;        // index into Method or MemberRef table; more precisely, a MethodDefOrRef coded index
		uint methodDeclaration; // index into Method or MemberRef table; more precisely, a MethodDefOrRef coded index
		
		public uint MyClass {
			get {
				return myClass;
			}
			set {
				myClass = value;
			}
		}
		public uint MethodBody {
			get {
				return methodBody;
			}
			set {
				methodBody = value;
			}
		}
		public uint MethodDeclaration {
			get {
				return methodDeclaration;
			}
			set {
				methodDeclaration = value;
			}
		}
		
		public override void LoadRow()
		{
			myClass           = ReadSimpleIndex(TypeDef.TABLE_ID);
			methodBody        = ReadCodedIndex(CodedIndex.MethodDefOrRef);
			methodDeclaration = ReadCodedIndex(CodedIndex.MethodDefOrRef);
		}
	}
}

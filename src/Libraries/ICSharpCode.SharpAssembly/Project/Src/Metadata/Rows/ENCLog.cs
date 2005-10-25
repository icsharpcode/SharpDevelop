// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class ENCLog : AbstractRow
	{
		public static readonly int TABLE_ID = 0x1E;
		
		uint token;
		uint funcCode;
		
		public uint Token {
			get {
				return token;
			}
			set {
				token = value;
			}
		}
		public uint FuncCode {
			get {
				return funcCode;
			}
			set {
				funcCode = value;
			}
		}
		
		public override void LoadRow()
		{
			token    = binaryReader.ReadUInt32();
			funcCode = binaryReader.ReadUInt32();
		}
	}
}

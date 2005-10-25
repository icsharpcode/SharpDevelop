// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class MethodPtr : AbstractRow
	{
		public static readonly int TABLE_ID = 0x05;
		
		uint method;
		
		public uint Method {
			get {
				return method;
			}
			set {
				method = value;
			}
		}
		
		public override void LoadRow()
		{
			method = ReadSimpleIndex(ICSharpCode.SharpAssembly.Metadata.Rows.Method.TABLE_ID);
		}
	}
}

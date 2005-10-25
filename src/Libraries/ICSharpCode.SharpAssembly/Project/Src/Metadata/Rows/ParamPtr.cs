// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class ParamPtr : AbstractRow
	{
		public static readonly int TABLE_ID = 0x07;
		
		uint param;
		
		public uint Param {
			get {
				return param;
			}
			set {
				param = value;
			}
		}
		
		public override void LoadRow()
		{
			param = ReadSimpleIndex(ICSharpCode.SharpAssembly.Metadata.Rows.Param.TABLE_ID);
		}
	}
}

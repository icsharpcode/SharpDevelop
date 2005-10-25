// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class ModuleRef : AbstractRow
	{
		public static readonly int TABLE_ID = 0x1A;
		
		uint name;      // index into String heap
		
		public uint Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public override void LoadRow()
		{
			name  = LoadStringIndex();
		}
	}
}

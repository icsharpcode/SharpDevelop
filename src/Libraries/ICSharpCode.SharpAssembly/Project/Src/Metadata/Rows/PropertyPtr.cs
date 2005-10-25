// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class PropertyPtr : AbstractRow
	{
		public static readonly int TABLE_ID = 22;
		
		uint property;
		
		public uint Property {
			get {
				return property;
			}
			set {
				property = value;
			}
		}
		
		public override void LoadRow()
		{
			property = ReadSimpleIndex(ICSharpCode.SharpAssembly.Metadata.Rows.Property.TABLE_ID);
		}
	}
}

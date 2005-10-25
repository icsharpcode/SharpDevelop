// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class PropertyMap : AbstractRow
	{
		public static readonly int TABLE_ID = 0x15;
		
		uint parent;       // index into the TypeDef table
		uint propertyList; // index into Property table
		
		public uint Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}
		public uint PropertyList {
			get {
				return propertyList;
			}
			set {
				propertyList = value;
			}
		}
		
		
		public override void LoadRow()
		{
			parent       = ReadSimpleIndex(TypeDef.TABLE_ID);
			propertyList = ReadSimpleIndex(Property.TABLE_ID);
		}
	}
}

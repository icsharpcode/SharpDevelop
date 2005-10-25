// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class FieldMarshal : AbstractRow
	{
		public static readonly int TABLE_ID = 0x0D;
		
		uint parent;     // index into Field or Param table; more precisely, a HasFieldMarshal coded index
		uint nativeType; // index into the Blob heap
		
		public uint Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		
		}
		public uint NativeType {
			get {
				return nativeType;
			}
			set {
				nativeType = value;
			}
		}
		
		public override void LoadRow()
		{
			parent     = ReadCodedIndex(CodedIndex.HasFieldMarshall);
			nativeType = LoadBlobIndex();
		}
	}
}

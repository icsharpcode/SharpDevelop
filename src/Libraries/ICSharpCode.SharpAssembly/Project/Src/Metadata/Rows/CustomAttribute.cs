// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class CustomAttribute : AbstractRow
	{
		public static readonly int TABLE_ID = 0x0C;
		
		uint parent; // index into any metadata table, except the CustomAttribute table itself; more precisely, a HasCustomAttribute coded index
		uint type;   // index into the Method or MemberRef table; more precisely, a CustomAttributeType coded index
		uint val;    // index into Blob heap
		
		public uint Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}
		
		public uint Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public uint Val {
			get {
				return val;
			}
			set {
				val = value;
			}
		}
		
		public override void LoadRow()
		{
			parent  = ReadCodedIndex(CodedIndex.HasCustomAttribute);
			type    = ReadCodedIndex(CodedIndex.CustomAttributeType);
			val     = LoadBlobIndex();
		}
	}
}

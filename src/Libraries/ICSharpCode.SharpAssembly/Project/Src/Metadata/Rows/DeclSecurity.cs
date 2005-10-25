// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class DeclSecurity : AbstractRow
	{
		public static readonly int TABLE_ID = 0x0E;
		
	
		ushort action;
		uint   parent; // index into the TypeDef, Method or Assembly table; more precisely, a HasDeclSecurity coded index
		uint   permissionSet; // index into Blob heap
		
		public ushort Action {
			get {
				return action;
			}
			set {
				action = value;
			}
		}
		public uint Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}
		public uint PermissionSet {
			get {
				return permissionSet;
			}
			set {
				permissionSet = value;
			}
		}
		
		public override void LoadRow()
		{
			action        = binaryReader.ReadUInt16();
			parent        = ReadCodedIndex(CodedIndex.HasDeclSecurity);
			permissionSet = LoadBlobIndex();
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class TypeDef : AbstractRow
	{
		public static readonly int TABLE_ID = 0x02;
		
		// Visibility attributes
		public static readonly uint FLAG_VISIBILITYMASK    = 0x00000007;
		public static readonly uint FLAG_NOTPUBLIC         = 0x00000000; 
		public static readonly uint FLAG_PUBLIC            = 0x00000001;
		public static readonly uint FLAG_NESTEDPUBLIC      = 0x00000002;
		public static readonly uint FLAG_NESTEDPRIVATE     = 0x00000003;
		public static readonly uint FLAG_NESTEDFAMILY      = 0x00000004;
		public static readonly uint FLAG_NESTEDASSEMBLY    = 0x00000005;
		public static readonly uint FLAG_NESTEDFAMANDASSEM = 0x00000006;
		public static readonly uint FLAG_NESTEDFAMORASSEM  = 0x00000007;
		
		//Class layout attributes
		public static readonly uint FLAG_LAYOUTMASK       = 0x00000018;
		public static readonly uint FLAG_AUTOLAYOUT       = 0x00000000;
		public static readonly uint FLAG_SEQUENTIALLAYOUT = 0x00000008;
		public static readonly uint FLAG_EXPLICITLAYOUT   = 0x00000010;
		
		//Class semantics attributes
		public static readonly uint FLAG_CLASSSEMANTICSMASK = 0x00000020;
		public static readonly uint FLAG_CLASS              = 0x00000000;
		public static readonly uint FLAG_INTERFACE          = 0x00000020;
		
		// Special semantics in addition to class semantics
		public static readonly uint FLAG_ABSTRACT    = 0x00000080;
		public static readonly uint FLAG_SEALED      = 0x00000100;
		public static readonly uint FLAG_SPECIALNAME = 0x00000400;
		
		// Implementation Attributes
		public static readonly uint FLAG_IMPORT       = 0x00001000;
		public static readonly uint FLAG_SERIALIZABLE = 0x00002000;
		
		//String formatting Attributes
		public static readonly uint FLAG_STRINGFORMATMASK = 0x00030000;
		public static readonly uint FLAG_ANSICLASS        = 0x00000000;
		public static readonly uint FLAG_UNICODECLASS     = 0x00010000;
		public static readonly uint FLAG_AUTOCLASS        = 0x00020000;
		
		//Class Initialization Attributes
		public static readonly uint FLAG_BEFOREFIELDINIT  = 0x00100000;
		
		//Additional Flags
		public static readonly uint FLAG_RTSPECIALNAME = 0x00000800;
		public static readonly uint FLAG_HASSECURITY   = 0x00040000;
		
		uint flags;
		uint name;
		uint nSpace;
		uint extends;    // index into TypeDef, TypeRef or TypeSpec table; more precisely, a TypeDefOrRef coded index
		uint fieldList;  // index into Field table; it marks the first of a continguous run of Fields owned by this Type
		uint methodList; // index into Method table; it marks the first of a continguous run of Methods owned by this Type
		
		public uint Flags {
			get {
				return flags;
			}
			set {
				flags = value;
			}
		}
		public uint Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		public uint NSpace {
			get {
				return nSpace;
			}
			set {
				nSpace = value;
			}
		}
		public uint Extends {
			get {
				return extends;
			}
			set {
				extends = value;
			}
		}
		public uint FieldList {
			get {
				return fieldList;
			}
			set {
				fieldList = value;
			}
		}
		public uint MethodList {
			get {
				return methodList;
			}
			set {
				methodList = value;
			}
		}
		
		public bool IsFlagSet(uint flag)
		{
			return base.BaseIsFlagSet(this.flags, flag);
		}
		
		public bool IsMaskedFlagSet(uint flag, uint flag_mask)
		{
			return base.BaseIsFlagSet(this.flags, flag, flag_mask);
		}
		
		public override void LoadRow()
		{
			flags      = binaryReader.ReadUInt32();
			name       = LoadStringIndex();
			nSpace     = LoadStringIndex();
			extends    = ReadCodedIndex(CodedIndex.TypeDefOrRef);
			fieldList  = ReadSimpleIndex(Field.TABLE_ID);
			methodList = ReadSimpleIndex(Method.TABLE_ID);
		}
	}
}

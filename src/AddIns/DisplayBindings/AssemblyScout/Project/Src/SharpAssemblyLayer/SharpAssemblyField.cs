// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpAssembly.Metadata.Rows;
using ICSharpCode.SharpAssembly.Metadata;
using ICSharpCode.SharpAssembly.PE;
using ICSharpCode.SharpAssembly.Assembly;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	[Serializable]
	public class SharpAssemblyField : DefaultField
	{
		public override string DocumentationTag {
			get {
				return null;
			}
		}
		
		public SharpAssemblyField(SA.SharpAssembly assembly, Field[] fieldTable, SharpAssemblyClass declaringType, uint index) : base(declaringType, null)
		{
			if (assembly == null) {
				throw new System.ArgumentNullException("assembly");
			}
			if (fieldTable == null) {
				throw new System.ArgumentNullException("fieldTable");
			}
			if (declaringType == null) {
				throw new System.ArgumentNullException("declaringType");
			}
			if (index > fieldTable.GetUpperBound(0) || index < 0) {
				throw new System.ArgumentOutOfRangeException("index", index, String.Format("must be between 1 and {0}!", fieldTable.GetUpperBound(0)));
			}
						
			Field field = fieldTable[index];
			string name = assembly.Reader.GetStringFromHeap(field.Name);
			FullyQualifiedName = String.Concat(DeclaringType.FullyQualifiedName, ".", name);
			
			// Attributes
			ArrayList attrib = assembly.Attributes.Field[index] as ArrayList;
			if (attrib == null) goto noatt;
			
			foreach(SharpCustomAttribute customattribute in attrib) {
				Attributes.Add(new SharpAssemblyAttribute(assembly, customattribute));
			}
		
		noatt:
			
			if (field.IsFlagSet(Field.FLAG_INITONLY)) {
				Modifiers |= ModifierEnum.Readonly;
			}
			
			if (field.IsFlagSet(Field.FLAG_STATIC)) {
				Modifiers |= ModifierEnum.Static;
			}
						
			if (field.IsMaskedFlagSet(Field.FLAG_PRIVATE, Field.FLAG_FIELDACCESSMASK)) { // I assume that private is used most and public last (at least should be)
				Modifiers |= ModifierEnum.Private;
			} else if (field.IsMaskedFlagSet(Field.FLAG_FAMILY, Field.FLAG_FIELDACCESSMASK)) {
				Modifiers |= ModifierEnum.Protected;
			} else if (field.IsMaskedFlagSet(Field.FLAG_PUBLIC, Field.FLAG_FIELDACCESSMASK)) {
				Modifiers |= ModifierEnum.Public;
			} else if (field.IsMaskedFlagSet(Field.FLAG_ASSEMBLY, Field.FLAG_FIELDACCESSMASK)) {
				Modifiers |= ModifierEnum.Internal;
			} else if (field.IsMaskedFlagSet(Field.FLAG_FAMORASSEM, Field.FLAG_FIELDACCESSMASK)) {
				Modifiers |= ModifierEnum.ProtectedOrInternal;
			} else if (field.IsMaskedFlagSet(Field.FLAG_FAMANDASSEM, Field.FLAG_FIELDACCESSMASK)) {
				Modifiers |= ModifierEnum.Protected;
				Modifiers |= ModifierEnum.Internal;
			}
			
			if (field.IsFlagSet(Field.FLAG_LITERAL)) {
				Modifiers |= ModifierEnum.Const;
			}
			
			if (field.IsFlagSet(Field.FLAG_SPECIALNAME)) {
				Modifiers |= ModifierEnum.Extern | ModifierEnum.Unsafe | ModifierEnum.Volatile;
			}
			
			// field return type
			uint sigOffset = field.Signature;
			int sigSize = assembly.Reader.LoadBlob(ref sigOffset);
			sigOffset++;  // skip field id
			ReturnType = SharpAssemblyReturnType.Create(assembly, ref sigOffset);
			
			// field constant value -- for enums
			Constant cst = (Constant)assembly.FieldConstantTable[index];
			if (declaringType.ClassType == ClassType.Enum && cst != null) {
				try {
					DataType dt = (DataType)cst.Type;
					
					byte[] blob = assembly.Reader.GetBlobFromHeap(cst.Val);
					BinaryReader binReader = new BinaryReader(new MemoryStream(blob));
					
					switch (dt) {
						case DataType.Byte:
							initialValue = binReader.ReadByte();
							break;
						case DataType.Int16:
							initialValue = binReader.ReadInt16();
							break;
						case DataType.Int32:
							initialValue = binReader.ReadInt32();
							break;
						case DataType.Int64:
							initialValue = binReader.ReadInt64();
							break;
						case DataType.SByte:
							initialValue = binReader.ReadSByte();
							break;
						case DataType.UInt16:
							initialValue = binReader.ReadUInt16();
							break;
						case DataType.UInt32:
							initialValue = binReader.ReadUInt32();
							break;
						case DataType.UInt64:
							initialValue = binReader.ReadUInt64();
							break;
						default: // not supported
							break;
					}
					binReader.Close();
				} catch {
					Console.WriteLine("SharpAssemblyField: Error reading constant value");
				}
			}
		}
		
		public override string ToString()
		{
			return FullyQualifiedName;
		}
		
		object initialValue;
		
		public object InitialValue {
			get {
				return initialValue;
			}
		}
		
		public static bool IsSpecial(IField field)
		{
			return ((field.Modifiers & ModifierEnum.Extern) == ModifierEnum.Extern) ||
				((field.Modifiers & ModifierEnum.Volatile) == ModifierEnum.Volatile) ||
				((field.Modifiers & ModifierEnum.Unsafe) == ModifierEnum.Unsafe);
		}
	}
}

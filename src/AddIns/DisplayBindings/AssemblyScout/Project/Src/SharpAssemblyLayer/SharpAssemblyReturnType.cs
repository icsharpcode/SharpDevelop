// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpAssembly.Metadata.Rows;
using ICSharpCode.SharpAssembly.Metadata;
using ICSharpCode.SharpAssembly.PE;
using ICSharpCode.SharpAssembly.Assembly;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	[Serializable]
	public class SharpAssemblyReturnType : DefaultReturnType
	{
		object declaredIn;
		
		SharpAssemblyClass underlyingClass;
		
		public SharpAssemblyClass UnderlyingClass {
			get {
				return underlyingClass;
			}
		}
		
		public object DeclaredIn {
			get {
				return declaredIn;
			}
		}
			
		public override string ToString()
		{
			return FullyQualifiedName;
		}
		
		public static SharpAssemblyReturnType Create(string name)
		{
			return new SharpAssemblyReturnType(name);
		}
		
		public static SharpAssemblyReturnType Create(SA.SharpAssembly assembly, TypeRef[] typeRefTable, uint index)
		{
			string fullyQualifiedName = String.Empty;
			SharpAssemblyClass underlyingClass = SharpAssemblyClass.FromTypeRef(assembly, index);
			if (underlyingClass != null) {
				fullyQualifiedName = underlyingClass.FullyQualifiedName;
			} else {
				fullyQualifiedName = assembly.Reader.GetStringFromHeap(typeRefTable[index].Nspace) + "." + 
			                                                assembly.Reader.GetStringFromHeap(typeRefTable[index].Name);
			    LoggingService.Warn("SharpAssemblyReturnType from TypeRef: TypeRef not resolved!");
			}
			
			return new SharpAssemblyReturnType(fullyQualifiedName, underlyingClass, assembly.GetRefAssemblyFor(index));
		}
		
		public static SharpAssemblyReturnType Create(SA.SharpAssembly assembly, TypeDef[] typeDefTable, uint index)
		{
			string fullyQualifiedName = String.Empty;
			SharpAssemblyClass underlyingClass = SharpAssemblyClass.FromTypeDef(assembly, index);
			if (underlyingClass != null) {
				fullyQualifiedName = underlyingClass.FullyQualifiedName;
			} else {
				fullyQualifiedName = assembly.Reader.GetStringFromHeap(typeDefTable[index].NSpace) + "." + 
														assembly.Reader.GetStringFromHeap(typeDefTable[index].Name);
			}
			
			return new SharpAssemblyReturnType(fullyQualifiedName, underlyingClass, assembly);
		}
		
		public static SharpAssemblyReturnType Create(SA.SharpAssembly assembly, ref uint blobSignatureIndex)
		{
			ArrayList arrayRanks = new ArrayList();
			string fullyQualifiedName = String.Empty;
			SA.SharpAssembly declaredIn = null;
			SharpAssemblyClass underlyingClass = null;
			
			try {
				GetDataType(assembly, ref blobSignatureIndex, ref arrayRanks, ref fullyQualifiedName, ref underlyingClass, ref declaredIn);
			} catch (Exception e) {
				LoggingService.Error("Got exception in ReturnType creation: " + e.ToString());
				fullyQualifiedName = "GOT_EXCEPTION";
			}
			
//			if (this.arrayRanks.Count > 0) {
//				arrayDimensions = new int[arrayRanks.Count];
//				arrayRanks.CopyTo(arrayDimensions, 0);
//			} else {
//				arrayRanks = null;
//			}
			
			return new SharpAssemblyReturnType(fullyQualifiedName, underlyingClass, declaredIn);
		}
		
		SharpAssemblyReturnType(string name) : base(new DefaultClass(null, name))
		{
		}
		
		SharpAssemblyReturnType(string name, SharpAssemblyClass underlyingClass, SA.SharpAssembly declaredIn) : this(name)
		{
			this.declaredIn = declaredIn;
			this.underlyingClass = underlyingClass;
		}
		
		static void GetDataType(SA.SharpAssembly asm, ref uint offset, ref ArrayList arrayRanks, ref string fullyQualifiedName, ref SharpAssemblyClass underlyingClass, ref SA.SharpAssembly declaredIn)
		{
			AssemblyReader assembly = asm.Reader;
			DataType dt = (DataType)assembly.LoadBlob(ref offset);
			switch (dt) {
				case DataType.Void:
				case DataType.Boolean:
				case DataType.Char:
				case DataType.SByte:
				case DataType.Byte:
				case DataType.Int16:
				case DataType.UInt16:
				case DataType.Int32:
				case DataType.UInt32:
				case DataType.Int64:
				case DataType.UInt64:
				case DataType.Single:
				case DataType.Double:
				case DataType.String:
				case DataType.Object:
				case DataType.IntPtr:
				case DataType.UIntPtr:
					fullyQualifiedName = "System." + dt.ToString();
					
					declaredIn = asm.GetReference("mscorlib");
					break;
				
				case DataType.SZArray:
					GetDataType(asm, ref offset, ref arrayRanks, ref fullyQualifiedName, ref underlyingClass, ref declaredIn);
					arrayRanks.Add(0);
					break;
				
				case DataType.Array:
					GetDataType(asm, ref offset, ref arrayRanks, ref fullyQualifiedName, ref underlyingClass, ref declaredIn);
					int rank      = assembly.LoadBlob(ref offset);
					int num_sizes = assembly.LoadBlob(ref offset);
					int[] sizes   = new int[num_sizes];
					for (int i = 0; i < num_sizes; ++i) {
						sizes[i] = assembly.LoadBlob(ref offset);
					}
					int num_lowerBounds = assembly.LoadBlob(ref offset);
					int[] lowerBounds   = new int[num_lowerBounds];
					for (int i = 0; i < num_lowerBounds; ++i) {
						lowerBounds[i] = assembly.LoadBlob(ref offset);
					}
					arrayRanks.Add(rank - 1);
					break;
				
				case DataType.ValueType:
				case DataType.Class:
					uint idx = (uint)assembly.LoadBlob(ref offset);
					bool isTypeRef = (idx & 1) == 1;
					uint  index    = (idx >> 2);
					
					TypeDef[] typeDefTable = asm.Tables.TypeDef;
					TypeRef[] typeRefTable = asm.Tables.TypeRef;
					
					if (isTypeRef) {
						underlyingClass = SharpAssemblyClass.FromTypeRef(asm, index);
						if (underlyingClass != null) {
							fullyQualifiedName = underlyingClass.FullyQualifiedName;
						} else {
							fullyQualifiedName = assembly.GetStringFromHeap(typeRefTable[index].Nspace) + "." + 
						                                                assembly.GetStringFromHeap(typeRefTable[index].Name);
						    LoggingService.Warn("GetDataType: TypeRef not resolved!");
						}
						declaredIn = asm.GetRefAssemblyFor(index);
					} else {
						underlyingClass = SharpAssemblyClass.FromTypeDef(asm, index);
						if (underlyingClass != null) {
							fullyQualifiedName = underlyingClass.FullyQualifiedName;
						} else {
							fullyQualifiedName = assembly.GetStringFromHeap(typeDefTable[index].NSpace) + "." + 
																	assembly.GetStringFromHeap(typeDefTable[index].Name);
						}
						declaredIn = asm;
					}

					break;
				
				case DataType.Ptr:
					GetDataType(asm, ref offset, ref arrayRanks, ref fullyQualifiedName, ref underlyingClass, ref declaredIn);
					break;
				case DataType.ByRef:
					GetDataType(asm, ref offset, ref arrayRanks, ref fullyQualifiedName, ref underlyingClass, ref declaredIn);
					fullyQualifiedName += "&";
					break;
				
				case DataType.TypeReference:
					fullyQualifiedName = "typedref";
					break;
				
				case DataType.Pinned:
					GetDataType(asm, ref offset, ref arrayRanks, ref fullyQualifiedName, ref underlyingClass, ref declaredIn);
					//fullyQualifiedName += " pinned";
					break;
				
				case DataType.CModOpt:
				case DataType.CModReq:
					GetDataType(asm, ref offset, ref arrayRanks, ref fullyQualifiedName, ref underlyingClass, ref declaredIn);
					break;
				
				default:
					//Console.WriteLine("NOT supported: " + dt.ToString());
					fullyQualifiedName += " NOT_SUPPORTED [" + dt.ToString() + "]";
					break;
			}
		}
	}
}

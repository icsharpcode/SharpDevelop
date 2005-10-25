// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Diagnostics;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Xml;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpAssembly.Metadata.Rows;
using ICSharpCode.SharpAssembly.Metadata;
using ICSharpCode.SharpAssembly.PE;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	[Serializable]
	public class SharpAssemblyMethod : DefaultMethod
	{
		public override string DocumentationTag {
			get {
				return null;
			}
		}
		
		public SharpAssemblyMethod(SA.SharpAssembly asm, Method[] methodTable, SharpAssemblyClass declaringType, uint index) : base(declaringType, null)
		{
			if (asm == null) {
				throw new System.ArgumentNullException("asm");
			}
			if (methodTable == null) {
				throw new System.ArgumentNullException("methodTable");
			}
			if (declaringType == null) {
				throw new System.ArgumentNullException("declaringType");
			}
			if (index > methodTable.GetUpperBound(0) || index < 1) {
				throw new System.ArgumentOutOfRangeException("index", index, String.Format("must be between 1 and {0}!", methodTable.GetUpperBound(0)));
			}
			SA.AssemblyReader assembly = asm.Reader;
			
			Method methodDef = methodTable[index];
			string name = assembly.GetStringFromHeap(methodDef.Name);
			
			FullyQualifiedName = String.Concat(DeclaringType.FullyQualifiedName, ".", name);
			
			// Attributes
 			ArrayList attrib = asm.Attributes.Method[index] as ArrayList;
			if (attrib == null) goto noatt;
						
			foreach(SA.SharpCustomAttribute customattribute in attrib) {
				Attributes.Add(new SharpAssemblyAttribute(asm, customattribute));
			}
		
		noatt:
			
			Modifiers = ModifierEnum.None;
			
			if (methodDef.IsFlagSet(Method.FLAG_STATIC)) {
				Modifiers |= ModifierEnum.Static;
			}
			
			if (methodDef.IsMaskedFlagSet(Method.FLAG_PRIVATE, Method.FLAG_MEMBERACCESSMASK)) {
				Modifiers |= ModifierEnum.Private;
			} else if (methodDef.IsMaskedFlagSet(Method.FLAG_PUBLIC, Method.FLAG_MEMBERACCESSMASK)) {
				Modifiers |= ModifierEnum.Public;
			} else if (methodDef.IsMaskedFlagSet(Method.FLAG_FAMILY, Method.FLAG_MEMBERACCESSMASK)) {
				Modifiers |= ModifierEnum.Protected;
			} else if (methodDef.IsMaskedFlagSet(Method.FLAG_ASSEM, Method.FLAG_MEMBERACCESSMASK)) {
				Modifiers |= ModifierEnum.Internal;
			} else if (methodDef.IsMaskedFlagSet(Method.FLAG_FAMORASSEM, Method.FLAG_MEMBERACCESSMASK)) {
				Modifiers |= ModifierEnum.ProtectedOrInternal;
			} else if (methodDef.IsMaskedFlagSet(Method.FLAG_FAMANDASSEM, Method.FLAG_MEMBERACCESSMASK)) {
				Modifiers |= ModifierEnum.Protected;
				Modifiers |= ModifierEnum.Internal;
			}
			
			if (methodDef.IsFlagSet(Method.FLAG_VIRTUAL)) {
				Modifiers |= ModifierEnum.Virtual;
			}
			
			if (methodDef.IsFlagSet(Method.FLAG_FINAL)) {
				Modifiers |= ModifierEnum.Default;
			}
			
			if (methodDef.IsFlagSet(Method.FLAG_ABSTRACT)) {
				Modifiers |= ModifierEnum.Abstract;
			}
			
			if (methodDef.IsFlagSet(Method.FLAG_SPECIALNAME)) {
				Modifiers |= ModifierEnum.Extern | ModifierEnum.Volatile | ModifierEnum.Unsafe;
			}
			
			uint offset = methodDef.Signature;
			int size = assembly.LoadBlob(ref offset);
			offset += 1;  // skip calling convention
			int numReturnTypes = assembly.LoadBlob(ref offset);
					
			ReturnType = SharpAssemblyReturnType.Create(asm, ref offset);
			
			IReturnType[] returnTypes = new IReturnType[numReturnTypes];
			for (int i = 0; i < returnTypes.Length; ++i) {
				returnTypes[i] = SharpAssemblyReturnType.Create(asm, ref offset);
			}
			
			AddParameters(asm, methodTable, index, returnTypes);
		}
		
		public static bool IsSpecial(IMethod method)
		{
			return ((method.Modifiers & ModifierEnum.Extern) == ModifierEnum.Extern) ||
				((method.Modifiers & ModifierEnum.Volatile) == ModifierEnum.Volatile) ||
				((method.Modifiers & ModifierEnum.Unsafe) == ModifierEnum.Unsafe);
		}
		
		void AddParameters(SA.SharpAssembly asm, Method[] methodDefTable, uint index, IReturnType[] returnTypes)
		{
			Param[] paramTable = asm.Tables.Param;
			if (paramTable == null) return;
			
			uint paramIndexStart = methodDefTable[index].ParamList;
			
			// 0 means no parameters
			if (paramIndexStart > paramTable.GetUpperBound(0) || paramIndexStart == 0) {
				return;
			}
			
			uint paramIndexEnd   = (uint)paramTable.GetUpperBound(0);
			if (index < methodDefTable.GetUpperBound(0)) {
				paramIndexEnd = methodDefTable[index + 1].ParamList;
			}
			
			if (paramTable[paramIndexStart].Sequence == 0) paramIndexStart++;
			
			for (uint i = paramIndexStart; i < paramIndexEnd; ++i) {
				uint j = (i - paramIndexStart);
				Parameters.Add(new SharpAssemblyParameter(asm, paramTable, i, j < returnTypes.Length ? returnTypes[j] : null));
			}
		}
		
		public override bool IsConstructor {
			get {
				return FullyQualifiedName.IndexOf("..") != -1;
			}
		}
		
		public override string ToString()
		{
			return FullyQualifiedName;
		}
	}
}


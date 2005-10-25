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

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpAssembly.Metadata.Rows;
using ICSharpCode.SharpAssembly.Metadata;
using ICSharpCode.SharpAssembly.PE;
using ICSharpCode.SharpAssembly.Assembly;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	[Serializable]
	public class SharpAssemblyProperty : DefaultProperty
	{
		public override string DocumentationTag {
			get {
				return null;
			}
		}
		
		public SharpAssemblyProperty(SA.SharpAssembly asm, Property[] propertyTable, SharpAssemblyClass declaringType, uint index) : base(declaringType, String.Empty)
		{
			if (asm == null) {
				throw new System.ArgumentNullException("asm");
			}
			if (propertyTable == null) {
				throw new System.ArgumentNullException("propertyTable");
			}
			if (declaringType == null) {
				throw new System.ArgumentNullException("declaringType");
			}
			if (index > propertyTable.GetUpperBound(0) || index < 1) {
				throw new System.ArgumentOutOfRangeException("index", index, String.Format("must be between 1 and {0}!", propertyTable.GetUpperBound(0)));
			}
			
			AssemblyReader assembly = asm.Reader;
			
			Property property = asm.Tables.Property[index];
			string name = assembly.GetStringFromHeap(property.Name);
			FullyQualifiedName = String.Concat(DeclaringType.FullyQualifiedName, ".", name);
			
			MethodSemantics[] sem = (MethodSemantics[])assembly.MetadataTable.Tables[MethodSemantics.TABLE_ID];
			Method[] method       = (Method[])assembly.MetadataTable.Tables[Method.TABLE_ID];
			
			uint getterMethodIndex = 0; // used later for parameters
			
			if (sem == null) goto nosem;
			
			for (int i = 1; i <= sem.GetUpperBound(0); ++i) {
				uint table = sem[i].Association & 1;
				uint ident = sem[i].Association >> 1;
				
				if (table == 1 && ident == index) {  // table: Property
					Modifiers = ModifierEnum.None;
					Method methodDef = method[sem[i].Method];
			
					if (methodDef.IsFlagSet(Method.FLAG_STATIC)) {
						Modifiers |= ModifierEnum.Static;
					}
					
					if (methodDef.IsFlagSet(Method.FLAG_ABSTRACT)) {
						Modifiers |= ModifierEnum.Abstract;
					}
					
					if (methodDef.IsFlagSet(Method.FLAG_VIRTUAL)) {
						Modifiers |= ModifierEnum.Virtual;
					}
 					if (methodDef.IsFlagSet(Method.FLAG_FINAL)) {
						Modifiers |= ModifierEnum.Default;
					}
					
					if (methodDef.IsMaskedFlagSet(Method.FLAG_PRIVATE, Method.FLAG_MEMBERACCESSMASK)) { // I assume that private is used most and public last (at least should be)
						Modifiers |= ModifierEnum.Private;
					} else if (methodDef.IsMaskedFlagSet(Method.FLAG_FAMILY, Method.FLAG_MEMBERACCESSMASK)) {
						Modifiers |= ModifierEnum.Protected;
					} else if (methodDef.IsMaskedFlagSet(Method.FLAG_PUBLIC, Method.FLAG_MEMBERACCESSMASK)) {
						Modifiers |= ModifierEnum.Public;
					} else if (methodDef.IsMaskedFlagSet(Method.FLAG_ASSEM, Method.FLAG_MEMBERACCESSMASK)) {
						Modifiers |= ModifierEnum.Internal;
					} else if (methodDef.IsMaskedFlagSet(Method.FLAG_FAMORASSEM, Method.FLAG_MEMBERACCESSMASK)) {
						Modifiers |= ModifierEnum.ProtectedOrInternal;
					} else if (methodDef.IsMaskedFlagSet(Method.FLAG_FAMANDASSEM, Method.FLAG_MEMBERACCESSMASK)) {
						Modifiers |= ModifierEnum.Protected;
						Modifiers |= ModifierEnum.Internal;
					}

					
					if ((sem[i].Semantics & MethodSemantics.SEM_GETTER) == MethodSemantics.SEM_GETTER) {
						GetterRegion = new DomRegion(0, 0, 0, 0);
						// TODO GetterMethod missing.
//						GetterMethod = new SharpAssemblyMethod(asm, method, declaringtype, sem[i].Method);
//						GetterMethodIndex = sem[i].Method;
					}
					
					if ((sem[i].Semantics & MethodSemantics.SEM_SETTER) == MethodSemantics.SEM_SETTER) {
						SetterRegion = new DomRegion(0, 0, 0, 0);
						// TODO SetterMethod missing.
//						SetterMethod = new SharpAssemblyMethod(asm, method, declaringtype, sem[i].Method);
					}
				}
				
			}
			
		nosem:
			
			// Attributes
			ArrayList attrib = asm.Attributes.Property[index] as ArrayList;
			if (attrib == null) goto noatt;
						
			foreach(SA.SharpCustomAttribute customattribute in attrib) {
				Attributes.Add(new SharpAssemblyAttribute(asm, customattribute));
			}
		
		noatt:
			
			if ((property.Flags & Property.FLAG_SPECIALNAME) == Property.FLAG_SPECIALNAME) Modifiers |= ModifierEnum.Extern | ModifierEnum.Volatile | ModifierEnum.Unsafe;
			
			uint offset = property.Type;
			int sigSize = assembly.LoadBlob(ref offset);
			offset += 1; // skip calling convention
			int paramCount = assembly.LoadBlob(ref offset);
			
			ReturnType = SharpAssemblyReturnType.Create(asm, ref offset);
	
			IReturnType[] returnTypes = new IReturnType[paramCount];
			for (int i = 0; i < returnTypes.Length; ++i) {
				returnTypes[i] = SharpAssemblyReturnType.Create(asm, ref offset);
			}
			
			if (getterMethodIndex != 0) {
				AddParameters(asm, asm.Tables.Method, getterMethodIndex, returnTypes);
			} else {
				AddParameters(asm, returnTypes);
			}
		}
		
		void AddParameters(SA.SharpAssembly asm, Method[] methodTable, uint index, IReturnType[] returnTypes)
		{
			Param[] paramTable = asm.Tables.Param;
			if (paramTable == null) return;
			
			uint paramIndexStart = methodTable[index].ParamList;
			
			// 0 means no parameters
			if (paramIndexStart > paramTable.GetUpperBound(0) || paramIndexStart == 0) {
				return;
			}
			
			uint paramIndexEnd   = (uint)paramTable.GetUpperBound(0);
			if (index < methodTable.GetUpperBound(0)) {
				paramIndexEnd = methodTable[index + 1].ParamList;
			}
			
			if (paramTable[paramIndexStart].Sequence == 0) paramIndexStart++;
			
			for (uint i = paramIndexStart; i < paramIndexEnd; ++i) {
				uint j = (i - paramIndexStart);
				Parameters.Add(new SharpAssemblyParameter(asm, paramTable, i, j < returnTypes.Length ? returnTypes[j] : null));
			}
		}
		
		void AddParameters(SA.SharpAssembly asm, IReturnType[] returnTypes)
		{
			for (uint i = 0; i < returnTypes.GetUpperBound(0); ++i) {
				Parameters.Add(new SharpAssemblyParameter(asm, "param_" + i, returnTypes[i]));
			}
		}
		
		public override string ToString()
		{
			return FullyQualifiedName;
		}
	}
}

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
	public class SharpAssemblyEvent : DefaultEvent
	{
		public override string DocumentationTag {
			get {
				return null;
			}
		}
		
		public SharpAssemblyEvent(SA.SharpAssembly asm, Event[] eventTable, SharpAssemblyClass declaringType, uint index) : base(declaringType, null)
		{
			if (asm == null) {
				throw new System.ArgumentNullException("asm");
			}
			if (eventTable == null) {
				throw new System.ArgumentNullException("eventTable");
			}
			if (declaringType == null) {
				throw new System.ArgumentNullException("declaringtype");
			}
			if (index > eventTable.GetUpperBound(0) || index < 1) {
				throw new System.ArgumentOutOfRangeException("index", index, String.Format("must be between 1 and {0}!", eventTable.GetUpperBound(0)));
			}
			
			AssemblyReader assembly = asm.Reader;
						
			Event evt = eventTable[index];
			string name = assembly.GetStringFromHeap(evt.Name);
			FullyQualifiedName = String.Concat(DeclaringType.FullyQualifiedName, ".", name);
			
			MethodSemantics[] sem = asm.Tables.MethodSemantics;
			Method[] method       = asm.Tables.Method;
			if (sem == null) goto nosem;
			
			for (int i = 1; i <= sem.GetUpperBound(0); ++i) {
				uint table = sem[i].Association & 1;
				uint ident = sem[i].Association >> 1;
				
				if (table == 0 && ident == index) {  // table: Event
					Modifiers = ModifierEnum.None;
					Method methodDef = method[sem[i].Method];
			
					if (methodDef.IsFlagSet(Method.FLAG_STATIC)) {
						Modifiers |= ModifierEnum.Static;
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
					
					if ((sem[i].Semantics & MethodSemantics.SEM_ADDON) == MethodSemantics.SEM_ADDON) {
						addMethod = new SharpAssemblyMethod(asm, method, declaringType, sem[i].Method);
					}
					
					if ((sem[i].Semantics & MethodSemantics.SEM_REMOVEON) == MethodSemantics.SEM_REMOVEON) {
						removeMethod = new SharpAssemblyMethod(asm, method, declaringType, sem[i].Method);
					}

					if ((sem[i].Semantics & MethodSemantics.SEM_FIRE) == MethodSemantics.SEM_FIRE) {
						raiseMethod = new SharpAssemblyMethod(asm, method, declaringType, sem[i].Method);
					}
				}
				
			}
			
		nosem:
			
			// Attributes
			ArrayList attrib = asm.Attributes.Event[index] as ArrayList;
			if (attrib == null) goto noatt;
			
			foreach(SharpCustomAttribute customattribute in attrib) {
				Attributes.Add(new SharpAssemblyAttribute(asm, customattribute));
			}
		
		noatt:
			
			if ((evt.EventFlags & Event.FLAG_SPECIALNAME) == Event.FLAG_SPECIALNAME) Modifiers |= ModifierEnum.Extern | ModifierEnum.Volatile | ModifierEnum.Unsafe;
			
			uint typtab = evt.EventType & 0x03;
			uint typid  = evt.EventType >> 2;
			
			if (typtab == 0) {        // TypeDef
				TypeDef[] typedef = (TypeDef[])assembly.MetadataTable.Tables[TypeDef.TABLE_ID];
				ReturnType = SharpAssemblyReturnType.Create(asm, typedef, typid);

			} else if (typtab == 1) { // TypeRef
				TypeRef[] typeref = (TypeRef[])assembly.MetadataTable.Tables[TypeRef.TABLE_ID];
				ReturnType = SharpAssemblyReturnType.Create(asm, typeref, typid);

			} else {                  // TypeSpec
				ReturnType = SharpAssemblyReturnType.Create("NOT_SUPPORTED");
				Console.WriteLine("SharpAssemblyEvent: TypeSpec -- not supported");
			}
			
		}
		
		public override string ToString()
		{
			return FullyQualifiedName;
		}
	}
}

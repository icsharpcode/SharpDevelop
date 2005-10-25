// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.Specialized;

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
	public class SharpAssemblyClass : DefaultClass
	{
		List<IClass> baseTypeCollection = new List<IClass>();
		
		object           declaredIn;

		public object DeclaredIn {
			get {
				return declaredIn;
			}
		}
		
		public override string DocumentationTag {
			get {
				return null;
			}
		}
		
		public List<IClass> BaseTypeCollection {
			get {
				if (baseTypeCollection == null) baseTypeCollection = new List<IClass>();
				return baseTypeCollection;
			}
		}
		
		public static string GetNestedName(SA.SharpAssembly asm, TypeRef[] typeRefTable, uint index)
		{
			uint val = typeRefTable[index].ResolutionScope;
			int table = asm.Reader.GetCodedIndexTable(CodedIndex.ResolutionScope, ref val);
			
			switch (table) {
				case 2: // AssemblyRef
					return asm.Reader.GetStringFromHeap(typeRefTable[index].Nspace) + "." + asm.Reader.GetStringFromHeap(typeRefTable[index].Name);
				case 3: // TypeRef -- nested type
					return GetNestedName(asm, typeRefTable, val) + "+" + asm.Reader.GetStringFromHeap(typeRefTable[index].Name);
				default: // other token - not supported
					Console.WriteLine("GetNestedName: Unsupported resolution scope!");
					goto case 3;
			}
		}
		
		public static string GetNestedName(SA.SharpAssembly asm, TypeDef[] typeDefTable, uint index)
		{
			uint nestedParent = asm.GetNestedTypeParent(index);
			
			if (nestedParent == 0) {
				return asm.Reader.GetStringFromHeap(typeDefTable[index].NSpace) + "." + asm.Reader.GetStringFromHeap(typeDefTable[index].Name);
			}
			
			return GetNestedName(asm, typeDefTable, nestedParent) + "+" + asm.Reader.GetStringFromHeap(typeDefTable[index].Name);
		}
		
		/// <summary>
		/// Constructs a SharpAssemblyClass from an entry in the assembly's TypeRef table
		/// by looking in the referencing assembly's TypeDef table
		/// </summary>
		public static SharpAssemblyClass FromTypeRef(SA.SharpAssembly referencingAssembly, uint index)
		{
			if (referencingAssembly.TypeRefObjects[index] as SharpAssemblyClass != null) {
				return (SharpAssemblyClass)referencingAssembly.TypeRefObjects[index];
			}
			
			TypeRef[] typeRefTable = referencingAssembly.Tables.TypeRef;
			
			string name = referencingAssembly.Reader.GetStringFromHeap(typeRefTable[index].Name);
			
			SA.SharpAssembly declaringAssembly = referencingAssembly.GetRefAssemblyFor(index);
			if (declaringAssembly == null) {
				Console.Write("FromTypeRef failed for: " + name + " declared in assembly " + referencingAssembly.Name);
				Console.WriteLine(": Declaring assembly not found.");
				return null;
			}
			
			
			TypeDef[] typeDefTable = declaringAssembly.Tables.TypeDef;
			if (typeDefTable == null) {
				return null;
			}
			
			string nestedName = GetNestedName(referencingAssembly, typeRefTable, index);
			
			for (uint i = 1; i <= typeDefTable.GetUpperBound(0); ++i) {
				if (declaringAssembly.Reader.GetStringFromHeap(typeDefTable[i].Name) == name) {
					if (GetNestedName(declaringAssembly, typeDefTable, i) == nestedName) {
						SharpAssemblyClass newclass = FromTypeDef(declaringAssembly, i);
						
						// store new class object in assembly's cache
						if (newclass != null) referencingAssembly.TypeRefObjects[index] = newclass;
						return newclass;
					}
				}
			}
			
			Console.Write("FromTypeRef failed for: " + name + " declared in assembly " + referencingAssembly.Name);
			Console.WriteLine(": Matching type not found for nested name: " + nestedName);
			return null;
		}
		
		/// <summary>
		/// Constructs a SharpAssemblyClass from an entry in the assembly's TypeDef table
		/// Looks in the class cache for the assembly first
		/// </summary>
		public static SharpAssemblyClass FromTypeDef(SA.SharpAssembly assembly, uint index)
		{
			if (assembly.TypeDefObjects[index] as SharpAssemblyClass != null) {
				SharpAssemblyClass exclass = (SharpAssemblyClass)assembly.TypeDefObjects[index];
				
				return exclass;
			}
			
			return new SharpAssemblyClass(assembly, assembly.Tables.TypeDef, index);
		}
		
		/// <summary>
		/// The constructor is private because the only way to construct SharpAssemblyClass objects
		/// is to call FromTypeRef/Def to make us of the cache
		/// </summary>
		SharpAssemblyClass(SA.SharpAssembly assembly, TypeDef[] typeDefTable, uint index) : base(null, String.Empty)
		{
			if (assembly == null) {
				throw new System.ArgumentNullException("assembly");
			}
			if (typeDefTable == null) {
				throw new System.ArgumentNullException("typeDefTable");
			}
			if (index > typeDefTable.GetUpperBound(0) || index < 1) {
				throw new System.ArgumentOutOfRangeException("index", index, String.Format("must be between 1 and {0}!", typeDefTable.GetUpperBound(0)));
			}
			
			TypeDef typeDef = typeDefTable[index];
			typeDefIndex = index;  // store index for use in LoadMembers()
			
			declaredIn = assembly;
			
			FullyQualifiedName = GetNestedName(assembly, typeDefTable, index);
			
			// store in assembly's cache
			assembly.TypeDefObjects[index] = this;
			
			if (typeDef.IsFlagSet(TypeDef.FLAG_INTERFACE)) {
				ClassType = ClassType.Interface;
			} else if (typeDef.IsFlagSet(TypeDef.FLAG_CLASS)) {
				ClassType = ClassType.Class;
			}
			
			if (typeDef.Extends == 0) goto noext;
			
			SharpAssemblyClass extend = GetTypeRefOrDefClass(assembly, typeDef.Extends);
			
			if (extend == null) goto noext;
			
			if (extend.FullyQualifiedName == "System.Enum") {
				ClassType = ClassType.Enum;
			} else if (extend.FullyQualifiedName == "System.ValueType") {
				ClassType = ClassType.Struct;
			}
			
			baseTypeCollection.Add(extend);
			
			if (IsSubclassOf("System.Delegate")) ClassType = ClassType.Delegate;
			
		noext:
		
			InterfaceImpl[] ifaces = assembly.Tables.InterfaceImpl;
			if (ifaces == null) goto nointerfaces;
			
			for (int i = 1; i <= ifaces.GetUpperBound(0); ++i) {
				if (ifaces[i].Class == index) {
					SharpAssemblyClass impl = GetTypeRefOrDefClass(assembly, ifaces[i].Interface);
					if (impl != null) {
						baseTypeCollection.Add(impl);
					}
				}
			}
			
		nointerfaces:
		
			NestedClass[] nestedClasses = assembly.Tables.NestedClass;
			if (nestedClasses == null) goto nonested;

			for (int i = 1; i <= nestedClasses.GetUpperBound(0); ++i) {
				if (nestedClasses[i].EnclosingClass == index) {
					IClass newclass = FromTypeDef(assembly, nestedClasses[i].NestedClassIndex);
					InnerClasses.Add(newclass);
				}
			}
		
		nonested:
			
			// Attributes
			ArrayList attrib = assembly.Attributes.TypeDef[index] as ArrayList;
			if (attrib == null) goto modifiers;
						
			foreach(SharpCustomAttribute customattribute in attrib) {
				Attributes.Add(new SharpAssemblyAttribute(assembly, customattribute));
			}
			
		modifiers:
			
			Modifiers = ModifierEnum.None;
			
			if (typeDef.IsFlagSet(TypeDef.FLAG_SEALED)) {
				Modifiers |= ModifierEnum.Sealed;
			}
			
			if (typeDef.IsFlagSet(TypeDef.FLAG_ABSTRACT)) {
				Modifiers |= ModifierEnum.Abstract;
			}
						
			if (typeDef.IsMaskedFlagSet(TypeDef.FLAG_NESTEDPRIVATE, TypeDef.FLAG_VISIBILITYMASK)) {
				Modifiers |= ModifierEnum.Private;
			} else if (typeDef.IsMaskedFlagSet(TypeDef.FLAG_NESTEDPUBLIC, TypeDef.FLAG_VISIBILITYMASK) || typeDef.IsMaskedFlagSet(TypeDef.FLAG_PUBLIC, TypeDef.FLAG_VISIBILITYMASK)) {
				Modifiers |= ModifierEnum.Public;
			} else if (typeDef.IsMaskedFlagSet(TypeDef.FLAG_NESTEDASSEMBLY, TypeDef.FLAG_VISIBILITYMASK) ||
			    typeDef.IsMaskedFlagSet(TypeDef.FLAG_NOTPUBLIC, TypeDef.FLAG_VISIBILITYMASK)) {
				Modifiers |= ModifierEnum.Internal;
			} else if (typeDef.IsMaskedFlagSet(TypeDef.FLAG_NESTEDFAMILY, TypeDef.FLAG_VISIBILITYMASK)) {
				Modifiers |= ModifierEnum.Protected;
			} else if (typeDef.IsMaskedFlagSet(TypeDef.FLAG_NESTEDFAMORASSEM, TypeDef.FLAG_VISIBILITYMASK)) {
				Modifiers |= ModifierEnum.ProtectedOrInternal;
			} else if (typeDef.IsMaskedFlagSet(TypeDef.FLAG_NESTEDFAMANDASSEM, TypeDef.FLAG_VISIBILITYMASK)) {
				Modifiers |= ModifierEnum.Protected;
				Modifiers |= ModifierEnum.Internal;
			}
			
			if (typeDef.IsFlagSet(TypeDef.FLAG_SPECIALNAME)) {
				Modifiers |= ModifierEnum.Volatile | ModifierEnum.Unsafe | ModifierEnum.Extern;
			}
			
			/* members are loaded on demand now
			if (classType != ClassType.Delegate && loadMembers) {
				AddMethods(assembly, typeDefTable, index);
				AddFields(assembly, typeDefTable, index);
				AddProperties(assembly, typeDefTable, index);
				AddEvents(assembly, typeDefTable, index);
				
				membersLoaded = true;
			}
			*/
		}
		
		uint typeDefIndex = 0;
		
		bool membersLoaded = false;
		
		void LoadMembers()
		{
			if (membersLoaded) return;
			
			membersLoaded = true;
			
			SA.SharpAssembly assembly = (SA.SharpAssembly)declaredIn;
			TypeDef[] typeDefTable = assembly.Tables.TypeDef;
				
			AddMethods(assembly, typeDefTable, typeDefIndex);
			AddFields(assembly, typeDefTable, typeDefIndex);
			AddProperties(assembly, typeDefTable, typeDefIndex);
			AddEvents(assembly, typeDefTable, typeDefIndex);
		}
		
		public bool IsSubclassOf(string FullName)
		{
			foreach (SharpAssemblyClass basetype in baseTypeCollection) {
				if (basetype.FullyQualifiedName == FullName) return true;
				
				if (basetype.IsSubclassOf(FullName)) return true;
			}
			
			return false;
		}
		
		private static SharpAssemblyClass GetTypeRefOrDefClass(SA.SharpAssembly assembly, uint cind) {
			uint nTable = cind & 0x03;
			uint nIndex = cind >> 2;
			
			switch (nTable) {
				case 0:  // TypeDef
					return FromTypeDef(assembly, nIndex);
				case 1:  // TypeRef
					return FromTypeRef(assembly, nIndex);
				default:
					Console.WriteLine("GetTypeRefOrDefClass: Wrong TypeDefOrRef coded index!");
					return null;
			}
		}
		
		void AddEvents(SA.SharpAssembly asm, TypeDef[] typeDefTable, uint index)
		{
			EventMap[] eventMapTable = asm.Tables.EventMap;
			Event[]    eventTable    = asm.Tables.Event;
			if (eventMapTable == null || eventTable == null) {
				return;
			}
			
			for (int i = 1; i <= eventMapTable.GetUpperBound(0); ++i) {
				EventMap eventMap = eventMapTable[i];
				
				if (eventMap.Parent == index) {
					uint eventIndexStart = eventMap.EventList;
					
					// 0 means no events
					if (eventIndexStart == 0) {
						return;
					}
					
					uint eventIndexEnd   = (uint)eventTable.GetUpperBound(0) + 1;
					if (i < eventMapTable.GetUpperBound(0)) {
						eventIndexEnd = eventMapTable[i + 1].EventList;
					}
					
					for (uint j = eventIndexStart; j < eventIndexEnd; ++j) {
						IEvent newEvent = new SharpAssemblyEvent(asm, eventTable, this, j);
						Events.Add(newEvent);	
					}
					
					break;
				}
			}
		}
		
		void AddProperties(SA.SharpAssembly asm, TypeDef[] typeDefTable, uint index)
		{
			PropertyMap[] propertyMapTable = asm.Tables.PropertyMap;
			Property[]    propertyTable    = asm.Tables.Property;
			if (propertyMapTable == null || propertyTable == null) {
				return;
			}
			
			for (int i = 1; i <= propertyMapTable.GetUpperBound(0); ++i) {
				PropertyMap propertyMap = propertyMapTable[i];
				
				if (propertyMap.Parent == index) {
					uint propertyIndexStart = propertyMap.PropertyList;
					
					// 0 means no properties
					if (propertyIndexStart == 0) {
						return;
					}
					
					uint propertyIndexEnd   = (uint)propertyTable.GetUpperBound(0) + 1;
					if (i < propertyMapTable.GetUpperBound(0)) {
						propertyIndexEnd = propertyMapTable[i + 1].PropertyList;
					}
					
					for (uint j = propertyIndexStart; j < propertyIndexEnd; ++j) {
						IProperty newProperty = new SharpAssemblyProperty(asm, propertyTable, this, j);
						Properties.Add(newProperty);	
					}
					
					break;
				}
			}
		}
		
		void AddFields(SA.SharpAssembly asm, TypeDef[] typeDefTable, uint index)
		{
			Field[] fieldTable = asm.Tables.Field;
			if (fieldTable == null) {
				return;
			}
			
			uint fieldIndexStart = typeDefTable[index].FieldList;
			
			// 0 means no fields
			if (fieldIndexStart == 0) {
				return;
			}
			
			uint fieldIndexEnd   = (uint)fieldTable.GetUpperBound(0) + 1;
			if (index < typeDefTable.GetUpperBound(0)) {
				fieldIndexEnd = typeDefTable[index + 1].FieldList;
			}
			
			for (uint i = fieldIndexStart; i < fieldIndexEnd; ++i) {
				IField newField = new SharpAssemblyField(asm, fieldTable, this, i);
				Fields.Add(newField);		
			}
		}
		
		void AddMethods(SA.SharpAssembly asm, TypeDef[] typeDefTable, uint index)
		{
			Method[] methodDefTable = asm.Tables.Method;
			if (methodDefTable == null) {
				return;
			}
			
			uint methodIndexStart = typeDefTable[index].MethodList;
			
			// 0 means no methods
			if (methodIndexStart == 0) {
				return;
			}
			
			uint methodIndexEnd   = (uint)methodDefTable.GetUpperBound(0) + 1;
			if (index < typeDefTable.GetUpperBound(0)) {
				methodIndexEnd = typeDefTable[index + 1].MethodList;
			}
			
			for (uint i = methodIndexStart; i < methodIndexEnd; ++i) {
				IMethod newMethod = new SharpAssemblyMethod(asm, methodDefTable, this, i);
				Methods.Add(newMethod);
			}
		}
		
		public static SharpAssemblyClass[] GetAssemblyTypes(SA.SharpAssembly assembly)
		{
			
			TypeDef[] typeDefTable = assembly.Tables.TypeDef;
			if (typeDefTable == null) return new SharpAssemblyClass[0];
			
			ArrayList classes = new ArrayList();
			
			for (uint i = 1; i <= typeDefTable.GetUpperBound(0); ++i) {
				try {
					IClass newclass = new SharpAssemblyClass(assembly, typeDefTable, i);
					classes.Add(newclass);
				} catch {
					Console.WriteLine("GetAssemblyTypes: Error loading class " + i);
				}
			}
			
			return (SharpAssemblyClass[])classes.ToArray(typeof(SharpAssemblyClass));
		}
		
		public override string ToString()
		{
			return FullyQualifiedName;
		}
		
		public override List<IField> Fields {
			get {
				if (!membersLoaded) LoadMembers();
				return base.Fields;
			}
		}

		public override List<IProperty> Properties {
			get {
				if (!membersLoaded) LoadMembers();
				return base.Properties;
			}
		}

		public override List<IMethod> Methods {
			get {
				if (!membersLoaded) LoadMembers();
				return base.Methods;
			}
		}

		public override List<IEvent> Events {
			get {
				if (!membersLoaded) LoadMembers();
				return base.Events;
			}
		}
	}
}

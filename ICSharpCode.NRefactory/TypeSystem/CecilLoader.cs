// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Static methods for loading an IProjectContent from an already compiled assembly.
	/// </summary>
	public static class CecilLoader
	{
		#region Load From AssemblyDefinition
		public static IProjectContent LoadAssembly(AssemblyDefinition assemblyDefinition)
		{
			if (assemblyDefinition == null)
				throw new ArgumentNullException("assemblyDefinition");
			List<IAttribute> assemblyAttributes = new List<IAttribute>();
			ReadAttributes(assemblyDefinition, assemblyAttributes, earlyBindContext: null);
			TypeStorage typeStorage = new TypeStorage();
			CecilProjectContent pc = new CecilProjectContent(typeStorage, assemblyDefinition.Name.FullName, assemblyAttributes.AsReadOnly());
			List<CecilTypeDefinition> types = new List<CecilTypeDefinition>();
			foreach (ModuleDefinition module in assemblyDefinition.Modules) {
				foreach (TypeDefinition td in module.Types) {
					if ((td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public) {
						string name = td.FullName;
						if (name.Length == 0 || name[0] == '<')
							continue;
						types.Add(new CecilTypeDefinition(pc, td));
					}
				}
			}
			foreach (CecilTypeDefinition c in types) {
				c.Init(pc);
				typeStorage.UpdateType(c);
			}
			return pc;
		}
		
		/// <summary>
		/// Loads a type from Cecil.
		/// </summary>
		/// <param name="typeDefinition">The Cecil TypeDefinition.</param>
		/// <param name="projectContent">The project content used as parent for the new type.</param>
		/// <returns>ITypeDefinition representing the Cecil type.</returns>
		public static ITypeDefinition LoadType(TypeDefinition typeDefinition, IProjectContent projectContent)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			if (projectContent == null)
				throw new ArgumentNullException("projectContent");
			var c = new CecilTypeDefinition(projectContent, typeDefinition);
			c.Init(null);
			return c;
		}
		#endregion
		
		#region IProjectContent implementation
		sealed class CecilProjectContent : ProxyTypeResolveContext, IProjectContent, ISynchronizedTypeResolveContext
		{
			readonly string assemblyName;
			readonly ReadOnlyCollection<IAttribute> assemblyAttributes;
			
			public CecilProjectContent(TypeStorage types, string assemblyName, ReadOnlyCollection<IAttribute> assemblyAttributes)
				: base(types)
			{
				this.assemblyName = assemblyName;
				this.assemblyAttributes = assemblyAttributes;
			}
			
			public IList<IAttribute> AssemblyAttributes {
				get { return assemblyAttributes; }
			}
			
			public override string ToString()
			{
				return "[CecilProjectContent " + assemblyName + "]";
			}
			
			public override ISynchronizedTypeResolveContext Synchronize()
			{
				// CecilProjectContent is immutable, so we don't need to synchronize
				return this;
			}
			
			public void Dispose()
			{
			}
		}
		#endregion
		
		#region Load Assembly From Disk
		public static IProjectContent LoadAssemblyFile(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(fileName, new ReaderParameters { AssemblyResolver = new DummyAssemblyResolver() });
			return LoadAssembly(asm);
		}
		
		// used to prevent Cecil from loading referenced assemblies
		sealed class DummyAssemblyResolver : IAssemblyResolver
		{
			public AssemblyDefinition Resolve(AssemblyNameReference name)
			{
				return null;
			}
			
			public AssemblyDefinition Resolve(string fullName)
			{
				return null;
			}
		}
		#endregion
		
		#region Read Type Reference
		/// <summary>
		/// Reads a type reference.
		/// </summary>
		/// <param name="type">The Cecil type reference that should be converted into
		/// a type system type reference.</param>
		/// <param name="typeAttributes">Attributes associated with the Cecil type reference.
		/// This is used to support the 'dynamic' type.</param>
		/// <param name="entity">The entity that owns this type reference.
		/// Used for generic type references.</param>
		/// <param name="earlyBindContext">Early binding context - used to pre-resolve
		/// type references where possible.</param>
		public static ITypeReference ReadTypeReference(
			TypeReference type,
			ICustomAttributeProvider typeAttributes = null,
			IEntity entity = null,
			ITypeResolveContext earlyBindContext = null)
		{
			int typeIndex = 0;
			return CreateType(type, entity, earlyBindContext, typeAttributes, ref typeIndex);
		}
		
		static ITypeReference CreateType(
			TypeReference type,
			IEntity entity ,
			ITypeResolveContext earlyBindContext,
			ICustomAttributeProvider typeAttributes, ref int typeIndex)
		{
			while (type is OptionalModifierType || type is RequiredModifierType) {
				type = ((TypeSpecification)type).ElementType;
			}
			if (type == null) {
				return SharedTypes.UnknownType;
			}
			
			if (type is ByReferenceType) {
				throw new NotImplementedException();
			} else if (type is Mono.Cecil.PointerType) {
				typeIndex++;
				return PointerTypeReference.Create(
					CreateType(
						(type as Mono.Cecil.PointerType).ElementType,
						entity,
						earlyBindContext,
						typeAttributes, ref typeIndex));
			} else if (type is Mono.Cecil.ArrayType) {
				typeIndex++;
				return ArrayTypeReference.Create(
					CreateType(
						(type as Mono.Cecil.ArrayType).ElementType,
						entity,
						earlyBindContext,
						typeAttributes, ref typeIndex),
					(type as Mono.Cecil.ArrayType).Rank);
			} else if (type is GenericInstanceType) {
				GenericInstanceType gType = (GenericInstanceType)type;
				/*IReturnType baseType = CreateType(pc, member, gType.ElementType, attributeProvider, ref typeIndex);
				IReturnType[] para = new IReturnType[gType.GenericArguments.Count];
				for (int i = 0; i < para.Length; ++i) {
					typeIndex++;
					para[i] = CreateType(pc, member, gType.GenericArguments[i], attributeProvider, ref typeIndex);
				}
				return new ConstructedReturnType(baseType, para);*/
				throw new NotImplementedException();
			} else if (type is GenericParameter) {
				GenericParameter typeGP = type as GenericParameter;
				if (typeGP.Owner is MethodDefinition) {
					IMethod method = entity as IMethod;
					if (method != null) {
						if (typeGP.Position < method.TypeParameters.Count) {
							return method.TypeParameters[typeGP.Position];
						}
					}
					return SharedTypes.UnknownType;
				} else {
					ITypeDefinition c = (entity as ITypeDefinition) ?? (entity is IMember ? ((IMember)entity).DeclaringTypeDefinition : null);
					if (c != null && typeGP.Position < c.TypeParameters.Count) {
						if (c.TypeParameters[typeGP.Position].Name == type.Name) {
							return c.TypeParameters[typeGP.Position];
						}
					}
					return SharedTypes.UnknownType;
				}
			} else {
				string name = type.FullName;
				if (name == null)
					throw new ApplicationException("type.FullName returned null. Type: " + type.ToString());
				
				if (name.IndexOf('/') > 0) {
					string[] nameparts = name.Split('/');
					ITypeReference typeRef = GetSimpleType(nameparts[0], earlyBindContext);
					for (int i = 1; i < nameparts.Length; i++) {
						int partTypeParameterCount;
						string namepart = SplitTypeParameterCountFromReflectionName(nameparts[i], out partTypeParameterCount);
						typeRef = new NestedTypeReference(typeRef, namepart, partTypeParameterCount);
					}
					return typeRef;
				} else if (name == "System.Object" && HasDynamicAttribute(typeAttributes, typeIndex)) {
					return SharedTypes.Dynamic;
				} else {
					return GetSimpleType(name, earlyBindContext);
				}
			}
		}
		
		static ITypeReference GetSimpleType(string reflectionName, ITypeResolveContext earlyBindContext)
		{
			int typeParameterCount;
			string name = SplitTypeParameterCountFromReflectionName(reflectionName, out typeParameterCount);
			if (earlyBindContext != null) {
				IType c = earlyBindContext.GetClass(name, typeParameterCount, StringComparer.Ordinal);
				if (c != null)
					return c;
			}
			return new GetClassTypeReference(name, typeParameterCount);
		}
		
		static string SplitTypeParameterCountFromReflectionName(string reflectionName)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0) {
				return reflectionName;
			} else {
				return reflectionName.Substring(0, pos);
			}
		}
		
		static string SplitTypeParameterCountFromReflectionName(string reflectionName, out int typeParameterCount)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0) {
				typeParameterCount = 0;
				return reflectionName;
			} else {
				string typeCount = reflectionName.Substring(pos + 1);
				if (int.TryParse(typeCount, out typeParameterCount))
					return reflectionName.Substring(0, pos);
				else
					return reflectionName;
			}
		}
		
		static bool HasDynamicAttribute(ICustomAttributeProvider attributeProvider, int typeIndex)
		{
			if (attributeProvider == null || !attributeProvider.HasCustomAttributes)
				return false;
			foreach (CustomAttribute a in attributeProvider.CustomAttributes) {
				if (a.Constructor.DeclaringType.FullName == "System.Runtime.CompilerServices.DynamicAttribute") {
					if (a.ConstructorArguments.Count == 1) {
						CustomAttributeArgument[] values = a.ConstructorArguments[0].Value as CustomAttributeArgument[];
						if (values != null && typeIndex < values.Length && values[typeIndex].Value is bool)
							return (bool)values[typeIndex].Value;
					}
					return true;
				}
			}
			return false;
		}
		#endregion
		
		#region Read Attributes
		static void ReadAttributes(ICustomAttributeProvider attributeProvider, IList<IAttribute> outputList, ITypeResolveContext earlyBindContext)
		{
			foreach (var cecilAttribute in attributeProvider.CustomAttributes) {
				outputList.Add(new CecilAttribute(cecilAttribute, earlyBindContext));
			}
		}
		
		sealed class CecilAttribute : Immutable, IAttribute
		{
			readonly ITypeReference attributeType;
			readonly IList<IConstantValue> positionalArguments;
			readonly IList<KeyValuePair<string, IConstantValue>> namedArguments;
			
			public CecilAttribute(CustomAttribute ca, ITypeResolveContext earlyBindContext)
			{
				this.attributeType = ReadTypeReference(ca.AttributeType, earlyBindContext: earlyBindContext);
				try {
					if (ca.HasConstructorArguments) {
						var posArgs = new List<IConstantValue>();
						foreach (var arg in ca.ConstructorArguments) {
							posArgs.Add(ReadConstantValue(arg, earlyBindContext));
						}
						this.positionalArguments = posArgs.AsReadOnly();
					} else {
						this.positionalArguments = EmptyList<IConstantValue>.Instance;
					}
				} catch (InvalidOperationException) {
					this.positionalArguments = EmptyList<IConstantValue>.Instance;
				}
				try {
					if (ca.HasFields || ca.HasProperties) {
						var namedArgs = new List<KeyValuePair<string, IConstantValue>>();
						foreach (var arg in ca.Fields) {
							namedArgs.Add(new KeyValuePair<string, IConstantValue>(arg.Name, ReadConstantValue(arg.Argument, earlyBindContext)));
						}
						foreach (var arg in ca.Properties) {
							namedArgs.Add(new KeyValuePair<string, IConstantValue>(arg.Name, ReadConstantValue(arg.Argument, earlyBindContext)));
						}
						this.namedArguments = namedArgs.AsReadOnly();
					} else {
						this.namedArguments = EmptyList<KeyValuePair<string, IConstantValue>>.Instance;
					}
				} catch (InvalidOperationException) {
					this.namedArguments = EmptyList<KeyValuePair<string, IConstantValue>>.Instance;
				}
			}
			
			public DomRegion Region {
				get { return DomRegion.Empty; }
			}
			
			public ITypeReference AttributeType {
				get { return attributeType; }
			}
			
			public IList<IConstantValue> PositionalArguments {
				get { return positionalArguments; }
			}
			
			public IList<KeyValuePair<string, IConstantValue>> NamedArguments {
				get { return namedArguments; }
			}
		}
		#endregion
		
		#region Read Constant Value
		static IConstantValue ReadConstantValue(CustomAttributeArgument arg, ITypeResolveContext earlyBindContext)
		{
			ITypeReference type = ReadTypeReference(arg.Type, earlyBindContext: earlyBindContext);
			object value = arg.Value;
			TypeReference valueType = value as TypeReference;
			if (valueType != null)
				value = ReadTypeReference(valueType, earlyBindContext: earlyBindContext);
			return new SimpleConstantValue(type, value);
		}
		#endregion
		
		#region Read Type Definition
		class CecilTypeDefinition : DefaultTypeDefinition
		{
			TypeDefinition typeDefinition;
			
			public CecilTypeDefinition(IProjectContent pc, TypeDefinition typeDefinition)
				: base(pc, typeDefinition.Namespace, SplitTypeParameterCountFromReflectionName(typeDefinition.Name))
			{
				this.typeDefinition = typeDefinition;
			}
			
			public CecilTypeDefinition(CecilTypeDefinition parentType, string name, TypeDefinition typeDefinition)
				: base(parentType, name)
			{
				this.typeDefinition = typeDefinition;
			}
			
			public void Init(ITypeResolveContext earlyBindContext)
			{
				InitNestedTypes(earlyBindContext);
				InitModifiers();
				
				if (typeDefinition.HasGenericParameters) {
					throw new NotImplementedException();
					/*foreach (GenericParameter g in td.GenericParameters) {
						this.TypeParameters.Add(new DefaultTypeParameter(this, g.Name, g.Position));
					}
					int i = 0;
					foreach (GenericParameter g in td.GenericParameters) {
						AddConstraintsFromType(this.TypeParameters[i++], g);
					}*/
				}
				
				if (typeDefinition.HasCustomAttributes) {
					ReadAttributes(typeDefinition, this.Attributes, earlyBindContext);
				}
				
				// set base classes
				if (typeDefinition.BaseType != null) {
					BaseTypes.Add(ReadTypeReference(typeDefinition.BaseType, entity: this, earlyBindContext: earlyBindContext));
				}
				if (typeDefinition.HasInterfaces) {
					foreach (TypeReference iface in typeDefinition.Interfaces) {
						BaseTypes.Add(ReadTypeReference(iface, entity: this, earlyBindContext: earlyBindContext));
					}
				}
				
				InitMembers(earlyBindContext);
				
				this.typeDefinition = null;
				Freeze(); // freeze after initialization
			}
			
			void InitNestedTypes(ITypeResolveContext earlyBindContext)
			{
				if (!typeDefinition.HasNestedTypes)
					return;
				foreach (TypeDefinition nestedType in typeDefinition.NestedTypes) {
					TypeAttributes visibility = nestedType.Attributes & TypeAttributes.VisibilityMask;
					if (visibility == TypeAttributes.NestedPublic
					    || visibility == TypeAttributes.NestedFamily
					    || visibility == TypeAttributes.NestedFamORAssem)
					{
						string name = nestedType.Name;
						int pos = name.LastIndexOf('/');
						if (pos > 0)
							name = name.Substring(pos + 1);
						if (name.Length == 0 || name[0] == '<')
							continue;
						name = SplitTypeParameterCountFromReflectionName(name);
						InnerClasses.Add(new CecilTypeDefinition(this, name, nestedType));
					}
				}
				foreach (CecilTypeDefinition innerClass in this.InnerClasses) {
					innerClass.Init(earlyBindContext);
				}
			}
			
			void InitModifiers()
			{
				TypeDefinition td = this.typeDefinition;
				// set classtype
				if (td.IsInterface) {
					this.ClassType = ClassType.Interface;
				} else if (td.IsEnum) {
					this.ClassType = ClassType.Enum;
				} else if (td.IsValueType) {
					this.ClassType = ClassType.Struct;
				} else if (IsDelegate(td)) {
					this.ClassType = ClassType.Delegate;
				} else if (IsModule(td)) {
					this.ClassType = ClassType.Module;
				} else {
					this.ClassType = ClassType.Class;
				}
				this.IsSealed = td.IsSealed;
				this.IsAbstract = td.IsAbstract;
				if ((td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic) {
					this.Accessibility = Accessibility.Public;
				} else if ((td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily) {
					this.Accessibility = Accessibility.Protected;
				} else if ((td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamORAssem) {
					// we don't care about the 'OrAssem' part because it's an external assembly
					this.Accessibility = Accessibility.Protected;
				} else {
					this.Accessibility = Accessibility.Public;
				}
			}
			
			static bool IsDelegate(TypeDefinition type)
			{
				if (type.BaseType == null)
					return false;
				else
					return type.BaseType.FullName == "System.Delegate"
						|| type.BaseType.FullName == "System.MulticastDelegate";
			}
			
			static bool IsModule(TypeDefinition type)
			{
				if (!type.HasCustomAttributes)
					return false;
				foreach (var att in type.CustomAttributes) {
					if (att.AttributeType.FullName == "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute"
					    || att.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGlobalScopeAttribute")
					{
						return true;
					}
				}
				return false;
			}
			
			void InitMembers(ITypeResolveContext earlyBindContext)
			{
				//throw new NotImplementedException();
			}
		}
		#endregion
	}
}

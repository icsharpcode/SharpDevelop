// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Allows loading an IProjectContent from an already compiled assembly.
	/// </summary>
	/// <remarks>Instance methods are not thread-safe; you need to create multiple instances of CecilLoader
	/// if you want to load multiple project contents in parallel.</remarks>
	public class CecilLoader
	{
		#region Options
		/// <summary>
		/// Gets/Sets the early bind context.
		/// This context is used to pre-resolve type references - setting this property will cause the CecilLoader
		/// to directly reference the resolved types, and create links (<see cref="GetClassTypeReference"/>) to types
		/// that could not be resolved.
		/// </summary>
		public ITypeResolveContext EarlyBindContext { get; set; }
		
		/// <summary>
		/// Specifies whether to include internal members. The default is false.
		/// </summary>
		public bool IncludeInternalMembers { get; set; }
		#endregion
		
		#region Load From AssemblyDefinition
		public IProjectContent LoadAssembly(AssemblyDefinition assemblyDefinition)
		{
			if (assemblyDefinition == null)
				throw new ArgumentNullException("assemblyDefinition");
			ITypeResolveContext oldEarlyBindContext = this.EarlyBindContext;
			try {
				List<IAttribute> assemblyAttributes = new List<IAttribute>();
				AddAttributes(assemblyDefinition, assemblyAttributes);
				TypeStorage typeStorage = new TypeStorage();
				CecilProjectContent pc = new CecilProjectContent(typeStorage, assemblyDefinition.Name.FullName, assemblyAttributes.AsReadOnly());
				
				this.EarlyBindContext = MultiTypeResolveContext.Combine(pc, this.EarlyBindContext);
				List<CecilTypeDefinition> types = new List<CecilTypeDefinition>();
				foreach (ModuleDefinition module in assemblyDefinition.Modules) {
					foreach (TypeDefinition td in module.Types) {
						if (this.IncludeInternalMembers || (td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public) {
							string name = td.FullName;
							if (name.Length == 0 || name[0] == '<')
								continue;
							types.Add(new CecilTypeDefinition(pc, td));
						}
					}
				}
				foreach (CecilTypeDefinition c in types) {
					c.Init(this);
					typeStorage.UpdateType(c);
				}
				return pc;
			} finally {
				this.EarlyBindContext = oldEarlyBindContext;
			}
		}
		
		/// <summary>
		/// Loads a type from Cecil.
		/// </summary>
		/// <param name="typeDefinition">The Cecil TypeDefinition.</param>
		/// <param name="projectContent">The project content used as parent for the new type.</param>
		/// <returns>ITypeDefinition representing the Cecil type.</returns>
		public ITypeDefinition LoadType(TypeDefinition typeDefinition, IProjectContent projectContent)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			if (projectContent == null)
				throw new ArgumentNullException("projectContent");
			var c = new CecilTypeDefinition(projectContent, typeDefinition);
			c.Init(this);
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
		public IProjectContent LoadAssemblyFile(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			var param = new ReaderParameters { AssemblyResolver = new DummyAssemblyResolver() };
			AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(fileName, param);
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
		public ITypeReference ReadTypeReference(
			TypeReference type,
			ICustomAttributeProvider typeAttributes = null,
			IEntity entity = null)
		{
			int typeIndex = 0;
			return CreateType(type, entity, typeAttributes, ref typeIndex);
		}
		
		ITypeReference CreateType(
			TypeReference type,
			IEntity entity ,
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
						entity, typeAttributes, ref typeIndex));
			} else if (type is Mono.Cecil.ArrayType) {
				typeIndex++;
				return ArrayTypeReference.Create(
					CreateType(
						(type as Mono.Cecil.ArrayType).ElementType,
						entity, typeAttributes, ref typeIndex),
					(type as Mono.Cecil.ArrayType).Rank);
			} else if (type is GenericInstanceType) {
				GenericInstanceType gType = (GenericInstanceType)type;
				ITypeReference baseType = CreateType(gType.ElementType, entity, typeAttributes, ref typeIndex);
				ITypeReference[] para = new ITypeReference[gType.GenericArguments.Count];
				for (int i = 0; i < para.Length; ++i) {
					typeIndex++;
					para[i] = CreateType(gType.GenericArguments[i], entity, typeAttributes, ref typeIndex);
				}
				return ConstructedType.Create(baseType, para);
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
					ITypeReference typeRef = GetSimpleType(nameparts[0]);
					for (int i = 1; i < nameparts.Length; i++) {
						int partTypeParameterCount;
						string namepart = SplitTypeParameterCountFromReflectionName(nameparts[i], out partTypeParameterCount);
						typeRef = new NestedTypeReference(typeRef, namepart, partTypeParameterCount);
					}
					return typeRef;
				} else if (name == "System.Object" && HasDynamicAttribute(typeAttributes, typeIndex)) {
					return SharedTypes.Dynamic;
				} else {
					return GetSimpleType(name);
				}
			}
		}
		
		/// <summary>
		/// Gets a type reference for a reflection name.
		/// This method does not handle nested types -- it can be only used with top-level types.
		/// </summary>
		ITypeReference GetSimpleType(string reflectionName)
		{
			int typeParameterCount;
			string name = SplitTypeParameterCountFromReflectionName(reflectionName, out typeParameterCount);
			var earlyBindContext = this.EarlyBindContext;
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
		void AddAttributes(ICustomAttributeProvider attributeProvider, IList<IAttribute> outputList)
		{
			foreach (var cecilAttribute in attributeProvider.CustomAttributes) {
				outputList.Add(ReadAttribute(cecilAttribute));
			}
		}
		
		public IAttribute ReadAttribute(CustomAttribute cecilAttribute)
		{
			if (cecilAttribute == null)
				throw new ArgumentNullException("cecilAttribute");
			return new CecilAttribute(cecilAttribute, this);
		}
		
		sealed class CecilAttribute : Immutable, IAttribute
		{
			readonly ITypeReference attributeType;
			readonly IList<IConstantValue> positionalArguments;
			readonly IList<KeyValuePair<string, IConstantValue>> namedArguments;
			
			public CecilAttribute(CustomAttribute ca, CecilLoader loader)
			{
				this.attributeType = loader.ReadTypeReference(ca.AttributeType);
				try {
					if (ca.HasConstructorArguments) {
						var posArgs = new List<IConstantValue>();
						foreach (var arg in ca.ConstructorArguments) {
							posArgs.Add(loader.ReadConstantValue(arg));
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
							namedArgs.Add(new KeyValuePair<string, IConstantValue>(arg.Name, loader.ReadConstantValue(arg.Argument)));
						}
						foreach (var arg in ca.Properties) {
							namedArgs.Add(new KeyValuePair<string, IConstantValue>(arg.Name, loader.ReadConstantValue(arg.Argument)));
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
		public IConstantValue ReadConstantValue(CustomAttributeArgument arg)
		{
			ITypeReference type = ReadTypeReference(arg.Type);
			object value = arg.Value;
			TypeReference valueType = value as TypeReference;
			if (valueType != null)
				value = ReadTypeReference(valueType);
			return new SimpleConstantValue(type, value);
		}
		#endregion
		
		#region Read Type Definition
		sealed class CecilTypeDefinition : DefaultTypeDefinition
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
			
			public void Init(CecilLoader loader)
			{
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
				
				InitNestedTypes(loader); // nested types can be initialized only after generic parameters were created
				
				if (typeDefinition.HasCustomAttributes) {
					loader.AddAttributes(typeDefinition, this.Attributes);
				}
				
				// set base classes
				if (typeDefinition.BaseType != null) {
					BaseTypes.Add(loader.ReadTypeReference(typeDefinition.BaseType, entity: this));
				}
				if (typeDefinition.HasInterfaces) {
					foreach (TypeReference iface in typeDefinition.Interfaces) {
						BaseTypes.Add(loader.ReadTypeReference(iface, entity: this));
					}
				}
				
				InitMembers(loader);
				
				this.typeDefinition = null;
				Freeze(); // freeze after initialization
			}
			
			void InitNestedTypes(CecilLoader loader)
			{
				if (!typeDefinition.HasNestedTypes)
					return;
				foreach (TypeDefinition nestedType in typeDefinition.NestedTypes) {
					TypeAttributes visibility = nestedType.Attributes & TypeAttributes.VisibilityMask;
					if (loader.IncludeInternalMembers
					    || visibility == TypeAttributes.NestedPublic
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
					innerClass.Init(loader);
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
				switch (td.Attributes & TypeAttributes.VisibilityMask) {
					case TypeAttributes.NotPublic:
					case TypeAttributes.NestedAssembly:
						this.Accessibility = Accessibility.Internal;
						break;
					case TypeAttributes.Public:
					case TypeAttributes.NestedPublic:
						this.Accessibility = Accessibility.Public;
						break;
					case TypeAttributes.NestedPrivate:
						this.Accessibility = Accessibility.Private;
						break;
					case TypeAttributes.NestedFamily:
						this.Accessibility = Accessibility.Protected;
						break;
					case TypeAttributes.NestedFamANDAssem:
						this.Accessibility = Accessibility.ProtectedAndInternal;
						break;
					case TypeAttributes.NestedFamORAssem:
						this.Accessibility = Accessibility.ProtectedOrInternal;
						break;
					case TypeAttributes.LayoutMask:
						this.Accessibility = Accessibility;
						break;
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
			
			void InitMembers(CecilLoader loader)
			{
				this.AddDefaultConstructorIfRequired = (this.ClassType == ClassType.Struct || this.ClassType == ClassType.Enum);
				if (typeDefinition.HasMethods) {
					foreach (MethodDefinition method in typeDefinition.Methods) {
						if (loader.IsVisible(method.Attributes)) {
							EntityType type = EntityType.Method;
							if (method.IsSpecialName) {
								if (method.IsConstructor)
									type = EntityType.Constructor;
								else if (method.Name.StartsWith("op_", StringComparison.Ordinal))
									type = EntityType.Operator;
								else
									continue;
							}
							this.Methods.Add(loader.ReadMethod(method, this, type));
						}
					}
				}
			}
		}
		#endregion
		
		#region ReadMethod
		IMethod ReadMethod(MethodDefinition method, ITypeDefinition parentType, EntityType methodType)
		{
			DefaultMethod m = new DefaultMethod(parentType, method.Name);
			m.EntityType = methodType;
			if (method.HasGenericParameters) {
				throw new NotImplementedException();
				/*foreach (GenericParameter g in method.GenericParameters) {
					m.TypeParameters.Add(new DefaultTypeParameter(this, g.Name, g.Position));
				}
				int i = 0;
				foreach (GenericParameter g in method.GenericParameters) {
					AddConstraintsFromType(m.TypeParameters[i++], g);
				}*/
			}
			
			if (method.IsConstructor)
				m.ReturnType = parentType;
			else
				m.ReturnType = ReadTypeReference(method.ReturnType, typeAttributes: method, entity: m);
			
			if (method.HasCustomAttributes) {
				AddAttributes(method, m.Attributes);
			}
			
			if (parentType.ClassType == ClassType.Interface) {
				// interface members don't have modifiers, but we want to handle them as "public abstract"
				m.Accessibility = Accessibility.Public;
				m.IsAbstract = true;
			} else {
				TranslateModifiers(method, m);
			}
			
			if (method.HasParameters) {
				foreach (ParameterDefinition p in method.Parameters) {
					m.Parameters.Add(ReadParameter(p, parentMember: m));
				}
			}
			
			AddExplicitInterfaceImplementations(method, m);
			
			// mark as extension method is the attribute is set
			if (method.IsStatic && method.HasCustomAttributes) {
				foreach (var attr in method.CustomAttributes) {
					if (attr.AttributeType.FullName == typeof(ExtensionAttribute).FullName)
						m.IsExtensionMethod = true;
				}
			}
			
			return m;
		}
		
		bool IsVisible(MethodAttributes att)
		{
			att &= MethodAttributes.MemberAccessMask;
			return IncludeInternalMembers
				|| att == MethodAttributes.Public
				|| att == MethodAttributes.Family
				|| att == MethodAttributes.FamORAssem;
		}
		
		Accessibility GetAccessibility(MethodAttributes attr)
		{
			switch (attr & MethodAttributes.MemberAccessMask) {
				case MethodAttributes.Private:
					return Accessibility.Private;
				case MethodAttributes.FamANDAssem:
					return Accessibility.ProtectedAndInternal;
				case MethodAttributes.Assem:
					return Accessibility.Internal;
				case MethodAttributes.Family:
					return Accessibility.Protected;
				case MethodAttributes.FamORAssem:
					return Accessibility.ProtectedOrInternal;
				default:
					return Accessibility.Public;
			}
		}
		
		void TranslateModifiers(MethodDefinition method, AbstractMember member)
		{
			member.Accessibility = GetAccessibility(method.Attributes);
			if (method.IsAbstract)
				member.IsAbstract = true;
			else if (method.IsFinal)
				member.IsSealed = true;
			else if (method.IsVirtual)
				member.IsVirtual = true;
		}
		#endregion
		
		bool IsVisible(FieldAttributes att)
		{
			att &= FieldAttributes.FieldAccessMask;
			return IncludeInternalMembers
				|| att == FieldAttributes.Public
				|| att == FieldAttributes.Family
				|| att == FieldAttributes.FamORAssem;
		}
		
		#region ReadParameter
		public IParameter ReadParameter(ParameterDefinition parameter, IParameterizedMember parentMember = null)
		{
			if (parameter == null)
				throw new ArgumentNullException("parameter");
			DefaultParameter p = new DefaultParameter();
			p.Name = parameter.Name;
			p.Type = ReadTypeReference(parameter.ParameterType, typeAttributes: parameter, entity: parentMember);
			
			if (parameter.HasCustomAttributes)
				AddAttributes(parameter, p.Attributes);
			
			if (parameter.ParameterType is ByReferenceType) {
				if (parameter.IsOut)
					p.IsOut = true;
				else
					p.IsRef = true;
			}
			
			if (parameter.IsOptional) {
				p.DefaultValue = ReadConstantValue(new CustomAttributeArgument(parameter.ParameterType, parameter.Constant));
			}
			
			if (parameter.ParameterType is Mono.Cecil.ArrayType) {
				foreach (CustomAttribute att in parameter.CustomAttributes) {
					if (att.AttributeType.FullName == typeof(ParamArrayAttribute).FullName) {
						p.IsParams = true;
						break;
					}
				}
			}
			
			return p;
		}
		#endregion
		
		void AddExplicitInterfaceImplementations(MethodDefinition method, AbstractMember targetMember)
		{
			if (method.HasOverrides) {
				throw new NotImplementedException();
			}
		}
	}
}

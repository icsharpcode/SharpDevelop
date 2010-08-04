// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Project content that represents an already compiled assembly.
	/// </summary>
	public class CecilProjectContent : IProjectContent
	{
		IList<IAttribute> assemblyAttributes;
		
		#region Constructor
		public CecilProjectContent(AssemblyDefinition assemblyDefinition)
		{
			this.assemblyAttributes = ReadAttributes(assemblyDefinition, this);
		}
		#endregion
		
		#region IProjectContent implementation
		public IList<IAttribute> AssemblyAttributes {
			get { return assemblyAttributes; }
		}
		
		public ITypeDefinition GetClass(string fullTypeName, int typeParameterCount, StringComparer nameComparer)
		{
			throw new NotImplementedException();
		}
		
		public ISynchronizedTypeResolveContext Synchronize()
		{
			// CecilProjectContent is immutable, so we don't need to synchronize
			return new DummySynchronizedTypeResolveContext(this);
		}
		
		sealed class DummySynchronizedTypeResolveContext : ProxyTypeResolveContext, ISynchronizedTypeResolveContext
		{
			public DummySynchronizedTypeResolveContext(ITypeResolveContext context) : base(context)
			{
			}
			
			public void Dispose()
			{
			}
		}
		#endregion
		
		#region Load Assembly From Disk
		public static CecilProjectContent LoadAssembly(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(fileName, new ReaderParameters { AssemblyResolver = new DummyAssemblyResolver() });
			return new CecilProjectContent(asm);
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
		/// <param name="earlyBindContext">Early binding context - used to pre-resolve
		/// type references where possible.</param>
		/// <param name="entity">The entity that owns this type reference.
		/// Used for generic type references.</param>
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
				throw new NotImplementedException();
				/*GenericParameter typeGP = type as GenericParameter;
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
				}*/
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
		public static IList<IAttribute> ReadAttributes(ICustomAttributeProvider attributeProvider, ITypeResolveContext earlyBindContext)
		{
			Contract.Ensures(Contract.Result<IList<IAttribute>>() != null);
			if (attributeProvider == null || !attributeProvider.HasCustomAttributes)
				return EmptyList<IAttribute>.Instance;
			var cecilAttributes = attributeProvider.CustomAttributes;
			IAttribute[] attributes = new IAttribute[cecilAttributes.Count];
			for (int i = 0; i < attributes.Length; i++) {
				attributes[i] = new CecilAttribute(cecilAttributes[i], earlyBindContext);
			}
			return Array.AsReadOnly(attributes);
		}
		
		sealed class CecilAttribute : Immutable, IAttribute
		{
			ITypeReference attributeType;
			volatile CustomAttribute ca;
			ITypeResolveContext earlyBindContext;
			IList<IConstantValue> positionalArguments;
			IList<KeyValuePair<string, IConstantValue>> namedArguments;
			
			public CecilAttribute(CustomAttribute ca, ITypeResolveContext earlyBindContext)
			{
				this.attributeType = ReadTypeReference(ca.AttributeType, earlyBindContext: earlyBindContext);
				this.ca = ca;
				this.earlyBindContext = earlyBindContext;
			}
			
			public DomRegion Region {
				get { return DomRegion.Empty; }
			}
			
			public ITypeReference AttributeType {
				get { return attributeType; }
			}
			
			public IList<IConstantValue> PositionalArguments {
				get {
					EnsureArguments();
					return positionalArguments;
				}
			}
			
			public IList<KeyValuePair<string, IConstantValue>> NamedArguments {
				get {
					EnsureArguments();
					return namedArguments;
				}
			}
			
			void EnsureArguments()
			{
				CustomAttribute ca = this.ca;
				if (ca != null) {
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
					this.ca = null;
				}
			}
		}
		#endregion
		
		#region Read Constant Value
		public static IConstantValue ReadConstantValue(CustomAttributeArgument arg, ITypeResolveContext earlyBindContext)
		{
			return new CecilConstantValue(arg.Type, arg.Value, earlyBindContext);
		}
		
		sealed class CecilConstantValue : Immutable, IConstantValue
		{
			ITypeReference type;
			object value;
			
			public CecilConstantValue(TypeReference type, object value, ITypeResolveContext earlyBindContext)
			{
				this.type = ReadTypeReference(type, earlyBindContext: earlyBindContext);
				TypeReference valueType = value as TypeReference;
				if (valueType != null)
					this.value = ReadTypeReference(valueType, earlyBindContext: earlyBindContext);
				else
					this.value = value;
			}
			
			public IType GetValueType(ITypeResolveContext context)
			{
				return type.Resolve(context);
			}
			
			public object GetValue(ITypeResolveContext context)
			{
				return value;
			}
		}
		#endregion
	}
}

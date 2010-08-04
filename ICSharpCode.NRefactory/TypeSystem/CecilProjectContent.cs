// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

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
			return ReadTypeReference(type, typeAttributes, entity, earlyBindContext, ref typeIndex);
		}
		
		static ITypeReference ReadTypeReference(
			TypeReference type,
			ICustomAttributeProvider typeAttributes,
			IEntity entity ,
			ITypeResolveContext earlyBindContext,
			ref int typeIndex)
		{
			while (type is OptionalModifierType || type is RequiredModifierType) {
				type = ((TypeSpecification)type).ElementType;
			}
			if (type == null) {
				return SharedTypes.UnknownType;
			}
			throw new NotImplementedException();
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

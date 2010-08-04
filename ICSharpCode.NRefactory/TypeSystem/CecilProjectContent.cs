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
		public static ITypeReference ReadTypeReference(TypeReference attributeType, ITypeResolveContext earlyBindContext)
		{
			throw new NotImplementedException();
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
				this.attributeType = ReadTypeReference(ca.AttributeType, earlyBindContext);
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
			throw new NotImplementedException();
		}
		#endregion
	}
}

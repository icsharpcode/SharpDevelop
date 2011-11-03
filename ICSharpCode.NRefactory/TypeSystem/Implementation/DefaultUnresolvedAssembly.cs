// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation for <see cref="IUnresolvedAssembly"/>.
	/// </summary>
	[Serializable]
	public class DefaultUnresolvedAssembly : AbstractFreezable, IUnresolvedAssembly
	{
		#region FullNameAndTypeParameterCount
		struct FullNameAndTypeParameterCount
		{
			public readonly string Namespace;
			public readonly string Name;
			public readonly int TypeParameterCount;
			
			public FullNameAndTypeParameterCount(string nameSpace, string name, int typeParameterCount)
			{
				this.Namespace = nameSpace;
				this.Name = name;
				this.TypeParameterCount = typeParameterCount;
			}
		}
		
		sealed class FullNameAndTypeParameterCountComparer : IEqualityComparer<FullNameAndTypeParameterCount>
		{
			public static readonly FullNameAndTypeParameterCountComparer Ordinal = new FullNameAndTypeParameterCountComparer(StringComparer.Ordinal);
			
			public readonly StringComparer NameComparer;
			
			public FullNameAndTypeParameterCountComparer(StringComparer nameComparer)
			{
				this.NameComparer = nameComparer;
			}
			
			public bool Equals(FullNameAndTypeParameterCount x, FullNameAndTypeParameterCount y)
			{
				return x.TypeParameterCount == y.TypeParameterCount
					&& NameComparer.Equals(x.Name, y.Name)
					&& NameComparer.Equals(x.Namespace, y.Namespace);
			}
			
			public int GetHashCode(FullNameAndTypeParameterCount obj)
			{
				return NameComparer.GetHashCode(obj.Name) ^ NameComparer.GetHashCode(obj.Namespace) ^ obj.TypeParameterCount;
			}
		}
		#endregion
		
		string assemblyName;
		IList<IUnresolvedAttribute> assemblyAttributes = new List<IUnresolvedAttribute>();
		IList<IUnresolvedAttribute> moduleAttributes = new List<IUnresolvedAttribute>();
		Dictionary<FullNameAndTypeParameterCount, IUnresolvedTypeDefinition> typeDefinitions = new Dictionary<FullNameAndTypeParameterCount, IUnresolvedTypeDefinition>(FullNameAndTypeParameterCountComparer.Ordinal);
		
		protected override void FreezeInternal()
		{
			base.FreezeInternal();
			assemblyAttributes = FreezableHelper.FreezeListAndElements(assemblyAttributes);
			moduleAttributes = FreezableHelper.FreezeListAndElements(moduleAttributes);
			foreach (var type in typeDefinitions.Values) {
				FreezableHelper.Freeze(type);
			}
		}
		
		public DefaultUnresolvedAssembly(string assemblyName)
		{
			if (assemblyName == null)
				throw new ArgumentNullException("assemblyName");
			this.assemblyName = assemblyName;
		}
		
		public string AssemblyName {
			get { return assemblyName; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				FreezableHelper.ThrowIfFrozen(this);
				assemblyName = value;
			}
		}
		
		public IList<IUnresolvedAttribute> AssemblyAttributes {
			get { return assemblyAttributes; }
		}
		
		IEnumerable<IUnresolvedAttribute> IUnresolvedAssembly.AssemblyAttributes {
			get { return assemblyAttributes; }
		}
		
		public IList<IUnresolvedAttribute> ModuleAttributes {
			get { return moduleAttributes; }
		}
		
		IEnumerable<IUnresolvedAttribute> IUnresolvedAssembly.ModuleAttributes {
			get { return moduleAttributes; }
		}
		
		public IEnumerable<IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
			get { return typeDefinitions.Values; }
		}
		
		/// <summary>
		/// Adds a new top-level type definition to this assembly.
		/// </summary>
		/// <remarks>DefaultUnresolvedAssembly does not support partial classes.
		/// Adding more than one part of a type will cause an ArgumentException.</remarks>
		public void AddTypeDefinition(IUnresolvedTypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			if (typeDefinition.DeclaringTypeDefinition != null)
				throw new ArgumentException("Cannot add nested types.");
			FreezableHelper.ThrowIfFrozen(this);
			var key = new FullNameAndTypeParameterCount(typeDefinition.Namespace, typeDefinition.Name, typeDefinition.TypeParameters.Count);
			typeDefinitions.Add(key, typeDefinition);
		}
		
		public IUnresolvedTypeDefinition GetTypeDefinition(string ns, string name, int typeParameterCount)
		{
			var key = new FullNameAndTypeParameterCount(ns, name, typeParameterCount);
			IUnresolvedTypeDefinition td;
			if (typeDefinitions.TryGetValue(key, out td))
				return td;
			else
				return null;
		}
		
		public IAssembly Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			Freeze();
			var cache = context.Compilation.CacheManager;
			IAssembly asm = (IAssembly)cache.GetShared(this);
			if (asm != null) {
				return asm;
			} else {
				asm = new DefaultResolvedAssembly(context.Compilation, this);
				return (IAssembly)cache.GetOrAddShared(this, asm);
			}
		}
		
		public override string ToString()
		{
			return "[" + GetType().Name + " " + assemblyName + "]";
		}
		
		sealed class DefaultResolvedAssembly : IAssembly, ITypeResolveContext
		{
			readonly DefaultUnresolvedAssembly unresolved;
			readonly ICompilation compilation;
			readonly ConcurrentDictionary<IUnresolvedTypeDefinition, ITypeDefinition> typeDict = new ConcurrentDictionary<IUnresolvedTypeDefinition, ITypeDefinition>();
			
			public DefaultResolvedAssembly(ICompilation compilation, DefaultUnresolvedAssembly unresolved)
			{
				this.compilation = compilation;
				this.unresolved = unresolved;
				this.AssemblyAttributes = unresolved.AssemblyAttributes.ToList().CreateResolvedAttributes(this);
				this.ModuleAttributes = unresolved.ModuleAttributes.ToList().CreateResolvedAttributes(this);
			}
			
			public IUnresolvedAssembly UnresolvedAssembly {
				get { return unresolved; }
			}
			
			public bool IsMainAssembly {
				get { return this.Compilation.MainAssembly == this; }
			}
			
			public string AssemblyName {
				get { return unresolved.AssemblyName; }
			}
			
			public IList<IAttribute> AssemblyAttributes { get; private set; }
			public IList<IAttribute> ModuleAttributes { get; private set; }
			
			public INamespace RootNamespace {
				get {
					throw new NotImplementedException();
				}
			}
			
			public ICompilation Compilation {
				get { return compilation; }
			}
			
			public bool InternalsVisibleTo(IAssembly assembly)
			{
				throw new NotImplementedException();
			}
			
			public ITypeDefinition GetTypeDefinition(string ns, string name, int typeParameterCount)
			{
				return GetTypeDefinition(unresolved.GetTypeDefinition(ns, name, typeParameterCount));
			}
			
			public ITypeDefinition GetTypeDefinition(IUnresolvedTypeDefinition unresolved)
			{
				if (unresolved == null)
					return null;
				return typeDict.GetOrAdd(unresolved, t => CreateTypeDefinition(t));
			}
			
			ITypeDefinition CreateTypeDefinition(IUnresolvedTypeDefinition unresolved)
			{
				if (unresolved.DeclaringTypeDefinition != null) {
					ITypeDefinition declaringType = GetTypeDefinition(unresolved.DeclaringTypeDefinition);
					return new DefaultResolvedTypeDefinition(new SimpleTypeResolveContext(declaringType), unresolved);
				} else if (unresolved.Name == "Void" && unresolved.Namespace == "System" && unresolved.TypeParameters.Count == 0) {
					return new VoidTypeDefinition(this, unresolved);
				} else {
					return new DefaultResolvedTypeDefinition(this, unresolved);
				}
			}
			
			IAssembly ITypeResolveContext.CurrentAssembly {
				get { return this; }
			}
			
			ITypeDefinition ITypeResolveContext.CurrentTypeDefinition {
				get { return null; }
			}
			
			IMember ITypeResolveContext.CurrentMember {
				get { return null; }
			}
		}
	}
}

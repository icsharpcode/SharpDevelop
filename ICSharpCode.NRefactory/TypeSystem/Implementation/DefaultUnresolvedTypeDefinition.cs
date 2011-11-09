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
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Represents an unresolved type definition.
	/// </summary>
	[Serializable]
	public class DefaultUnresolvedTypeDefinition : AbstractUnresolvedEntity, IUnresolvedTypeDefinition
	{
		TypeKind kind = TypeKind.Class;
		string namespaceName;
		IList<ITypeReference> baseTypes;
		IList<IUnresolvedTypeParameter> typeParameters;
		IList<IUnresolvedTypeDefinition> nestedTypes;
		IList<IUnresolvedMember> members;
		
		public DefaultUnresolvedTypeDefinition()
		{
			this.EntityType = EntityType.TypeDefinition;
		}
		
		public DefaultUnresolvedTypeDefinition(string namespaceName, string name)
		{
			this.EntityType = EntityType.TypeDefinition;
			this.namespaceName = namespaceName;
			this.Name = name;
		}
		
		public DefaultUnresolvedTypeDefinition(IUnresolvedTypeDefinition declaringTypeDefinition, string name)
		{
			this.EntityType = EntityType.TypeDefinition;
			this.DeclaringTypeDefinition = declaringTypeDefinition;
			this.namespaceName = declaringTypeDefinition.Namespace;
			this.Name = name;
		}
		
		public TypeKind Kind {
			get { return kind; }
			set {
				ThrowIfFrozen();
				kind = value;
			}
		}
		
		public bool AddDefaultConstructorIfRequired {
			get { return flags[FlagAddDefaultConstructorIfRequired]; }
			set {
				ThrowIfFrozen();
				flags[FlagAddDefaultConstructorIfRequired] = value;
			}
		}
		
		public override string Namespace {
			get { return namespaceName; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				ThrowIfFrozen();
				namespaceName = value;
			}
		}
		
		public override string ReflectionName {
			get {
				IUnresolvedTypeDefinition declaringTypeDef = this.DeclaringTypeDefinition;
				if (declaringTypeDef != null) {
					if (this.TypeParameters.Count > declaringTypeDef.TypeParameters.Count) {
						return declaringTypeDef.ReflectionName + "+" + this.Name + "`" + (this.TypeParameters.Count - declaringTypeDef.TypeParameters.Count).ToString();
					} else {
						return declaringTypeDef.ReflectionName + "+" + this.Name;
					}
				} else if (string.IsNullOrEmpty(namespaceName)) {
					if (this.TypeParameters.Count > 0)
						return this.Name + "`" + this.TypeParameters.Count.ToString();
					else
						return this.Name;
				} else {
					if (this.TypeParameters.Count > 0)
						return namespaceName + "." + this.Name + "`" + this.TypeParameters.Count.ToString();
					else
						return namespaceName + "." + this.Name;
				}
			}
		}
		
		public IList<ITypeReference> BaseTypes {
			get {
				if (baseTypes == null)
					baseTypes = new List<ITypeReference>();
				return baseTypes;
			}
		}
		
		public IList<IUnresolvedTypeParameter> TypeParameters {
			get {
				if (typeParameters == null)
					typeParameters = new List<IUnresolvedTypeParameter>();
				return typeParameters;
			}
		}
		
		public IList<IUnresolvedTypeDefinition> NestedTypes {
			get {
				if (nestedTypes == null)
					nestedTypes = new List<IUnresolvedTypeDefinition>();
				return nestedTypes;
			}
		}
		
		public IList<IUnresolvedMember> Members {
			get {
				if (members == null)
					members = new List<IUnresolvedMember>();
				return members;
			}
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (context.CurrentAssembly == null)
				throw new ArgumentException("An ITypeDefinition cannot be resolved in a context without a current assembly.");
			return context.CurrentAssembly.GetTypeDefinition(this) ?? (IType)SpecialType.UnknownType;
		}
		
		public virtual ITypeResolveContext CreateResolveContext(ITypeResolveContext parentContext)
		{
			return parentContext;
		}
	}
}

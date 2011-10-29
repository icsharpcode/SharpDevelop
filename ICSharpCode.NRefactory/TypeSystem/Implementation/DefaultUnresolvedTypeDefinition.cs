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
			this.namespaceName = namespaceName;
			this.Name = name;
		}
		
		public DefaultUnresolvedTypeDefinition(IUnresolvedTypeDefinition declaringTypeDefinition, string name)
		{
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
		
		public override string Namespace {
			get { return namespaceName; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				ThrowIfFrozen();
				namespaceName = value;
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
		
		public new ITypeDefinition Resolve(ITypeResolveContext context)
		{
			return (ITypeDefinition)base.Resolve(context);
		}
		
		IType ITypeReference.Resolve(ITypeResolveContext context)
		{
			return (ITypeDefinition)base.Resolve(context);
		}
		
		protected override IEntity ResolveInternal(ITypeResolveContext context)
		{
			throw new NotImplementedException();
		}
	}
}

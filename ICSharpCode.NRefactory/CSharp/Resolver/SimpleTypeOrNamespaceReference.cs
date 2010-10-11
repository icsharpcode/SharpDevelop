// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Represents a simple C# name. (a single identifier with an optional list of type arguments)
	/// </summary>
	public sealed class SimpleTypeOrNamespaceReference : ITypeOrNamespaceReference
	{
		readonly IMember parentMember;
		readonly ITypeDefinition parentTypeDefinition;
		readonly UsingScope parentUsingScope;
		readonly string identifier;
		readonly IList<ITypeReference> typeArguments;
		
		public SimpleTypeOrNamespaceReference(string identifier, IList<ITypeReference> typeArguments, IMember parentMember, ITypeDefinition parentTypeDefinition, UsingScope parentUsingScope)
		{
			if (identifier == null)
				throw new ArgumentNullException("identifier");
			this.identifier = identifier;
			this.typeArguments = typeArguments ?? EmptyList<ITypeReference>.Instance;
			this.parentMember = parentMember;
			this.parentTypeDefinition = parentTypeDefinition;
			this.parentUsingScope = parentUsingScope;
		}
		
		ResolveResult DoResolve(ITypeResolveContext context)
		{
			CSharpResolver r = new CSharpResolver(context);
			r.CurrentMember = parentMember;
			r.CurrentTypeDefinition = parentTypeDefinition.GetCompoundClass();
			r.UsingScope = parentUsingScope;
			IType[] typeArgs = new IType[typeArguments.Count];
			for (int i = 0; i < typeArgs.Length; i++) {
				typeArgs[i] = typeArguments[i].Resolve(context);
			}
			return r.LookupSimpleNamespaceOrTypeName(identifier, typeArgs);
		}
		
		public string ResolveNamespace(ITypeResolveContext context)
		{
			NamespaceResolveResult nrr = DoResolve(context) as NamespaceResolveResult;
			return nrr != null ? nrr.NamespaceName : null;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			TypeResolveResult rr = DoResolve(context) as TypeResolveResult;
			return rr != null ? rr.Type : SharedTypes.UnknownType;
		}
	}
}

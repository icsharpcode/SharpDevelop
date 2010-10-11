// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Represents a reference which could point to a type or namespace.
	/// </summary>
	public interface ITypeOrNamespaceReference : ITypeReference
	{
		string ResolveNamespace(ITypeResolveContext context);
	}
}

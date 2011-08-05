// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	public enum SimpleNameLookupMode
	{
		/// <summary>
		/// Normal name lookup in expressions
		/// </summary>
		Expression,
		/// <summary>
		/// Name lookup in expression, where the expression is the target of an invocation.
		/// Such a lookup will only return methods and delegate-typed fields.
		/// </summary>
		InvocationTarget,
		/// <summary>
		/// Normal name lookup in type references.
		/// </summary>
		Type,
		/// <summary>
		/// Name lookup in the type reference inside a using declaration.
		/// </summary>
		TypeInUsingDeclaration,
		/// <summary>
		/// Name lookup for base type references.
		/// </summary>
		BaseTypeReference
	}
}

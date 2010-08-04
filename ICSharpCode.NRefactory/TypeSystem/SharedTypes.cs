// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Contains static implementations of well-known types.
	/// </summary>
	public static class SharedTypes
	{
		/// <summary>
		/// Gets the type representing resolve errors.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It's immutable")]
		public readonly static IType UnknownType = new UnknownType();
		
		/// <summary>
		/// The null type is used as type of the null literal. It is a reference type without any members; and it is a subtype of all reference types.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It's immutable")]
		public readonly static IType Null = new NullType();
	}
}

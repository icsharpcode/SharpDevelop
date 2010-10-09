// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Util;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Contains extension methods for the type system.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Gets all base types.
		/// </summary>
		/// <remarks>This is the reflexive and transitive closure of <see cref="IType.GetBaseTypes"/>.
		/// Note that this method does not return all supertypes - doing so is impossible due to contravariance
		/// (and undesirable for covariance as the list could become very large).
		/// </remarks>
		public static IEnumerable<IType> GetAllBaseTypes(this IType type, ITypeResolveContext context)
		{
			List<IType> output = new List<IType>();
			Stack<ITypeDefinition> activeTypeDefinitions = new Stack<ITypeDefinition>();
			CollectAllBaseTypes(type, context, activeTypeDefinitions, output);
			return output;
		}
		
		static void CollectAllBaseTypes(IType type, ITypeResolveContext context, Stack<ITypeDefinition> activeTypeDefinitions, List<IType> output)
		{
			ITypeDefinition def = type.GetDefinition();
			if (def != null) {
				// Maintain a stack of currently active type definitions, and avoid having one definition
				// multiple times on that stack.
				// This is necessary to ensure the output is finite in the presence of cyclic inheritance:
				// class C<X> : C<C<X>> {} would not be caught by the 'no duplicate output' check, yet would
				// produce infinite output.
				if (activeTypeDefinitions.Contains(def))
					return;
				activeTypeDefinitions.Push(def);
			}
			// Avoid outputting a type more than once - necessary for "diamond" multiple inheritance
			// (e.g. C implements I1 and I2, and both interfaces derive from Object)
			if (!output.Contains(type)) {
				output.Add(type);
				foreach (IType baseType in type.GetBaseTypes(context)) {
					CollectAllBaseTypes(baseType, context, activeTypeDefinitions, output);
				}
			}
			if (def != null)
				activeTypeDefinitions.Pop();
		}
	}
}

// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Helper class for the GetAllBaseTypes() implementation.
	/// </summary>
	sealed class BaseTypeCollector : List<IType>
	{
		readonly ITypeResolveContext context;
		readonly Stack<ITypeDefinition> activeTypeDefinitions = new Stack<ITypeDefinition>();
		
		/// <summary>
		/// If this option is enabled, the list will not contain interfaces when retrieving the base types
		/// of a class.
		/// </summary>
		internal bool SkipImplementedInterfaces;
		
		public BaseTypeCollector(ITypeResolveContext context)
		{
			this.context = context;
		}
		
		public void CollectBaseTypes(IType type)
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
			if (!this.Contains(type)) {
				this.Add(type);
				foreach (IType baseType in type.GetBaseTypes(context)) {
					if (SkipImplementedInterfaces && def != null && def.Kind != TypeKind.Interface) {
						if (baseType.Kind == TypeKind.Interface) {
							// skip the interface
							continue;
						}
					}
					CollectBaseTypes(baseType);
				}
			}
			if (def != null)
				activeTypeDefinitions.Pop();
		}
	}
}

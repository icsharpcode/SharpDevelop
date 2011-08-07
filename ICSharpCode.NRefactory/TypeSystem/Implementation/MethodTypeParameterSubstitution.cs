// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Substitutes method type parameters with type arguments. Does not modify class type parameters.
	/// </summary>
	public class MethodTypeParameterSubstitution : TypeVisitor
	{
		readonly IList<IType> typeArguments;
		
		public MethodTypeParameterSubstitution(IList<IType> typeArguments)
		{
			this.typeArguments = typeArguments;
		}
		
		public override IType VisitTypeParameter(ITypeParameter type)
		{
			int index = type.Index;
			if (type.OwnerType == EntityType.Method) {
				if (index >= 0 && index < typeArguments.Count)
					return typeArguments[index];
				else
					return SharedTypes.UnknownType;
			} else {
				return base.VisitTypeParameter(type);
			}
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; i < typeArguments.Count; i++) {
				if (i > 0) b.Append(", ");
				b.Append("``");
				b.Append(i);
				b.Append(" -> ");
				b.Append(typeArguments[i]);
			}
			b.Append(']');
			return b.ToString();
		}
	}
}

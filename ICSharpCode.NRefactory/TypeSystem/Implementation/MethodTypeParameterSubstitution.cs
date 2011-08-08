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

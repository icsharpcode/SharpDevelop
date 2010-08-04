// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Type Reference used when the fully qualified type name is known.
	/// </summary>
	public class GetClassTypeReference : AbstractTypeReference
	{
		string fullTypeName;
		int typeParameterCount;
		
		public GetClassTypeReference(string fullTypeName, int typeParameterCount)
		{
			if (fullTypeName == null)
				throw new ArgumentNullException("fullTypeName");
			this.fullTypeName = fullTypeName;
			this.typeParameterCount = typeParameterCount;
		}
		
		public override IType Resolve(ITypeResolveContext context)
		{
			return context.GetClass(fullTypeName, typeParameterCount, StringComparer.Ordinal);
		}
		
		public override string ToString()
		{
			if (typeParameterCount == 0)
				return fullTypeName;
			else
				return fullTypeName + "`" + typeParameterCount;
		}
	}
}

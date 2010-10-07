// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

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
			if (context == null)
				throw new ArgumentNullException("context");
			return context.GetClass(fullTypeName, typeParameterCount, StringComparer.Ordinal) ?? SharedTypes.UnknownType;
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

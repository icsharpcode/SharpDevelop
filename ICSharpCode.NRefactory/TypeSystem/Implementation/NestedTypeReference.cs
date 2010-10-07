// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Type reference used to reference nested types.
	/// </summary>
	public sealed class NestedTypeReference : ITypeReference, ISupportsInterning
	{
		ITypeReference baseTypeRef;
		string name;
		int additionalTypeParameterCount;
		
		/// <summary>
		/// Creates a new NestedTypeReference.
		/// </summary>
		/// <param name="baseTypeRef">Reference to the base type.</param>
		/// <param name="name">Name of the nested class</param>
		/// <param name="additionalTypeParameterCount">Number of type parameters on the inner class (without type parameters on baseTypeRef)</param>
		public NestedTypeReference(ITypeReference baseTypeRef, string name, int additionalTypeParameterCount)
		{
			if (baseTypeRef == null)
				throw new ArgumentNullException("baseTypeRef");
			if (name == null)
				throw new ArgumentNullException("name");
			this.baseTypeRef = baseTypeRef;
			this.name = name;
			this.additionalTypeParameterCount = additionalTypeParameterCount;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			IType baseType = baseTypeRef.Resolve(context);
			int tpc = baseType.TypeParameterCount;
			foreach (IType type in baseType.GetNestedTypes(context)) {
				if (type.Name == name && type.TypeParameterCount == tpc + additionalTypeParameterCount)
					return type;
			}
			return SharedTypes.UnknownType;
		}
		
		public override string ToString()
		{
			if (additionalTypeParameterCount == 0)
				return baseTypeRef + "+" + name;
			else
				return baseTypeRef + "+" + name + "`" + additionalTypeParameterCount;
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			baseTypeRef = provider.Intern(baseTypeRef);
			name = provider.Intern(name);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return baseTypeRef.GetHashCode() ^ name.GetHashCode() ^ additionalTypeParameterCount;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			NestedTypeReference o = other as NestedTypeReference;
			return o != null && baseTypeRef == o.baseTypeRef && name == o.name && additionalTypeParameterCount == o.additionalTypeParameterCount;
		}
	}
}

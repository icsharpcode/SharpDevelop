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
	/// Type reference used to reference nested types.
	/// </summary>
	public class NestedTypeReference : AbstractTypeReference
	{
		ITypeReference baseTypeRef; string name; int typeParameterCount;
		
		public NestedTypeReference(ITypeReference baseTypeRef, string name, int typeParameterCount)
		{
			if (baseTypeRef == null)
				throw new ArgumentNullException("baseTypeRef");
			if (name == null)
				throw new ArgumentNullException("name");
			this.baseTypeRef = baseTypeRef;
			this.name = name;
			this.typeParameterCount = typeParameterCount;
		}
		
		public override IType Resolve(ITypeResolveContext context)
		{
			foreach (IType type in baseTypeRef.GetNestedTypes(context)) {
				if (type.Name == name && type.TypeParameterCount == typeParameterCount)
					return type;
			}
			return SharedTypes.UnknownType;
		}
		
		public override string ToString()
		{
			if (typeParameterCount == 0)
				return baseTypeRef + "+" + name;
			else
				return baseTypeRef + "+" + name + "`" + typeParameterCount;
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// The type reference that is the element of an enumerable.
	/// This class is used in combination with an InferredReturnType to
	/// represent the implicitly typed loop variable <c>v</c> in
	/// "<c>foreach (var v in enumerableInstance) {}</c>"
	/// </summary>
	public class ElementReturnType : ProxyReturnType
	{
		IReturnType enumerableType;
		IProjectContent pc;
		
		public ElementReturnType(IProjectContent pc, IReturnType enumerableType)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");
			this.enumerableType = enumerableType;
			this.pc = pc;
		}
		
		public override IReturnType BaseType {
			get {
				// get element type from enumerableType
				if (enumerableType.IsArrayReturnType)
					return enumerableType.CastToArrayReturnType().ArrayElementType;
				
				IClass c = enumerableType.GetUnderlyingClass();
				if (c == null)
					return null;
				IClass genumerable = pc.GetClass("System.Collections.Generic.IEnumerable", 1);
				if (c.IsTypeInInheritanceTree(genumerable)) {
					return MemberLookupHelper.GetTypeParameterPassedToBaseClass(enumerableType, genumerable, 0);
				}
				IClass enumerable = pc.GetClass("System.Collections.IEnumerable", 0);
				if (c.IsTypeInInheritanceTree(enumerable)) {
					return pc.SystemTypes.Object;
				}
				return null;
			}
		}
	}
}

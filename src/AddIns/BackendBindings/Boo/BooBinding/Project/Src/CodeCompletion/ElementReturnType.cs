// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace Grunwald.BooBinding.CodeCompletion
{
	/// <summary>
	/// The return type that is the element of an enumerable.
	/// Used to infer the type in "for x in enumerableVariable" loops.
	/// </summary>
	public class ElementReturnType : ProxyReturnType
	{
		IReturnType listType;
		IProjectContent pc;
		
		public ElementReturnType(IProjectContent pc, IReturnType listType)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");
			// listType is probably an InferredReturnType
			this.listType = listType;
			this.pc = pc;
		}
		
		public override IReturnType BaseType {
			get {
				// get element type from listType
				if (listType.IsArrayReturnType)
					return listType.CastToArrayReturnType().ArrayElementType;
				
				IClass c = listType.GetUnderlyingClass();
				if (c == null)
					return null;
				IClass genumerable = pc.GetClass("System.Collections.Generic.IEnumerable", 1);
				if (c.IsTypeInInheritanceTree(genumerable)) {
					return MemberLookupHelper.GetTypeParameterPassedToBaseClass(listType, genumerable, 0);
				}
				IClass enumerable = pc.GetClass("System.Collections.IEnumerable", 0);
				if (c.IsTypeInInheritanceTree(enumerable)) {
					// We can't use the EnumeratorItemType attribute because SharpDevelop
					// does not store attribute argument values in the cache.
					
					// HACK: Hacked in support for range(), take out when RangeEnumerator implements IEnumerable<int>
					if (c.FullyQualifiedName == "Boo.Lang.Builtins.RangeEnumerator") {
						return pc.SystemTypes.Int32;
					}
					
					return pc.SystemTypes.Object;
				}
				return null;
			}
		}
	}
}

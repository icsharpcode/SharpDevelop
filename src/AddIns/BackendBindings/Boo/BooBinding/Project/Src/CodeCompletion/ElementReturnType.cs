// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		
		public ElementReturnType(IReturnType listType)
		{
			// listType is probably an InferredReturnType
			this.listType = listType;
		}
		
		public override IReturnType BaseType {
			get {
				// get element type from listType
				if (listType.ArrayDimensions > 0)
					return listType.ArrayElementType;
				
				IClass c = listType.GetUnderlyingClass();
				if (c == null)
					return null;
				IClass genumerable = ProjectContentRegistry.Mscorlib.GetClass("System.Collections.Generic.IEnumerable", 1);
				if (c.IsTypeInInheritanceTree(genumerable)) {
					return MemberLookupHelper.GetTypeParameterPassedToBaseClass(listType, genumerable, 0);
				}
				IClass enumerable = ProjectContentRegistry.Mscorlib.GetClass("System.Collections.IEnumerable", 0);
				if (c.IsTypeInInheritanceTree(enumerable)) {
					// We can't use the EnumeratorItemType attribute because SharpDevelop
					// does not store attribute argument values in the cache.
					
					// HACK: Hacked in support for range(), take out when RangeEnumerator implements IEnumerable<int>
					if (c.FullyQualifiedName == "Boo.Lang.Builtins.RangeEnumerator") {
						return ReflectionReturnType.Int;
					}
					
					return ReflectionReturnType.Object;
				}
				return null;
			}
		}
	}
}

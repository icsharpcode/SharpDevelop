// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonClassResolver
	{
		public PythonClassResolver()
		{
		}
		
		public ResolveResult Resolve(PythonResolverContext context, ExpressionResult expressionResult)
		{
			IClass matchingClass = GetClass(context, expressionResult.Expression);
			if (matchingClass != null) {
				return CreateTypeResolveResult(matchingClass);
			}
			return null;
		}
		
		public IClass GetClass(PythonResolverContext context, string name)
		{
			if (String.IsNullOrEmpty(name)) {
				return null;
			}
			
			IClass matchedClass = context.GetClass(name);
			if (matchedClass != null) {
				return matchedClass;
			}
			
			return context.GetImportedClass(name);
		}
		
		TypeResolveResult CreateTypeResolveResult(IClass c)
		{
			return new TypeResolveResult(null, null, c);
		}
	}
}

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
	public class PythonDotNetMethodResolver
	{
		PythonClassResolver classResolver;
		
		public PythonDotNetMethodResolver(PythonClassResolver classResolver)
		{
			this.classResolver = classResolver;
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			MemberName memberName = new MemberName(expressionResult.Expression);
			IClass matchingClass = classResolver.GetClass(resolverContext, memberName.Type);
			if (matchingClass != null) {
				return new PythonMethodGroupResolveResult(matchingClass, memberName.Name);
			}
			return null;
		}
	}
}

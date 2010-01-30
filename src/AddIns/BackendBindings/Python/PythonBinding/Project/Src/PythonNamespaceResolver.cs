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
	public class PythonNamespaceResolver : IPythonResolver
	{
		public PythonNamespaceResolver()
		{
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			string actualNamespace = resolverContext.UnaliasImportedModuleName(expressionResult.Expression);
			if (resolverContext.NamespaceExists(actualNamespace)) {
				return new NamespaceResolveResult(null, null, actualNamespace);
			}
			return null;
		}
	}
}

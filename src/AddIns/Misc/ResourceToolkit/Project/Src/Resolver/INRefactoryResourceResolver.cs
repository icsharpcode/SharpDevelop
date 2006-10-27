// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Detects and resolves resource references using SharpDevelop.Dom and
	/// NRefactory. At this stage the parser and resolver have already been
	/// called.
	/// </summary>
	public interface INRefactoryResourceResolver
	{
		/// <summary>
		/// Tries to find a resource reference in the specified expression.
		/// </summary>
		/// <param name="expressionResult">The ExpressionResult for the expression.</param>
		/// <param name="expr">The AST representation of the full expression.</param>
		/// <param name="resolveResult">SharpDevelop's ResolveResult for the expression.</param>
		/// <param name="caretLine">The line where the expression is located.</param>
		/// <param name="caretColumn">The column where the expression is located.</param>
		/// <param name="fileName">The name of the source file where the expression is located.</param>
		/// <param name="fileContent">The content of the source file where the expression is located.</param>
		/// <returns>A ResourceResolveResult describing the referenced resource, or <c>null</c>, if this expression does not reference a resource in a known way.</returns>
		ResourceResolveResult Resolve(ExpressionResult expressionResult, Expression expr, ResolveResult resolveResult, int caretLine, int caretColumn, string fileName, string fileContent);
		
		/// <summary>
		/// Gets a list of patterns that can be searched for in the specified file
		/// to find possible resource references that are supported by this
		/// resolver.
		/// </summary>
		/// <param name="fileName">The name of the file to get a list of possible patterns for.</param>
		IEnumerable<string> GetPossiblePatternsForFile(string fileName);
	}
}

/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 30.11.2004
 * Time: 23:23
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Description of IResolver.
	/// </summary>
	public interface IResolver
	{
		/// <summary>
		/// Resolves an expression.
		/// The caretLineNumber and caretColumn is 1 based.
		/// </summary>
		ResolveResult Resolve(ExpressionResult expressionResult,
		                      int caretLineNumber,
		                      int caretColumn,
		                      string fileName,
		                      string fileContent);
		
		ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent);
	}
}

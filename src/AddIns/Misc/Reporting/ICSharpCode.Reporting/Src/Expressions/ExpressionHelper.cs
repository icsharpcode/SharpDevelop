// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Globalization;
using ICSharpCode.Reporting.Globals;
using Irony.Interpreter.Ast;

namespace ICSharpCode.Reporting.Expressions
{
	/// <summary>
	/// Description of ExpressionHelper.
	/// </summary>
	class ExpressionHelper
	{
		
		public static string ExtractExpressionPart (string src)
		{
			char v = Convert.ToChar("=",CultureInfo.CurrentCulture );
			return StringHelper.RightOf(src,v).Trim();
		}
		
		
		public static bool CanEvaluate (string expression)
		{
			if ((!String.IsNullOrEmpty(expression)) && (expression.StartsWith("=",StringComparison.OrdinalIgnoreCase))) {
				return true;
			}
			return false;
		}
		
		
		public static string ComposeAstNodeError (string branch,AstNode node) {
			return String.Format (CultureInfo.CurrentCulture,"Missing {0} <{1}>",branch,node.AsString);
		}
	}
}

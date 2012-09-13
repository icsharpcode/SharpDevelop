// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpExpressionFinder : IExpressionFinder
	{
		public RazorCSharpExpressionFinder()
		{
		}
		
		public ExpressionResult FindExpression(string text, int offset)
		{
			int position = offset - 1;
			while (position > 0 && IsValidCharacter(text[position])) {
				position--;
			}
			position++;
			string expression = text.Substring(position, offset - position);
			return new ExpressionResult(expression);
		}
		
		bool IsValidCharacter(char ch)
		{
			return Char.IsLetterOrDigit(ch) ||
				(ch == '.') ||
				(ch == '_');
		}
		
		public ExpressionResult FindFullExpression(string text, int offset)
		{
			return ExpressionResult.Empty;
		}
		
		public string RemoveLastPart(string expression)
		{
			return expression;
		}
	}
}

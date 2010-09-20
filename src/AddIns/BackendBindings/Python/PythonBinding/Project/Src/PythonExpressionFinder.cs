// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Finds python expressions for code completion.
	/// </summary>
	public class PythonExpressionFinder : IExpressionFinder
	{
		class ExpressionRange
		{
			public int Start;
			public int End;
			
			public int Length {
				get { return End - Start + 1; }
			}
		}
		
		ExpressionRange expressionRange = new ExpressionRange();
		
		public PythonExpressionFinder()
		{
		}
		
		/// <summary>
		/// Finds an expression around the current offset.
		/// </summary>
		/// <remarks>
		/// Currently not implemented. This method is used heavily
		/// in refactoring.
		/// </remarks>
		public ExpressionResult FindFullExpression(string text, int offset)
		{
			return new ExpressionResult(null);
		}
		
		/// <summary>
		/// Removes the last part of the expression.
		/// </summary>
		/// <example>
		/// "array[i]" => "array"
		/// "myObject.Field" => "myObject"
		/// "myObject.Method(arg1, arg2)" => "myObject.Method"
		/// </example>
		public string RemoveLastPart(string expression)
		{
			MemberName memberName = new MemberName(expression);
			return memberName.Type;
		}

		/// <summary>
		/// Finds an expression before the current offset.
		/// </summary>
		/// <remarks>
		/// The expression is found before the specified offset. The
		/// offset is just before the current cursor position. For example,
		/// if the user presses the dot character then the offset
		/// will be just before the dot. All characters before the offset and
		/// at the offset  are considered when looking for
		/// the expression. All characters afterwards are ignored.
		/// </remarks>
		public ExpressionResult FindExpression(string text, int offset)
		{
			if (!IsValidFindExpressionParameters(text, offset)) {
				return new ExpressionResult(null);
			}
			
			expressionRange.End = offset - 1;
			expressionRange.Start = FindExpressionStart(text, expressionRange.End);
			
			if (IsImportExpression(text)) {
				ExtendRangeToStartOfLine(text);
				return CreatePythonImportExpressionResult(text, expressionRange);
			}
			return CreateDefaultExpressionResult(text, expressionRange);
		}
		
		bool IsValidFindExpressionParameters(string text, int offset)
		{
			return (text != null) && IsValidOffset(text, offset);
		}

		/// <summary>
		/// This checks that the offset passed to the FindExpression method is valid. Usually the offset is
		/// just after the last character in the text.
		/// 
		/// The offset must be:
		/// 
		/// 1) Greater than zero.
		/// 2) Be inside the string.
		/// 3) Be just after the end of the text.
		/// </summary>
		bool IsValidOffset(string text, int offset)
		{
			return (offset > 0) && (offset <= text.Length);
		}
		
		int FindExpressionStart(string text, int offset)
		{
			while (offset >= 0) {
				char currentChar = text[offset];
				switch (currentChar) {
					case '\n':
					case '\r':
					case '\t':
					case ' ':
						return offset + 1;
				}
				offset--;
			}
			return 0;	
		}
		
		bool IsImportExpression(string text)
		{
			if (PythonImportExpression.IsImportExpression(text, expressionRange.End)) {
				return true;
			}
			
			if (IsSpaceCharacterBeforeExpression(text, expressionRange)) {
				if (PythonImportExpression.IsImportExpression(text, expressionRange.Start)) {
					return true;
				}
			}
			return false;
		}
		
		bool IsSpaceCharacterBeforeExpression(string text, ExpressionRange range)
		{
			int characterBeforeExpressionOffset = range.Start - 1;
			if (characterBeforeExpressionOffset >= 0) {
				return text[characterBeforeExpressionOffset] == ' ';
			}
			return false;
		}
		
		void ExtendRangeToStartOfLine(string text)
		{
			if (expressionRange.Start > expressionRange.End) {
				expressionRange.Start = expressionRange.End;
			}
			expressionRange.Start = FindLineStart(text, expressionRange.Start);
		}
		
		/// <summary>
		/// Finds the start of the line in the text starting from the
		/// offset and working backwards.
		/// </summary>
		int FindLineStart(string text, int offset)
		{
			while (offset >= 0) {
				char currentChar = text[offset];
				switch (currentChar) {
					case '\n':
						return offset + 1;
				}
				--offset;
			}
			return 0;
		}
		
		ExpressionResult CreatePythonImportExpressionResult(string text, ExpressionRange range)
		{
			return CreateExpressionResult(text, range, new PythonImportExpressionContext());
		}
		
		ExpressionResult CreateDefaultExpressionResult(string text, ExpressionRange range)
		{
			return CreateExpressionResult(text, range, ExpressionContext.Default);
		}
		
		ExpressionResult CreateExpressionResult(string text, ExpressionRange range, ExpressionContext context)
		{
			string expression = Substring(text, range);
			return new ExpressionResult(expression, context);
		}
		
		/// <summary>
		/// Gets the substring starting from the specified index and
		/// finishing at the specified end index. The character at the
		/// end index is included in the string.
		/// </summary>
		string Substring(string text, ExpressionRange range)
		{
			return text.Substring(range.Start, range.Length);
		}
	}
}

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
	/// <summary>
	/// Finds python expressions for code completion.
	/// </summary>
	public class PythonExpressionFinder : IExpressionFinder
	{
		static readonly string ImportStatement = "import";

		public PythonExpressionFinder()
		{
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
			if (text != null && IsValidOffset(text, offset)) {
				bool found = false;
				ExpressionContext expressionContext = ExpressionContext.Default;
				int currentOffset = offset - 1;
				while (!found && currentOffset >= 0) {
					char currentChar = text[currentOffset];
					switch (currentChar) {
						case '\n':
						case '\r':
						case '\t':
							found = true;
							break;
						case ' ':
							if (IsImportStatement(text, currentOffset - 1)) {
								// Make sure entire line is taken.
								currentOffset = FindLineStartOffset(text, currentOffset);
								currentOffset--;
								expressionContext = ExpressionContext.Importable;
							}
							found = true;
							break;
						default:
							currentOffset--;
							break;
					}
				}
				
				// Create expression result.
				string expression = Substring(text, currentOffset + 1, offset - 1);
				return new ExpressionResult(expression, expressionContext);
			}
			return new ExpressionResult(null);
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
			if (!String.IsNullOrEmpty(expression)) {
				int index = expression.LastIndexOf('.');
				if (index > 0) {
					return expression.Substring(0, index);
				}
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Gets the substring starting from the specified index and
		/// finishing at the specified end index. The character at the
		/// end index is included in the string.
		/// </summary>
		static string Substring(string text, int startIndex, int endIndex)
		{
			int length = endIndex - startIndex + 1;
			return text.Substring(startIndex, length);			
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
		static bool IsValidOffset(string text, int offset)
		{
			return (offset > 0) && (offset <= text.Length);
		}
		
		/// <summary>
		/// Checks that the preceding text is an import statement.
		/// The offset points to the last character of the import statement
		/// (i.e. the 't') if it exists.
		/// </summary>
		static bool IsImportStatement(string text, int offset)
		{
			// Trim any whitespace from the end.
			while (StringLongerThanImportStatement(offset)) {
				if (text[offset] == ' ') {
					--offset;
				} else {
					break;
				}
			}
			
			// Look for import statement.
			if (StringLongerThanImportStatement(offset)) {
				int i;
				for (i = ImportStatement.Length - 1; i >= 0; --i) {
					char currentChar = text[offset];
					if (currentChar != ImportStatement[i]) {
						return false;
					}
					--offset;
				}
				
				// Complete match?
				if (i == -1) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Tests that the string is long enough to contain the
		/// import statement. This tests that:
		/// 
		/// (offset + 1) >= ImportStatement.Length
		/// 
		/// The offset points to the last character in the string
		/// which could be part of the import statement.
		/// </summary>
		static bool StringLongerThanImportStatement(int offset)
		{
			return (offset + 1) >= ImportStatement.Length;
		}
		
		/// <summary>
		/// Finds the start of the line in the text starting from the
		/// offset and working backwards.
		/// </summary>
		static int FindLineStartOffset(string text, int offset)
		{
			while (offset >= 0) {
				char currentChar = text[offset];
				switch (currentChar) {
					case '\n':
						return offset + 1;
					default:
						--offset;
						break;
				}
			}
			return 0;
		}
	}
}

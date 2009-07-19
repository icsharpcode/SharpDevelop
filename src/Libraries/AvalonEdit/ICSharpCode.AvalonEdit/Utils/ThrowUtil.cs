// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// Contains exception-throwing helper methods.
	/// </summary>
	static class ThrowUtil
	{
		/// <summary>
		/// Throws an ArgumentNullException if <paramref name="val"/> is null; otherwise
		/// returns val.
		/// </summary>
		/// <example>
		/// Use this method to throw an ArgumentNullException when using parameters for base
		/// constructor calls.
		/// <code>
		/// public VisualLineText(string text) : base(ThrowUtil.CheckNotNull(text, "text").Length)
		/// </code>
		/// </example>
		public static T CheckNotNull<T>(T val, string parameterName) where T : class
		{
			if (val == null)
				throw new ArgumentNullException(parameterName);
			return val;
		}
		
		public static int CheckNotNegative(int val, string parameterName)
		{
			if (val < 0)
				throw new ArgumentOutOfRangeException(parameterName, val, "value must not be negative");
			return val;
		}
		
		public static int CheckInRangeInclusive(int val, string parameterName, int lower, int upper)
		{
			if (val < lower || val > upper)
				throw new ArgumentOutOfRangeException(parameterName, val, "Expected: " + lower.ToString() + " <= " + parameterName + " <= " + upper.ToString());
			return val;
		}
		
		public static InvalidOperationException NoDocumentAssigned()
		{
			return new InvalidOperationException("Document is null");
		}
		
		public static InvalidOperationException NoValidCaretPosition()
		{
			return new InvalidOperationException("Could not find a valid caret position in the line");
		}
	}
}

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
		/// public VisualLineText(string text) : base(ThrowUtil.CheckNull(text, "text").Length)
		/// </code>
		/// </example>
		public static T CheckNull<T>(T val, string parameterName) where T : class
		{
			if (val == null)
				throw new ArgumentNullException(parameterName);
			return val;
		}
		
		public static InvalidOperationException NoDocumentAssigned()
		{
			throw new InvalidOperationException("Document is null");
		}
	}
}

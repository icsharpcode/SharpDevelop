// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Helper methods for finding new lines.
	/// </summary>
	static class NewLineFinder
	{
		/// <summary>
		/// Gets the location of the next new line character, or SimpleSegment.Invalid
		/// if none is found.
		/// </summary>
		public static SimpleSegment NextNewLine(string text, int offset)
		{
			for (int i = offset; i < text.Length; i++) {
				switch (text[i]) {
					case '\r':
						if (i + 1 < text.Length) {
							if (text[i + 1] == '\n') {
								return new SimpleSegment(i, 2);
							}
						}
						goto case '\n';
					case '\n':
						return new SimpleSegment(i, 1);
				}
			}
			return SimpleSegment.Invalid;
		}
		
		/// <summary>
		/// Gets whether the specified string is a newline sequence.
		/// </summary>
		public static bool IsNewLine(string newLine)
		{
			return newLine == "\r\n" || newLine == "\n" || newLine == "\r";
		}
		
		/// <summary>
		/// Normalizes all new lines in <paramref name="input"/> to be <paramref name="newLine"/>.
		/// </summary>
		public static string NormalizeNewLines(string input, string newLine)
		{
			if (input == null)
				return null;
			Debug.Assert(IsNewLine(newLine));
			SimpleSegment ds = NextNewLine(input, 0);
			if (ds == SimpleSegment.Invalid) // text does not contain any new lines
				return input;
			StringBuilder b = new StringBuilder(input.Length);
			int lastEndOffset = 0;
			do {
				b.Append(input, lastEndOffset, ds.Offset - lastEndOffset);
				b.Append(newLine);
				lastEndOffset = ds.EndOffset;
				ds = NextNewLine(input, lastEndOffset);
			} while (ds != SimpleSegment.Invalid);
			// remaining string (after last newline)
			b.Append(input, lastEndOffset, input.Length - lastEndOffset);
			return b.ToString();
		}
		
		/// <summary>
		/// Gets the newline sequence used in the document at the specified line.
		/// </summary>
		public static string GetNewLineFromDocument(TextDocument document, int lineNumber)
		{
			DocumentLine line = document.GetLineByNumber(lineNumber);
			if (line.DelimiterLength == 0) {
				// at the end of the document, there's no line delimiter, so use the delimiter
				// from the previous line
				line = line.PreviousLine;
				if (line == null)
					return Environment.NewLine;
			}
			return document.GetText(line.Offset + line.Length, line.DelimiterLength);
		}
	}
}

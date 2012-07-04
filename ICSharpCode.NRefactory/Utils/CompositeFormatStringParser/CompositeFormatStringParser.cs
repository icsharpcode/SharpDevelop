//
// CompositeFormatStringParser.cs
//
// Authors:
//   Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Utils
{
	/// <summary>
	/// Composite format string parser.
	/// </summary>
	/// <remarks>
	/// Implements a complete parser for valid strings as well as
	/// error reporting and best-effort parsing for invalid strings.
	/// </remarks>		
	public class CompositeFormatStringParser
	{
		/// <summary>
		/// Parse the specified format string.
		/// </summary>
		/// <param name='format'>
		/// The format string.
		/// </param>
		public IEnumerable<IFormatStringSegment> Parse (string format)
		{
			// Format string syntax: http://msdn.microsoft.com/en-us/library/txafckwd.aspx
			int start = 0;
			var length = format.Length;
			for (int i = 0; i < length; i++) {
				if (format [i] == '{') {
					if (i + 1 == length) {
						// This is the end of the string.
						var textSegment = new TextSegment (format.Substring (start, i - start + 1), start) {
							Errors = {
								new DefaultFormatStringError {
									StartLocation = i,
									EndLocation = i + 1,
									Message = "Curly braces need to be escaped",
									OriginalText = "{",
									SuggestedReplacementText = "{{"
								}
							}
						};
						yield return textSegment;
						yield break;
					} else if (format [i + 1] == '{') {
						// Escape sequence; we're still in a text segment
						// Skip ahead to the char after the escape sequence
						++i;
						continue;
					} else {
						// This is the end of the text segment and the start of a FormatItem
						if (i - start > 0) {
							yield return TextSegment.FromUnescapedText (format.Substring (start, i - start));
							start = i;
						}
					}

					int index;
					int? alignment = null;
					string argumentFormat = null;

					// Index
					++i;
					index = int.Parse (GetUntil (format, ",:}", ref i));

					// Alignment
					if (format [i] == ',') {
						++i;
						while (i < length && char.IsWhiteSpace(format [i]))
							++i;
						if (format [i] == '-') {
							++i;
							alignment = -int.Parse (GetUntil (format, ":}", ref i));
						} else {
							alignment = int.Parse (GetUntil (format, ":}", ref i));
						}

					}

					// Format string
					if (format [i] == ':') {
						++i;
						int begin = i;
						while (i < length) {
							char c2 = format [i];
							if (c2 != '}') {
								++i;
								continue;
							}
							if (i + 1 < length && format [i + 1] == '}') {
								// Step past escape sequence
								i += 2;
								continue;
							} else {
								// This is the end of the FormatItem
								break;
							}
						}
						argumentFormat = TextSegment.UnEscape (format.Substring (begin, i - begin));
					}

					yield return new FormatItem (index, alignment, argumentFormat) { StartLocation = start, EndLocation = i + 1 };

					// The next potential text segment starts after this format item
					start = i + 1;
				}
			}
			// Handle remaining text
			if (start < length) {
				yield return new TextSegment (TextSegment.UnEscape (format.Substring (start)), start);
			}
		}

		string GetUntil (string format, string delimiters, ref int index)
		{
			int start = index;
			while (index < format.Length && !delimiters.Contains(format[index].ToString()))
				++index;

			return format.Substring (start, index - start);
		}

	}
}


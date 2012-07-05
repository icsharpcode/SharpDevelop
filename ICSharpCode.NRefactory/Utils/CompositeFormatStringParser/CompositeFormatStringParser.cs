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
using System.Globalization;

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

		public CompositeFormatStringParser ()
		{
			errors = new List<IFormatStringError> ();
		}

		/// <summary>
		/// Parse the specified format string.
		/// </summary>
		/// <param name='format'>
		/// The format string.
		/// </param>
		public IEnumerable<IFormatStringSegment> Parse (string format)
		{
			if (format == null)
				throw new ArgumentNullException ("format");

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
							yield return new TextSegment (UnEscape (format.Substring (start, i - start)));
							start = i;
						}
					}

					int index;
					int? alignment = null;
					string argumentFormat = null;

					// Index
					++i;
					index = ParseIndex(format, ref i);
					CheckForMissingEndBrace (format, i, length);

					// Alignment
					alignment = ParseAlignment(format, ref i, length);
					CheckForMissingEndBrace (format, i, length);

					// Format string
					argumentFormat = ParseSubFormatString(format, ref i, length);
					CheckForMissingEndBrace (format, i, length);

					// i may actually point outside of format; if that happens, we want the last position
					var endLocation = Math.Min (length, i + 1);
					var errors = GetErrors ();
					yield return new FormatItem (index, alignment, argumentFormat) {
						StartLocation = start,
						EndLocation = endLocation,
						Errors = errors
					};
					ClearErrors ();

					// The next potential text segment starts after this format item
					start = i + 1;
				}
			}
			// Handle remaining text
			if (start < length) {
				yield return new TextSegment (UnEscape (format.Substring (start)), start);
			}
		}

		int ParseIndex (string format, ref int i)
		{
			int parsedCharacters;
			int? maybeIndex = GetAndCheckNumber (format, ",:}", ref i, i, out parsedCharacters);
			if (parsedCharacters == 0) {
				AddError (new DefaultFormatStringError {
					StartLocation = i,
					EndLocation = i,
					Message = "Missing index",
					OriginalText = "",
					SuggestedReplacementText = "0"
				});
			}
			return maybeIndex ?? 0;
		}

		int? ParseAlignment(string format, ref int i, int length)
		{
			if (i < length && format [i] == ',') {
				int alignmentBegin = i;
				++i;
				while (i < length && char.IsWhiteSpace(format [i]))
					++i;
				if (i == length) {
					var originalText = format.Substring (alignmentBegin);
					var message = string.Format ("Unexpected end of string: '{0}'", originalText);
					AddMissingEndBraceError(alignmentBegin, i, message, originalText);
				} else {
					int parsedCharacters;
					var number = GetAndCheckNumber(format, ",:}", ref i, alignmentBegin + 1, out parsedCharacters);
					if (parsedCharacters == 0) {
						AddError (new DefaultFormatStringError {
							StartLocation = i,
							EndLocation = i,
							Message = "Missing alignment",
							OriginalText = "",
							SuggestedReplacementText = "0"
						});
					}
					return number ?? 0;
				}
			}
			return null;
		}

		string ParseSubFormatString(string format, ref int i, int length)
		{
			if (i < length && format [i] == ':') {
				++i;
				int begin = i;
				while (i < length) {
					char c = format [i];
					if (c != '}') {
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
				var escaped = format.Substring (begin, i - begin);
				return UnEscape (escaped);
			}
			return null;
		}

		void CheckForMissingEndBrace (string format, int i, int length)
		{
			if (i == length && format [length - 1] != '}') {
				AddMissingEndBraceError(i, i, "Missing '}'", "");
				return;
			}
			return;
		}
		
		string GetUntil (string format, string delimiters, ref int index)
		{
			int start = index;
			while (index < format.Length && !delimiters.Contains(format[index].ToString()))
				++index;
			
			return format.Substring (start, index - start);
		}
		
		int? GetNumber (string format, ref int index)
		{
			if (format.Length == 0) {
				return null;
			}
			int sum = 0;
			int i = index;
			bool positive = format [i] != '-';
			if (!positive)
				++i;
			int numberStartIndex = i;
			while (i < format.Length && format[i] >= '0' && format[i] <= '9') {
				sum = 10 * sum + format [i] - '0';
				++i;
			}
			if (i == numberStartIndex)
				return null;

			index = i;
			return positive ? sum : -sum;
		}

		int? GetAndCheckNumber(string format, string delimiters, ref int index, int numberFieldStart, out int parsedCharacters)
		{
			var numberText = GetUntil(format, delimiters, ref index);
			parsedCharacters = numberText.Length;
			int digitCount = 0;
			int? number = GetNumber(numberText, ref digitCount);
			if (digitCount != parsedCharacters) {
				// Not the entire number field could be parsed
				var suggestedNumber = (number ?? 0).ToString ();
				AddInvalidNumberFormatError(numberFieldStart, format.Substring(numberFieldStart, index - numberFieldStart), suggestedNumber);
			}
			return number;
		}

		public static string UnEscape (string unEscaped)
		{
			return unEscaped.Replace ("{{", "{").Replace ("}}", "}");
		}

		IList<IFormatStringError> errors;
		
		bool hasMissingEndBrace = false;

		void AddError (IFormatStringError error)
		{
			errors.Add (error);
		}

		void AddMissingEndBraceError(int start, int end, string message, string originalText)
		{
			// Only add a single missing end brace per format item
			if (hasMissingEndBrace)
				return;
			AddError (new DefaultFormatStringError {
				StartLocation = start,
				EndLocation = end,
				Message = message,
				OriginalText = originalText,
				SuggestedReplacementText = "}"
			});
			hasMissingEndBrace = true;
		}

		void AddInvalidNumberFormatError (int i, string number, string replacementText)
		{
			AddError (new DefaultFormatStringError {
				StartLocation = i,
				EndLocation = i + number.Length,
				Message = string.Format ("Invalid number '{0}'", number),
				OriginalText = number,
				SuggestedReplacementText = replacementText
			});
		}

		IList<IFormatStringError> GetErrors ()
		{
			return errors;
		}

		void ClearErrors ()
		{
			hasMissingEndBrace = false;
			errors = new List<IFormatStringError> ();
		}
	}
}


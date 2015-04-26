// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Parses output text in the Output Build pad window and extracts source code
	/// file references.
	/// </summary>
	/// <remarks>
	/// Supported formats:
	/// C#: "d:\somedir\somefile.ext(12,34)"
	/// NUnit: "in d:\somedir\somefile.ext:line 12" (stacktrace format)
	/// C++: "d:\somedir\somefile.ext(12)" (also VB and some NUnit failures)
	/// </remarks>
	public static class OutputTextLineParser
	{
		/// <summary>
		/// Extracts source code file reference from the c# compiler output.
		/// </summary>
		/// <param name="lineText">The text line to parse.</param>
		/// <returns>A <see cref="FileLineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetCSharpCompilerFileLineReference(string lineText)
		{
			if (lineText != null) {
				Match match = Regex.Match(lineText, @"\b(\w:[/\\].*?)\((\d+),(\d+)\)");
				if (match.Success) {
					try	{
						int line = Convert.ToInt32(match.Groups[2].Value);
						int col = Convert.ToInt32(match.Groups[3].Value);
						
						return new FileLineReference(match.Groups[1].Value, line, col);
					} catch (FormatException) {
					} catch (OverflowException) {
						// Ignore.
					}
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Extracts source code file reference.
		/// </summary>
		/// <param name="lineText">The text line to parse.</param>
		/// <returns>A <see cref="FileLineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetFileLineReference(string lineText)
		{
			FileLineReference lineReference = GetCSharpCompilerFileLineReference(lineText);
			
			if (lineReference == null) {
				lineReference = GetNUnitOutputFileLineReference(lineText, false);
			}
			
			if (lineReference == null) {
				// Also works for VB compiler output.
				lineReference = GetCppCompilerFileLineReference(lineText);
			}
			
			return lineReference;
		}
		
		/// <summary>
		/// Extracts source code file reference from NUnit output. (stacktrace format)
		/// </summary>
		/// <param name="lineText">The text line to parse.</param>
		/// <param name="multiline">The <paramref name="lineText"/> text is multilined.</param>
		/// <returns>A <see cref="FileLineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetNUnitOutputFileLineReference(string lineText, bool multiline)
		{
			RegexOptions regexOptions = multiline ? RegexOptions.Multiline : RegexOptions.None;
			
			FileLineReference result = null;
			
			if (lineText != null) {
				Match match = CreateStackTraceMatch(lineText, regexOptions);
				while (match.Success) {
					try	{
						int line = Convert.ToInt32(match.Groups[2].Value);
						result = new FileLineReference(match.Groups[1].Value, line);
					} catch (FormatException) {
					} catch (OverflowException) {
						// Ignore.
					}
					match = match.NextMatch();
				}
			}
			
			return result;
		}

		static readonly System.Reflection.MethodInfo GetResourceString = typeof(Environment)
			.GetMethod("GetResourceString", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic,
			           null, new[] { typeof(string) }, null);
		
		static Match CreateStackTraceMatch(string lineText, RegexOptions regexOptions)
		{
			string line = "in {0}:line {1}", pattern;
			bool useResourceString = false;
			Match match;
			
			do
			{
				pattern = line.Replace("{0}", @"(\w:[/\\].*?)").Replace("{1}", @"(\d+)");
				match = Regex.Match(lineText, pattern, regexOptions);
				if (useResourceString || match.Success || GetResourceString == null)
					break;
				
				line = (string)GetResourceString.Invoke(null, new[] { "StackTrace_InFileLineNumber" });
				useResourceString = true;
				
			} while (true);

			return match;
		}
		
		/// <summary>
		/// Extracts source code file reference from the c++ or VB.Net compiler output.
		/// </summary>
		/// <param name="lineText">The text line to parse.</param>
		/// <returns>A <see cref="FileLineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetCppCompilerFileLineReference(string lineText)
		{
			if (lineText != null ) {
				
				Match match = Regex.Match(lineText, @"\b(\w:[/\\].*?)\((\d+)\)");
				
				if (match.Success) {
					try	{
						int line = Convert.ToInt32(match.Groups[2].Value);
						
						return new FileLineReference(match.Groups[1].Value.Trim(), line);
					} catch (FormatException) {
					} catch (OverflowException) {
						// Ignore.
					}
				}
			}
			
			return null;
		}
	}
}

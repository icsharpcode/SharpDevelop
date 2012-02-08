// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		/// <param name="multiline">The <paramref name="line"/> text is multilined.</param>
		/// <returns>A <see cref="FileLineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetNUnitOutputFileLineReference(string lineText, bool multiline)
		{
			RegexOptions regexOptions = multiline ? RegexOptions.Multiline : RegexOptions.None;
			
			FileLineReference result = null;
			
			if (lineText != null) {
				Match match = Regex.Match(lineText, @"\b(\w:[/\\].*?):line\s(\d+)?\r?$", regexOptions);
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

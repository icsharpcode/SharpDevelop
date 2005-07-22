// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Parses output text in the Output Build pad window and extracts source code
	/// file references.
	/// </summary>
	public class OutputTextLineParser
	{
		/// <summary>
		/// Creates a new instance of the <see cref="OutputTextlineParser"/> class.
		/// </summary>
		OutputTextLineParser()
		{
		}

		/// <summary>
		/// Extracts source code file reference from the compiler output.
		/// </summary>
		/// <param name="line">The text line to parse.</param>
		/// <returns>A <see cref="lineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetCompilerOutputFileLineReference(string lineText)
		{
			if (lineText != null) {
				Match match = Regex.Match(lineText, @"^(.*?)\(([\d]*),([\d]*)\)");
				if (match.Success) {
					try	{
						// Take off 1 for line/pos since SharpDevelop is zero index based.
						int line = Convert.ToInt32(match.Groups[2].Value) - 1;
						int pos = Convert.ToInt32(match.Groups[3].Value) - 1;                     
						
						return new FileLineReference(match.Groups[1].Value, line, pos);
					} catch (Exception) {
						// Ignore.
					}
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Extracts source code file reference.
		/// </summary>
		/// <param name="line">The text line to parse.</param>
		/// <returns>A <see cref="lineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetFileLineReference(string lineText)
		{
			FileLineReference lineReference = GetCompilerOutputFileLineReference(lineText);
			
			if (lineReference == null) {
				lineReference = GetNUnitOutputFileLineReference(lineText, false);
			}
			
			return lineReference;
		}
		
		/// <summary>
		/// Extracts source code file reference from NUnit output.
		/// </summary>
		/// <param name="line">The text line to parse.</param>
		/// <param name="multiline">The <paramref name="line"/> text is multilined.</param>
		/// <returns>A <see cref="lineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetNUnitOutputFileLineReference(string lineText, bool multiline)
		{
			RegexOptions regexOptions = multiline ? RegexOptions.Multiline : RegexOptions.None;
			
			if (lineText != null) {
				Match match = Regex.Match(lineText, @"^.*?\sin\s(.*?):line\s(\d*)?$", regexOptions);
				
				if (match.Success) {
					try	{
						int line = Convert.ToInt32(match.Groups[2].Value) - 1;
						return new FileLineReference(match.Groups[1].Value, line);
					} catch (Exception) {
						// Ignore.i
					}
				}
			}
			
			return null;
		}		
	}
}

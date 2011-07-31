// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text.RegularExpressions;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.VBNetBinding
{
	public static class LanguageUtils
	{
		public static string TrimComments(this string line)
		{
			if (string.IsNullOrEmpty(line))
				return string.Empty;
			
			bool inStr = false;
			
			for (int i = 0; i < line.Length; i++) {
				if (line[i] == '"')
					inStr = !inStr;
				if (line[i] == '\'' && !inStr)
					return line.Substring(0, i);
			}
			
			return line;
		}
		
		public static string TrimPreprocessorDirectives(this string line)
		{
			if (string.IsNullOrEmpty(line))
				return string.Empty;
			
			bool wsOnly = true;
			
			for (int i = 0; i < line.Length; i++) {
				if (line[i] == '#' && wsOnly) {
					if (i < line.Length - 1) {
						if (char.IsLetter(line[i + 1]))
							return line.Substring(0, i);
					} else
						return line.Substring(0, i);
				}
				if (!char.IsWhiteSpace(line[i]))
					wsOnly = false;
			}
			
			return line;
		}
		
		public static string TrimLine(this string line)
		{
			if (string.IsNullOrEmpty(line))
				return string.Empty;
			// remove string content
			MatchCollection matches = Regex.Matches(line, "\"[^\"]*?\"", RegexOptions.Singleline);
			foreach (Match match in matches) {
				line = line.Remove(match.Index, match.Length).Insert(match.Index, new string('-', match.Length));
			}
			// remove comments
			return TrimComments(line);
		}
		
		public static bool IsInsideDocumentationComment(ITextEditor editor)
		{
			return IsInsideDocumentationComment(editor, editor.Document.GetLineForOffset(editor.Caret.Offset), editor.Caret.Offset);
		}
		
		public static bool IsInsideDocumentationComment(ITextEditor editor, IDocumentLine curLine, int cursorOffset)
		{
			for (int i = curLine.Offset; i < cursorOffset; ++i) {
				char ch = editor.Document.GetCharAt(i);
				if (ch == '"')
					return false;
				if (ch == '\'' && i + 2 < cursorOffset && editor.Document.GetCharAt(i + 1) == '\'' &&
				    editor.Document.GetCharAt(i + 2) == '\'')
					return true;
			}
			return false;
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Scripting
{
	public class TextSentToScriptingConsole
	{
		public List<string> lines = new List<string>();
		
		public TextSentToScriptingConsole()
		{
		}
		
		public void AddText(string text)
		{
			GetLines(text);
		}
		
		public void AddLines(IList<string> linesToAdd)
		{
			lines.AddRange(linesToAdd);
		}
		
		void GetLines(string text)
		{
			string[] linesToAdd = ConvertTextToLines(text);
			lines.AddRange(linesToAdd);
		}
		
		string[] ConvertTextToLines(string text)
		{
			text = text.Replace("\r\n", "\n");
			return text.Split('\n');
		}
		
		public bool HasLine {
			get { return lines.Count > 0; }
		}
		
		public bool HasAtLeastOneLine {
			get { return lines.Count > 1; }
		}
		
		/// <summary>
		/// Returns line with '\r\n' if not the last line of text.
		/// </summary>
		public string RemoveFirstLine()
		{
			string line = GetFirstLine();
			if (line != null) {
				lines.RemoveAt(0);
			}
			return line;
		}
		
		public string GetFirstLine()
		{
			if (HasLine) {
				if (lines.Count > 1) {
					string firstLine = lines[0] + "\r\n";
					return firstLine;
				}
				return lines[0];
			}
			return null;
		}
	}
}

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

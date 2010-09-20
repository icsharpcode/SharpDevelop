// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Scripting
{
	public class ScriptingConsoleUnreadLines
	{
		List<string> unreadLines = new List<string>();
		
		public ScriptingConsoleUnreadLines()
		{
		}
		
		public bool IsLineAvailable {
			get { return unreadLines.Count > 0; }
		}
		
		public int Count  {
			get { return unreadLines.Count; }
		}
		
		public string RemoveFirstLine()
		{
			if (IsLineAvailable) {
				string line = unreadLines[0];
				unreadLines.RemoveAt(0);
				return line;
			}
			return null;
		}
		
		public void AddLine(string line)
		{
			unreadLines.Add(line);
		}
		
		public void AddAllLinesExceptLast(IList<string> lines)
		{
			int howMany = lines.Count - 1;
			for (int i = 0; i < howMany; ++i) {
				string line = lines[i];
				unreadLines.Add(line);
			}
		}
		
		public string[] ToArray()
		{
			return unreadLines.ToArray();
		}
	}
}

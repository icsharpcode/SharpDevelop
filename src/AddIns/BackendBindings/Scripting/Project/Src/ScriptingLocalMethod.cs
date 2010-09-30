// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Scripting
{
	/// <summary>
	/// Used to extract the code for a method based on the range of lines.
	/// </summary>
	public class ScriptingLocalMethod
	{
		string code = String.Empty;
		
		public ScriptingLocalMethod(string code)
		{
			if (code != null) {
				this.code = code;
			}
		}
		
		/// <summary>
		/// End line is one based.
		/// </summary>
		public string GetCode(int endLine)
		{
			int endIndex = FindIndexForEndOfLine(endLine);
			if (endIndex > 0) {
				return code.Substring(0, endIndex);
			}
			return code;
		}
		
		int FindIndexForEndOfLine(int line)
		{
			int index = 0;
			for (int i = 0; i < line; ++i) {
				index = code.IndexOf('\n', index) + 1;
			}
			return index;
		}
	}
}

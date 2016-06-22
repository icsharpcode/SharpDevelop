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

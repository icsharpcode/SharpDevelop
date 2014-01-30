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
using Antlr.Runtime;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptRegionStart
	{
		public static readonly string RegionStartText = "//#region ";
		public static readonly int RegionStartTextLength = RegionStartText.Length;
		
		IToken token;
		string name;
		
		public JavaScriptRegionStart(IToken token)
		{
			this.token = token;
		}
		
		public string Name {
			get {
				if (name == null) {
					GetName();
				}
				return name;
			}
		}
		
		void GetName()
		{
			string text = token.Text;
			int index = text.IndexOf(RegionStartText);
			name = text.Substring(index + RegionStartTextLength);
		}
		
		public static bool IsRegionStart(IToken token)
		{
			return token.Text.StartsWith(RegionStartText);
		}
		
		public int Line {
			get { return token.Line; }
		}
		
		public int StartColumn {
			get { return token.BeginColumn(); }
		}
	}
}

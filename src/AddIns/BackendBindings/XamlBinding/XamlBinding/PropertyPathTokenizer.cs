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
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.XamlBinding
{
	public class PropertyPathTokenizer
	{
		string value;
		int offset;
		
		public static readonly char[] ControlChars = new char[] { '.', ',', '(', ')', '[', ']', '/' };
		
		PropertyPathTokenizer(string value)
		{
			this.value = value;
			this.offset = 0;
		}
		
		bool NextToken(out string token)
		{
			token = "";
			
			if (MoveToNext()) {
				switch (value[offset]) {
					case '.':
					case '(':
					case ')':
					case '[':
					case ']':
					case ',':
					case '/':
						token = value[offset].ToString();
						offset++;
						return true;
					default:
						string text = "";
						while (!AtEnd() && char.IsLetterOrDigit(value[offset])) {
							text += value[offset];
							offset++;
						}
						
						token = text;
						return true;
				}
			}
			
			return false;
		}
		
		bool MoveToNext()
		{
			// skip all invalid chars
			while (!AtEnd() && !char.IsLetterOrDigit(value[offset]) && !ControlChars.Contains(value[offset]))
				offset++;
			
			return !AtEnd();
		}
		
		bool AtEnd()
		{
			return offset >= value.Length;
		}
		
		public static IEnumerable<string> Tokenize(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			
			PropertyPathTokenizer tokenizer = new PropertyPathTokenizer(value);
			
			string token;
			
			while (tokenizer.NextToken(out token))
				yield return token;
		}
	}
}

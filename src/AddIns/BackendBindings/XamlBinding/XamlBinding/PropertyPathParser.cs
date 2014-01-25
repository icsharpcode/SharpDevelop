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
using System.Linq;

namespace ICSharpCode.XamlBinding
{
	public class PropertyPathParser
	{
		public static IEnumerable<PropertyPathSegment> Parse(string text)
		{
			string token = "";
			
			bool inBrace = false;
			bool inIndexer = false;
			bool isSourceTraversal = false;
			
			string lastToken = "";
			
			foreach (string v in PropertyPathTokenizer.Tokenize(text)) {
				lastToken = v;
				
				if (v == "(") {
					inBrace = true;
					continue;
				}
				
				if (v == "[") {
					inIndexer = true;
					if (!inBrace && !string.IsNullOrEmpty(token)) {
						yield return new PropertyPathSegment(SegmentKind.PropertyOrType, token);
						token = "";
					}
					continue;
				}
				
				if (v == ")") {
					inBrace = false;
					if (!string.IsNullOrEmpty(token))
						yield return new PropertyPathSegment(SegmentKind.AttachedProperty, token);
					token = "";
					continue;
				}
				
				if (v == "]") {
					inIndexer = false;
					if (!inBrace && !string.IsNullOrEmpty(token)) {
						yield return new PropertyPathSegment(SegmentKind.Indexer, token);
						token = "";
					}
					continue;
				}
				
				if (inBrace)
					token += v;
				else if (inIndexer)
					token += v;
				else if (v == ".") {
					if (!string.IsNullOrEmpty(token))
						yield return new PropertyPathSegment(isSourceTraversal ? SegmentKind.SourceTraversal : SegmentKind.PropertyOrType, token);
					token = "";
					isSourceTraversal = false;
				} else if (v == "/") {
					token += v;
					isSourceTraversal = true;
				} else {
					token += v;
				}
			}
			
			if (inBrace && !string.IsNullOrEmpty(token)) {
				yield return new PropertyPathSegment(SegmentKind.AttachedProperty, "(" + token.Trim('.', '/'));
			}
			if (!string.IsNullOrEmpty(lastToken)) {
				char c = lastToken.First();
				
				if (c == ')' || c == ']')
					yield break;
				
				if (PropertyPathTokenizer.ControlChars.Contains(c))
					yield return new PropertyPathSegment(SegmentKind.ControlChar, c.ToString());
				else if (!inBrace && !string.IsNullOrEmpty(token))
					yield return new PropertyPathSegment(isSourceTraversal ? SegmentKind.SourceTraversal : SegmentKind.PropertyOrType, token);
			}
		}
	}
}

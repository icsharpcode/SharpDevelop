// 
// NavigationBarItem.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2014 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.TypeScriptBinding.Hosting
{
	public class NavigationBarItem
	{
		public NavigationBarItem()
		{
			text = "";
			kind = "";
			kindModifiers = "";
			spans = new TextSpan[0];
			childItems = new NavigationBarItem[0];
		}
		
		public string text { get; set; }
		public string kind { get; set; }
		public string kindModifiers { get; set; }
		public TextSpan[] spans { get; set; }
		public NavigationBarItem[] childItems { get; set; }
		public int indent { get; set; }
		public bool bolded { get; set; }
		public bool grayed { get; set; }
		
		internal int minChar {
			get {
				if (HasSpans) {
					return spans[0].start;
				}
				return 0;
			}
		}
		
		internal bool HasSpans {
			get { return (spans != null) && (spans.Length > 0); }
		}
		
		internal int limChar {
			get {
				if (HasSpans) {
					TextSpan span = spans[0];
					return span.start + span.length;
				}
				return 0;
			}
		}
		
		internal DomRegion ToRegionStartingFromOpeningCurlyBrace(IDocument document)
		{
			int startOffset = GetOpeningCurlyBraceOffsetForRegion(document);
			TextLocation  start = document.GetLocation(startOffset);
			TextLocation  end = document.GetLocation(limChar);
			return new DomRegion(start, end);
		}
		
		int GetOpeningCurlyBraceOffsetForRegion(IDocument document)
		{
			int offset = minChar;
			while (offset < limChar) {
				if (document.GetCharAt(offset) == '{') {
					return offset - 1;
				}
				++offset;
			}
			
			return minChar;
		}
	}
}

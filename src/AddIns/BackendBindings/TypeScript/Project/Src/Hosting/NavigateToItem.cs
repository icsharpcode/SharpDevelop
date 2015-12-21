// 
// NavigateToItem.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
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
	public class NavigateToItem
	{
		public string name { get; set; }
		public string kind { get; set; }
		public string kindModifiers { get; set; }
		public string matchKind { get; set; }
		public bool isCaseSensitive { get; set; }
		public string fileName { get; set; }
		public TextSpan textSpan { get; set; }
		public string containerName { get; set; }
		public string containerKind { get; set; }
		
		internal int minChar {
			get { return textSpan.start; }
		}
		
		internal int limChar {
			get { return minChar + length; }
		}
		
		internal int length {
			get { return textSpan.length; }
		}
		
		public DomRegion ToRegion(IDocument document)
		{
			TextLocation start = document.GetLocation(minChar);
			TextLocation end = document.GetLocation(limChar);
			return new DomRegion(start, end);
		}
		
		public DomRegion ToRegionStartingFromOpeningCurlyBrace(IDocument document)
		{
			int startOffset = GetOpeningCurlyBraceOffsetForRegion(document);
			TextLocation start = document.GetLocation(startOffset);
			TextLocation end = document.GetLocation(limChar);
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
		
		public bool HasContainer()
		{
			return !String.IsNullOrEmpty(containerName);
		}
		
		public string GetFullName()
		{
			if (HasContainer()) {
				return String.Format("{0}.{1}", containerName, name);
			}
			return name;
		}
		
		public string GetContainerParentName()
		{
			int dotPosition = containerName.IndexOf('.');
			if (dotPosition > 0) {
				return containerName.Substring(0, dotPosition);
			}
			return null;
		}
	}
}

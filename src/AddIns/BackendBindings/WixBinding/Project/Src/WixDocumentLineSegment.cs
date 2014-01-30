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
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.WixBinding
{
	public class WixDocumentLineSegment
	{
		int offset;
		int length;
		
		public WixDocumentLineSegment(int offset, int length)
		{
			this.offset = offset;
			this.length = length;
		}
		
		public int Offset {
			get { return offset; }
			set { offset = value; }
		}
		
		public int Length {
			get { return length; }
			set { length = value; }
		}
		
		public override string ToString()
		{
			return String.Format("[LineSegment: Offset {0}, Length {1}]", offset, length);
		}
	
		public override int GetHashCode()
		{
			return offset.GetHashCode() ^ length.GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			WixDocumentLineSegment rhs = obj as WixDocumentLineSegment;
			if (rhs != null) {
				return (offset == rhs.offset) && (length == rhs.length);
			}
			return false;
		}
		
		public static WixDocumentLineSegment ConvertRegionToSegment(IDocument document, DomRegion region)
		{
			// Single line region
			if (IsSingleLineRegion(region)) {
				return ConvertRegionToSingleLineSegment(document, region);
			}
			return ConvertRegionToMultiLineSegment(document, region);
		}
		
		static bool IsSingleLineRegion(DomRegion region)
		{
			return region.BeginLine == region.EndLine;
		}
		
		static WixDocumentLineSegment ConvertRegionToSingleLineSegment(IDocument document, DomRegion region)
		{
			IDocumentLine documentLine = document.GetLineByNumber(region.BeginLine);
			return new WixDocumentLineSegment(documentLine.Offset + region.BeginColumn - 1, 
					region.EndColumn - region.BeginColumn + 1);
		}
		
		static WixDocumentLineSegment ConvertRegionToMultiLineSegment(IDocument document, DomRegion region)
		{
			int length = 0;
			int startOffset = 0;
			for (int line = region.BeginLine; line <= region.EndLine; ++line) {
				IDocumentLine currentDocumentLine = document.GetLineByNumber(line);
				if (line == region.BeginLine) {
					length += currentDocumentLine.TotalLength - region.BeginColumn;
					startOffset = currentDocumentLine.Offset + region.BeginColumn - 1;
				} else if (line < region.EndLine) {
					length += currentDocumentLine.TotalLength;
				} else {
					length += region.EndColumn + 1;
				}
			}
			return new WixDocumentLineSegment(startOffset, length);
		}
	}
}

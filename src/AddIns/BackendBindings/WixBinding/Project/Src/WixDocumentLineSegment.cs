// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

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
			IDocumentLine documentLine = document.GetLine(region.BeginLine);
			return new WixDocumentLineSegment(documentLine.Offset + region.BeginColumn - 1, 
					region.EndColumn - region.BeginColumn + 1);
		}
		
		static WixDocumentLineSegment ConvertRegionToMultiLineSegment(IDocument document, DomRegion region)
		{
			int length = 0;
			int startOffset = 0;
			for (int line = region.BeginLine; line <= region.EndLine; ++line) {
				IDocumentLine currentDocumentLine = document.GetLine(line);
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

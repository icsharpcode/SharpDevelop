// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Represents a selected segment.
	/// </summary>
	public class SelectionSegment : ISegment
	{
		int startOffset, endOffset;
		int startVC, endVC;
		
		public SelectionSegment(int startOffset, int endOffset)
		{
			this.startOffset = Math.Min(startOffset, endOffset);
			this.endOffset = Math.Max(startOffset, endOffset);
		}
		
		public SelectionSegment(int startOffset, int startVC, int endOffset, int endVC)
		{
			if (startOffset <= endOffset) {
				this.startOffset = startOffset;
				this.startVC = startVC;
				this.endOffset = endOffset;
				this.endVC = endVC;
			} else {
				this.startOffset = endOffset;
				this.startVC = endVC;
				this.endOffset = startOffset;
				this.endVC = startVC;
			}
		}
		
		public int StartOffset {
			get { return startOffset; }
		}
		
		public int EndOffset {
			get { return endOffset; }
		}
		
		public int StartVisualColumn {
			get { return startVC; }
		}
		
		public int EndVisualColumn {
			get { return endVC; }
		}
		
		int ISegment.Offset {
			get { return startOffset; }
		}
		
		/// <inheritdoc/>
		public int Length {
			get { return endOffset - startOffset; }
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// A section that can be folded.
	/// </summary>
	public sealed class FoldingSection : TextSegment
	{
		FoldingManager manager;
		bool isFolded;
		CollapsedLineSection collapsedSection;
		
		/// <summary>
		/// Gets/sets if the section is folded.
		/// </summary>
		public bool IsFolded {
			get { return isFolded; }
			set {
				if (isFolded != value) {
					isFolded = value;
					if (value) {
						if (manager != null) {
							DocumentLine startLine = manager.document.GetLineByOffset(StartOffset);
							DocumentLine endLine = manager.document.GetLineByOffset(EndOffset);
							if (startLine != endLine) {
								DocumentLine startLinePlusOne = startLine.NextLine;
								collapsedSection = manager.textView.CollapseLines(startLinePlusOne, endLine);
							}
						}
					} else {
						if (collapsedSection != null) {
							collapsedSection.Uncollapse();
							collapsedSection = null;
						}
					}
					if (manager != null)
						manager.textView.Redraw(this, DispatcherPriority.Normal);
				}
			}
		}
		
		internal FoldingSection(FoldingManager manager, int startOffset, int endOffset)
		{
			this.manager = manager;
			this.StartOffset = startOffset;
			this.Length = endOffset - startOffset;
		}
		
		internal void Removed()
		{
			if (collapsedSection != null) {
				collapsedSection.Uncollapse();
				collapsedSection = null;
			}
			manager = null;
		}
	}
}

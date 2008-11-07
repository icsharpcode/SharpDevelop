// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// VisualLineElement that represents a piece of text.
	/// </summary>
	public class VisualLineText : VisualLineElement
	{
		VisualLine parentVisualLine;
		
		/// <summary>
		/// Gets the parent visual line.
		/// </summary>
		public VisualLine ParentVisualLine {
			get { return parentVisualLine; }
		}
		
		/// <summary>
		/// Creates a visual line text element with the specified length.
		/// It uses the <see cref="ITextRunConstructionContext.VisualLine"/> and its
		/// <see cref="VisualLineElement.RelativeTextOffset"/> to find the actual text string.
		/// </summary>
		public VisualLineText(VisualLine parentVisualLine, int length) : base(length, length)
		{
			if (parentVisualLine == null)
				throw new ArgumentNullException("parentVisualLine");
			this.parentVisualLine = parentVisualLine;
		}
		
		/// <summary>
		/// Override this method to control the type of new VisualLineText instances when
		/// the visual line is split due to syntax highlighting.
		/// </summary>
		protected virtual VisualLineText CreateInstance(int length)
		{
			return new VisualLineText(parentVisualLine, length);
		}
		
		/// <inheritdoc/>
		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			
			int relativeOffset = startVisualColumn - VisualColumn;
			string text = context.Document.GetText(context.VisualLine.FirstDocumentLine.Offset + RelativeTextOffset + relativeOffset, DocumentLength - relativeOffset);
			return new TextCharacters(text, 0, text.Length, this.TextRunProperties);
		}
		
		/// <inheritdoc/>
		public override bool CanSplit {
			get { return true; }
		}
		
		/// <inheritdoc/>
		public override void Split(int splitVisualColumn, IList<VisualLineElement> elements, int elementIndex)
		{
			if (splitVisualColumn <= VisualColumn || splitVisualColumn >= VisualColumn + VisualLength)
				throw new ArgumentOutOfRangeException("splitVisualColumn", splitVisualColumn, "Value must be between " + (VisualColumn + 1) + " and " + (VisualColumn + VisualLength - 1));
			if (elements == null)
				throw new ArgumentNullException("elements");
			if (elements[elementIndex] != this)
				throw new ArgumentException("Invalid elementIndex - couldn't find this element at the index");
			int relativeSplitPos = splitVisualColumn - VisualColumn;
			VisualLineText splitPart = CreateInstance(DocumentLength - relativeSplitPos);
			SplitHelper(this, splitPart, splitVisualColumn, relativeSplitPos + RelativeTextOffset);
			elements.Insert(elementIndex + 1, splitPart);
		}
		
		/// <inheritdoc/>
		public override int GetRelativeOffset(int visualColumn)
		{
			return this.RelativeTextOffset + visualColumn - this.VisualColumn;
		}
		
		/// <inheritdoc/>
		public override int GetVisualColumn(int relativeTextOffset)
		{
			return VisualColumn + relativeTextOffset - this.RelativeTextOffset;
		}
		
		/// <inheritdoc/>
		public override int GetNextCaretPosition(int visualColumn, bool backwards, CaretPositioningMode mode)
		{
			int nextPos = backwards ? visualColumn - 1 : visualColumn + 1;
			if (nextPos >= this.VisualColumn && nextPos <= this.VisualColumn + this.VisualLength) {
				if (mode == CaretPositioningMode.WordBorder || mode == CaretPositioningMode.WordStart) {
					TextDocument document = parentVisualLine.FirstDocumentLine.Document;
					int textOffset = parentVisualLine.FirstDocumentLine.Offset + GetRelativeOffset(nextPos);
					if (textOffset > 0 && textOffset < document.TextLength) {
						CharClass charBefore = GetCharClass(document.GetCharAt(textOffset - 1));
						CharClass charAfter = GetCharClass(document.GetCharAt(textOffset));
						if (charBefore == charAfter || (charAfter == CharClass.Whitespace && mode == CaretPositioningMode.WordStart))
							return GetNextCaretPosition(nextPos, backwards, mode);
					}
				}
				return nextPos;
			}
			return -1;
		}
		
		enum CharClass
		{
			Whitespace,
			IdentifierPart,
			LineTerminator,
			Other
		}
		
		static CharClass GetCharClass(char c)
		{
			if (c == '\r' || c == '\n')
				return CharClass.LineTerminator;	
			else if (char.IsWhiteSpace(c))
				return CharClass.Whitespace;
			else if (char.IsLetterOrDigit(c) || c == '_')
				return CharClass.IdentifierPart;
			else
				return CharClass.Other;
		}
	}
}

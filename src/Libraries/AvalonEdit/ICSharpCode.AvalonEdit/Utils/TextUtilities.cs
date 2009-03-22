// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;
using System.ComponentModel;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// Static helper methods for working with text.
	/// </summary>
	public static class TextUtilities
	{
		/// <summary>
		/// Gets whether the character is whitespace, part of an identifier, or line terminator.
		/// </summary>
		public static CharacterClass GetCharacterClass(char c)
		{
			if (c == '\r' || c == '\n')
				return CharacterClass.LineTerminator;
			else if (char.IsWhiteSpace(c))
				return CharacterClass.Whitespace;
			else if (char.IsLetterOrDigit(c) || c == '_')
				return CharacterClass.IdentifierPart;
			else
				return CharacterClass.Other;
		}
		
		/// <summary>
		/// Gets the next caret position.
		/// </summary>
		/// <param name="textSource">The text source.</param>
		/// <param name="offset">The start offset inside the text source.</param>
		/// <param name="backwards">True to look backwards, false to look forwards.</param>
		/// <param name="mode">The mode for caret positioning.</param>
		/// <returns>The offset of the next caret position, or -1 if there is no further caret position
		/// in the text source.</returns>
		public static int GetNextCaretPosition(ITextSource textSource, int offset, bool backwards, CaretPositioningMode mode)
		{
			if (textSource == null)
				throw new ArgumentNullException("textSource");
			if (mode != CaretPositioningMode.Normal
			    && mode != CaretPositioningMode.WordBorder
			    && mode != CaretPositioningMode.WordStart)
			{
				throw new ArgumentException("Unsupported CaretPositioningMode: " + mode, "mode");
			}
			int textLength = textSource.TextLength;
			if (textLength <= 0) {
				// empty document? has a normal caret position at 0, though no word borders
				if (mode == CaretPositioningMode.Normal) {
					if (offset > 0 && backwards) return 0;
					if (offset < 0 && !backwards) return 0;
				}
				return -1;
			}
			while (true) {
				int nextPos = backwards ? offset - 1 : offset + 1;
				
				// return -1 if there is no further caret position in the text source
				// we also need this to handle offset values outside the valid range
				if (nextPos < 0 || nextPos > textLength)
					return -1;
				
				// stop at every caret position? we can stop immediately.
				if (mode == CaretPositioningMode.Normal)
					return nextPos;
				// not normal mode? we're looking for word borders...
				
				// check if we've run against the textSource borders.
				// a 'textSource' usually isn't the whole document, but a single VisualLineElement.
				if (nextPos == 0) {
					// at the document start, there's only a word border
					// if the first character is not whitespace
					if (!char.IsWhiteSpace(textSource.GetCharAt(0)))
						return nextPos;
				} else if (nextPos == textLength) {
					// at the document end, there's never a word start
					if (mode != CaretPositioningMode.WordStart) {
						// at the document end, there's only a word border
						// if the last character is not whitespace
						if (!char.IsWhiteSpace(textSource.GetCharAt(textLength - 1)))
							return nextPos;
					}
				} else {
					CharacterClass charBefore = GetCharacterClass(textSource.GetCharAt(nextPos - 1));
					CharacterClass charAfter = GetCharacterClass(textSource.GetCharAt(nextPos));
					if (charBefore != charAfter) {
						// this looks like a possible border
						
						// if we're looking for word starts, check that this is a word start (and not a word end)
						// if we're just checking for word borders, accept unconditionally
						if (!(mode == CaretPositioningMode.WordStart && (charAfter == CharacterClass.Whitespace || charAfter == CharacterClass.LineTerminator))) {
							return nextPos;
						}
					}
				}
				// we'll have to continue searching...
				offset = nextPos;
			}
		}
	}
	
	/// <summary>
	/// Classifies a character as whitespace, line terminator, part of an identifier, or other.
	/// </summary>
	public enum CharacterClass
	{
		/// <summary>
		/// The character is not whitespace, line terminator or part of an identifier.
		/// </summary>
		Other,
		/// <summary>
		/// The character is whitespace (but not line terminator).
		/// </summary>
		Whitespace,
		/// <summary>
		/// The character can be part of an identifier (LetterOrDigit or underscore).
		/// </summary>
		IdentifierPart,
		/// <summary>
		/// The character is line terminator (\r or \n).
		/// </summary>
		LineTerminator
	}
}

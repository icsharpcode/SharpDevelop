// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Interface for read-only access to a text source.
	/// </summary>
	/// <seealso cref="TextDocument"/>
	/// <seealso cref="StringTextSource"/>
	public interface ITextSource
	{
		/// <summary>
		/// Gets the whole text as string.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		string Text { get; }
		
		/// <summary>
		/// Is raised when the Text property changes.
		/// </summary>
		event EventHandler TextChanged;
		
		/// <summary>
		/// Gets the total text length.
		/// </summary>
		/// <returns>The length of the text, in characters.</returns>
		/// <remarks>This is the same as Text.Length, but is usually
		/// more efficient because it doesn't require creating a String object.</remarks>
		int TextLength { get; }
		
		/// <summary>
		/// Gets a character at the specified position in the document.
		/// </summary>
		/// <paramref name="offset">The index of the character to get.</paramref>
		/// <exception cref="ArgumentOutOfRangeException">Offset is outside the valid range (0 to TextLength-1).</exception>
		/// <returns>The character at the specified position.</returns>
		/// <remarks>This is the same as Text[offset], but is usually
		/// more efficient because it doesn't require creating a String object.</remarks>
		char GetCharAt(int offset);
		
		/// <summary>
		/// Retrieves the text for a portion of the document.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">offset or length is outside the valid range.</exception>
		/// <remarks>This is the same as Text.Substring, but is usually
		/// more efficient because it doesn't require creating a String object for the whole document.</remarks>
		string GetText(int offset, int length);
	}
	
	/// <summary>
	/// Implements the ITextSource interface using a string.
	/// </summary>
	public sealed class StringTextSource : ITextSource
	{
		readonly string text;
		
		/// <summary>
		/// Creates a new StringTextSource.
		/// </summary>
		public StringTextSource(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			this.text = text;
		}
		
		// Text can never change
		event EventHandler ITextSource.TextChanged { add {} remove {} }
		
		/// <inheritdoc/>
		public string Text {
			get { return text; }
		}
		
		/// <inheritdoc/>
		public int TextLength {
			get { return text.Length; }
		}
		
		/// <inheritdoc/>
		public char GetCharAt(int offset)
		{
			// GetCharAt must throw ArgumentOutOfRangeException, not IndexOutOfRangeException
			if (offset < 0 || offset >= text.Length)
				throw new ArgumentOutOfRangeException("offset", offset, "offset must be between 0 and " + (text.Length - 1));
			return text[offset];
		}
		
		/// <inheritdoc/>
		public string GetText(int offset, int length)
		{
			return text.Substring(offset, length);
		}
	}
}

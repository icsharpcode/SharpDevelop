// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

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
		/// <remarks>This is the same as Text.Length, but is more efficient because
		///  it doesn't require creating a String object.</remarks>
		int TextLength { get; }
		
		/// <summary>
		/// Gets a character at the specified position in the document.
		/// </summary>
		/// <paramref name="offset">The index of the character to get.</paramref>
		/// <exception cref="ArgumentOutOfRangeException">Offset is outside the valid range (0 to TextLength-1).</exception>
		/// <returns>The character at the specified position.</returns>
		/// <remarks>This is the same as Text[offset], but is more efficient because
		/// it doesn't require creating a String object.</remarks>
		char GetCharAt(int offset);
		
		/// <summary>
		/// Retrieves the text for a portion of the document.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">offset or length is outside the valid range.</exception>
		/// <remarks>This is the same as Text.Substring, but is more efficient because
		///  it doesn't require creating a String object for the whole document.</remarks>
		string GetText(int offset, int length);
		
		/// <summary>
		/// Creates a snapshot of the current text.
		/// The snapshot text source will be immutable and thread-safe.
		/// </summary>
		ITextSource CreateSnapshot();
		
		/// <summary>
		/// Creates a text reader.
		/// If the text is changed while a reader is active, the reader will continue to read from the old text version.
		/// </summary>
		TextReader CreateReader(int offset, int length);
	}
	
	/// <summary>
	/// Implements the ITextSource interface by wrapping another TextSource
	/// and viewing only a part of the text.
	/// </summary>
	public sealed class TextSourceView : ITextSource
	{
		readonly ITextSource baseTextSource;
		readonly ISegment viewedSegment;
		
		/// <summary>
		/// Creates a new TextSourceView object.
		/// </summary>
		/// <param name="baseTextSource">The base text source.</param>
		/// <param name="viewedSegment">A text segment from the base text source</param>
		public TextSourceView(ITextSource baseTextSource, ISegment viewedSegment)
		{
			if (baseTextSource == null)
				throw new ArgumentNullException("baseTextSource");
			if (viewedSegment == null)
				throw new ArgumentNullException("viewedSegment");
			this.baseTextSource = baseTextSource;
			this.viewedSegment = viewedSegment;
		}
		
		/// <inheritdoc/>
		public event EventHandler TextChanged {
			add { baseTextSource.TextChanged += value; }
			remove { baseTextSource.TextChanged -= value; }
		}
		
		/// <inheritdoc/>
		public string Text {
			get {
				return baseTextSource.GetText(viewedSegment.Offset, viewedSegment.Length);
			}
		}
		
		/// <inheritdoc/>
		public int TextLength {
			get { return viewedSegment.Length; }
		}
		
		/// <inheritdoc/>
		public char GetCharAt(int offset)
		{
			return baseTextSource.GetCharAt(viewedSegment.Offset + offset);
		}
		
		/// <inheritdoc/>
		public string GetText(int offset, int length)
		{
			return baseTextSource.GetText(viewedSegment.Offset + offset, length);
		}
		
		/// <inheritdoc/>
		public ITextSource CreateSnapshot()
		{
			return new TextSourceView(baseTextSource.CreateSnapshot(), new SimpleSegment(viewedSegment.Offset, viewedSegment.Length));
		}
		
		/// <inheritdoc/>
		public TextReader CreateReader(int offset, int length)
		{
			return baseTextSource.CreateReader(viewedSegment.Offset + offset, length);
		}
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
		
		/// <inheritdoc/>
		public TextReader CreateReader(int offset, int length)
		{
			return new StringReader(text.Substring(offset, length));
		}
		
		/// <inheritdoc/>
		public ITextSource CreateSnapshot()
		{
			return this; // StringTextSource already is immutable
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 13.06.2009
 * Time: 16:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

// not in Editor namespace because it's also used for the IParser interface etc.
namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A read-only view on a (potentially mutable) text buffer.
	/// The IDocument interfaces derives from this interface.
	/// </summary>
	public interface ITextBuffer
	{
		/// <summary>
		/// Creates an immutable snapshot of this text buffer.
		/// Unlike all other methods in this interface, this method is thread-safe.
		/// </summary>
		ITextBuffer CreateSnapshot();
		
		/// <summary>
		/// Creates an immutable snapshot of a part of this text buffer.
		/// Unlike all other methods in this interface, this method is thread-safe.
		/// </summary>
		ITextBuffer CreateSnapshot(int offset, int length);
		
		/// <summary>
		/// Creates a new TextReader to read from this text buffer.
		/// </summary>
		TextReader CreateReader();
		
		/// <summary>
		/// Gets the total text length.
		/// </summary>
		/// <returns>The length of the text, in characters.</returns>
		/// <remarks>This is the same as Text.Length, but is more efficient because
		///  it doesn't require creating a String object.</remarks>
		int TextLength { get; }
		
		/// <summary>
		/// Gets the whole text as string.
		/// </summary>
		string Text { get; }
		
		/// <summary>
		/// Is raised when the Text property changes.
		/// </summary>
		event EventHandler TextChanged;
		
		/// <summary>
		/// Gets a character at the specified position in the document.
		/// </summary>
		/// <paramref name="offset">The index of the character to get.</paramref>
		/// <exception cref="ArgumentOutOfRangeException">Offset is outside the valid range (0 to TextLength-1).</exception>
		/// <returns>The character at the specified position.</returns>
		/// <remarks>This is the same as Text[offset], but is more efficient because
		///  it doesn't require creating a String object.</remarks>
		char GetCharAt(int offset);
		
		/// <summary>
		/// Retrieves the text for a portion of the document.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">offset or length is outside the valid range.</exception>
		/// <remarks>This is the same as Text.Substring, but is more efficient because
		///  it doesn't require creating a String object for the whole document.</remarks>
		string GetText(int offset, int length);
	}
	
	public class StringTextBuffer : Editor.AvalonEdit.AvalonEditTextSourceAdapter
	{
		public StringTextBuffer(string text)
			: base(new AvalonEdit.Document.StringTextSource(text))
		{
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpDevelop.Editor;

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
		/// Gets a version identifier for this text buffer.
		/// Returns null for unversioned text buffers.
		/// </summary>
		ITextBufferVersion Version { get; }
		
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
		/// Creates a new TextReader to read from this text buffer.
		/// </summary>
		TextReader CreateReader(int offset, int length);
		
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
	
	/// <summary>
	/// Represents a version identifier for a text buffer.
	/// </summary>
	/// <remarks>
	/// This is SharpDevelop's equivalent to AvalonEdit ChangeTrackingCheckpoint.
	/// It is used by the ParserService to efficiently detect whether a document has changed and needs reparsing.
	/// It is a separate class from ITextBuffer to allow the GC to collect the text buffer while the version checkpoint
	/// is still in use.
	/// </remarks>
	public interface ITextBufferVersion
	{
		/// <summary>
		/// Gets whether this checkpoint belongs to the same document as the other checkpoint.
		/// </summary>
		bool BelongsToSameDocumentAs(ITextBufferVersion other);
		
		/// <summary>
		/// Compares the age of this checkpoint to the other checkpoint.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this version.</exception>
		/// <returns>-1 if this version is older than <paramref name="other"/>.
		/// 0 if <c>this</c> version instance represents the same version as <paramref name="other"/>.
		/// 1 if this version is newer than <paramref name="other"/>.</returns>
		int CompareAge(ITextBufferVersion other);
		
		/// <summary>
		/// Gets the changes from this checkpoint to the other checkpoint.
		/// If 'other' is older than this checkpoint, reverse changes are calculated.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this checkpoint.</exception>
		IEnumerable<TextChangeEventArgs> GetChangesTo(ITextBufferVersion other);
		
		/// <summary>
		/// Calculates where the offset has moved in the other buffer version.
		/// </summary>
		/// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this checkpoint.</exception>
		int MoveOffsetTo(ITextBufferVersion other, int oldOffset, AnchorMovementType movement);
	}
	
	public sealed class StringTextBuffer : Editor.AvalonEdit.AvalonEditTextSourceAdapter
	{
		public StringTextBuffer(string text)
			: base(new AvalonEdit.Document.StringTextSource(text))
		{
		}
	}
}

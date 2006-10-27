// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	/// <summary>
	/// This iterator iterates on a text buffer strategy.
	/// </summary>
	public interface ITextIterator
	{
		/// <value>
		/// The text buffer strategy
		/// </value>
		ITextBufferStrategy TextBuffer {
			get;
		}
		
		/// <value>
		/// Gets the current char this is the same as 
		/// GetCharRelative(0)
		/// </value>
		/// <exception cref="System.InvalidOperationException">
		/// If this method is called before the first MoveAhead or after 
		/// MoveAhead or after MoveAhead returns false.
		/// </exception>
		char Current {
			get;
		}
		
		/// <value>
		/// The current position=offset of the text iterator cursor
		/// </value>
		int Position {
			get;
			set;
		}
		
		/// <remarks>
		/// Gets a char relative to the current position (negative values
		/// will work too).
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">
		/// If this method is called before the first MoveAhead or after 
		/// MoveAhead or after MoveAhead returns false.
		/// </exception>
		char GetCharRelative(int offset);
		
		/// <remarks>
		/// Moves the iterator position numChars
		/// </remarks>
		bool MoveAhead(int numChars);
		
		/// <remarks>
		/// Rests the iterator
		/// </remarks>
		void Reset();
		
		/// <remarks>
		/// The find object calls the InformReplace method to inform the text iterator
		/// about the replace operation on the TextBuffer. The text iterator must update
		/// all internal offsets to the new offsets (if neccessary)
		/// </remarks>
		void InformReplace(int offset, int length, int newLength);
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using System;

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
		IDocument Document {
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

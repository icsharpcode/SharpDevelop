// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using System;

namespace ICSharpCode.SharpDevelop.Dom.Refactoring
{
	/// <summary>
	/// A document representing a source code file for refactoring.
	/// Line and column counting starts at 1.
	/// Offset counting starts at 0.
	/// </summary>
	public interface IDocument
	{
		int TextLength { get; }
		int TotalNumberOfLines { get; }
		string Text { get; set; }
		event EventHandler TextChanged;
		
		/// <summary>
		/// Gets the document line with the specified number.
		/// </summary>
		/// <param name="lineNumber">The number of the line to retrieve. The first line has number 1.</param>
		IDocumentLine GetLine(int lineNumber);
		
		/// <summary>
		/// Gets the document line that contains the specified offset.
		/// </summary>
		IDocumentLine GetLineForOffset(int offset);
		
		int PositionToOffset(int line, int column);
		Location OffsetToPosition(int offset);
		
		void Insert(int offset, string text);
		void Remove(int offset, int length);
		void Replace(int offset, int length, string newText);
		
		char GetCharAt(int offset);
		string GetText(int offset, int length);
		
		/// <summary>
		/// Make the document combine the following actions into a single
		/// action for undo purposes.
		/// </summary>
		void StartUndoableAction();
		
		/// <summary>
		/// Ends the undoable action started with <see cref="StartUndoableAction"/>.
		/// </summary>
		void EndUndoableAction();
		
		/// <summary>
		/// Creates an undo group. Dispose the returned value to close the undo group.
		/// </summary>
		/// <returns>An object that closes the undo group when Dispose() is called.</returns>
		IDisposable OpenUndoGroup();
	}
	
	/// <summary>
	/// A line inside a <see cref="IDocument"/>.
	/// </summary>
	public interface IDocumentLine
	{
		int Offset { get; }
		int Length { get; }
		int TotalLength { get; }
		int LineNumber { get; }
		string Text { get; }
	}
}

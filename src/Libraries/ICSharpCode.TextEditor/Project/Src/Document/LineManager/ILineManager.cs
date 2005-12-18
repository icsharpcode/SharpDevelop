// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// The line tracker keeps track of all lines in a document.
	/// </summary>
	public interface ILineManager
	{
		/// <value>
		/// A collection of all line segments
		/// </value>
		List<LineSegment> LineSegmentCollection {
			get;
		}
		
		/// <value>
		/// The total number of lines, this may be != ArrayList.Count 
		/// if the last line ends with a delimiter.
		/// </value>
		int TotalNumberOfLines {
			get;
		}
		
		/// <value>
		/// The current <see cref="IHighlightingStrategy"/> attached to this line manager
		/// </value>
		IHighlightingStrategy HighlightingStrategy {
			get;
			set;
		}
		
		/// <summary>
		/// Returns a valid line number for the given offset.
		/// </summary>
		/// <param name="offset">
		/// A offset which points to a character in the line which
		/// line number is returned.
		/// </param>
		/// <returns>
		/// An int which value is the line number.
		/// </returns>
		/// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
		int GetLineNumberForOffset(int offset);
		
		/// <summary>
		/// Returns a <see cref="LineSegment"/> for the given offset.
		/// </summary>
		/// <param name="offset">
		/// A offset which points to a character in the line which
		/// is returned.
		/// </param>
		/// <returns>
		/// A <see cref="LineSegment"/> object.
		/// </returns>
		/// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
		LineSegment GetLineSegmentForOffset(int offset);
		
		/// <summary>
		/// Returns a <see cref="LineSegment"/> for the given line number.
		/// This function should be used to get a line instead of getting the
		/// line using the <see cref="ArrayList"/>.
		/// </summary>
		/// <param name="lineNumber">
		/// The line number which is requested.
		/// </param>
		/// <returns>
		/// A <see cref="LineSegment"/> object.
		/// </returns>
		/// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
		LineSegment GetLineSegment(int lineNumber);
		
		/// <summary>
		/// Used internally, do not call yourself.
		/// </summary>
		void Insert(int offset, string text);
		
		/// <summary>
		/// Used internally, do not call yourself.
		/// </summary>
		void Remove(int offset, int length);
		
		/// <summary>
		/// Used internally, do not call yourself.
		/// </summary>
		void Replace(int offset, int length, string text);
		
		/// <summary>
		/// Sets the content of this line manager = break the text
		/// into lines.
		/// </summary>
		void SetContent(string text);
		
		/// <summary>
		/// Get the first logical line for a given visible line.
		/// example : lineNumber == 100 foldings are in the linetracker
		/// between 0..1 (2 folded, invisible lines) this method returns 102
		/// the 'logical' line number
		/// </summary>
		int GetFirstLogicalLine(int lineNumber);
		
		/// <summary>
		/// Get the last logical line for a given visible line.
		/// example : lineNumber == 100 foldings are in the linetracker
		/// between 0..1 (2 folded, invisible lines) this method returns 102
		/// the 'logical' line number
		/// </summary>
		int GetLastLogicalLine(int lineNumber);
		
		/// <summary>
		/// Get the visible line for a given logical line.
		/// example : lineNumber == 100 foldings are in the linetracker
		/// between 0..1 (2 folded, invisible lines) this method returns 98
		/// the 'visible' line number
		/// </summary>
		int GetVisibleLine(int lineNumber);
		
//		/// <summary>
//		/// Get the visible column for a given logical line and logical column.
//		/// </summary>
//		int GetVisibleColumn(int logicalLine, int logicalColumn);
		
		/// <summary>
		/// Get the next visible line after lineNumber
		/// </summary>
		int GetNextVisibleLineAbove(int lineNumber, int lineCount);
		
		/// <summary>
		/// Get the next visible line below lineNumber
		/// </summary>
		int GetNextVisibleLineBelow(int lineNumber, int lineCount);
		
		/// <summary>
		/// Is fired when lines are inserted or removed
		/// </summary>
		event LineManagerEventHandler LineCountChanged;
		
		event LineLengthEventHandler LineLengthChanged;
	}
}

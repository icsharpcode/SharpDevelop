// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Helper class that implements the Text Editor library's IDocument interface.
	/// </summary>
	public class MockDocument : IDocument
	{
		List<LineSegment> lineSegments = new List<LineSegment>();
		
		public MockDocument()
		{
		}
		
		public event EventHandler UpdateCommited;
		
		public event DocumentEventHandler DocumentAboutToBeChanged;
		
		public event DocumentEventHandler DocumentChanged;
		
		public event EventHandler TextContentChanged;
		
		public void AddLines(string code)
		{
			int offset = 0;
			string[] lines = code.Split('\n');
			foreach (string line in lines) {
				int delimiterLength = 1;
				if (line.Length > 0 && line[line.Length - 1] == '\r') {
					delimiterLength = 2;
				}
				LineSegment lineSegment = new LineSegment(offset, offset + line.Length, delimiterLength);
				lineSegments.Add(lineSegment);
				offset += line.Length + lineSegment.DelimiterLength - 1;
			}
		}
		
		public ITextEditorProperties TextEditorProperties {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.TextEditor.Undo.UndoStack UndoStack {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool ReadOnly {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IFormattingStrategy FormattingStrategy {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public ITextBufferStrategy TextBufferStrategy {
			get {
				throw new NotImplementedException();
			}
		}
		
		public FoldingManager FoldingManager {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IHighlightingStrategy HighlightingStrategy {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public BookmarkManager BookmarkManager {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICustomLineManager CustomLineManager {
			get {
				throw new NotImplementedException();
			}
		}
		
		public MarkerStrategy MarkerStrategy {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.List<LineSegment> LineSegmentCollection {
			get {
				return lineSegments;
			}
		}
		
		public int TotalNumberOfLines {
			get {
				if (lineSegments.Count == 0) {
					return 1;
				}
			
				return ((LineSegment)lineSegments[lineSegments.Count - 1]).DelimiterLength > 0 ? lineSegments.Count + 1 : lineSegments.Count;
			}
		}
		
		public string TextContent {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public int TextLength {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.List<ICSharpCode.TextEditor.TextAreaUpdate> UpdateQueue {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void UpdateSegmentListOnDocumentChange<T>(System.Collections.Generic.List<T> list, DocumentEventArgs e) where T : ISegment
		{
			throw new NotImplementedException();
		}
		
		public int GetLineNumberForOffset(int offset)
		{
			throw new NotImplementedException();
		}
		
		public LineSegment GetLineSegmentForOffset(int offset)
		{
			throw new NotImplementedException();
		}
		
		public LineSegment GetLineSegment(int lineNumber)
		{
			return lineSegments[lineNumber];
		}
		
		public int GetFirstLogicalLine(int lineNumber)
		{
			throw new NotImplementedException();
		}
		
		public int GetLastLogicalLine(int lineNumber)
		{
			throw new NotImplementedException();
		}
		
		public int GetVisibleLine(int lineNumber)
		{
			throw new NotImplementedException();
		}
		
		public int GetNextVisibleLineAbove(int lineNumber, int lineCount)
		{
			throw new NotImplementedException();
		}
		
		public int GetNextVisibleLineBelow(int lineNumber, int lineCount)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, string text)
		{
			throw new NotImplementedException();
		}
		
		public void Remove(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public void Replace(int offset, int length, string text)
		{
			throw new NotImplementedException();
		}
		
		public char GetCharAt(int offset)
		{
			throw new NotImplementedException();
		}
		
		public string GetText(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public string GetText(ISegment segment)
		{
			throw new NotImplementedException();
		}
		
		public System.Drawing.Point OffsetToPosition(int offset)
		{
			throw new NotImplementedException();
		}
		
		public int PositionToOffset(System.Drawing.Point p)
		{
			throw new NotImplementedException();
		}
		
		public void RequestUpdate(ICSharpCode.TextEditor.TextAreaUpdate update)
		{
			throw new NotImplementedException();
		}
		
		public void CommitUpdate()
		{
			throw new NotImplementedException();
		}
		
		void OnUpdateCommited()
		{
			if (UpdateCommited != null) {
			}
		}
		
		void OnDocumentAboutToBeChanged()
		{
			if (DocumentAboutToBeChanged != null) {
			}
		}

		void OnDocumentChanged()
		{
			if (DocumentChanged != null) {
			}
		}
		
		void OnTextContentChanged()
		{
			if (TextContentChanged != null) {
			}
		}		
	}
}

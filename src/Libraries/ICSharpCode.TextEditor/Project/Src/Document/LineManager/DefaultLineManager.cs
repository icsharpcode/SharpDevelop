// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.TextEditor.Document
{
	internal class DefaultLineManager : ILineManager
	{
		LineSegmentTree lineCollection = new LineSegmentTree();
		
		IDocument document;
		IHighlightingStrategy highlightingStrategy;
		
		public IList<LineSegment> LineSegmentCollection {
			get {
				return lineCollection;
			}
		}
		
		public int TotalNumberOfLines {
			get {
				return lineCollection.Count;
			}
		}
		
		public IHighlightingStrategy HighlightingStrategy {
			get {
				return highlightingStrategy;
			}
			set {
				if (highlightingStrategy != value) {
					highlightingStrategy = value;
					if (highlightingStrategy != null) {
						highlightingStrategy.MarkTokens(document);
					}
				}
			}
		}
		
		public DefaultLineManager(IDocument document, IHighlightingStrategy highlightingStrategy)
		{
			this.document = document;
			this.highlightingStrategy = highlightingStrategy;
		}
		
		public int GetLineNumberForOffset(int offset)
		{
			return GetLineSegmentForOffset(offset).LineNumber;
		}
		
		public LineSegment GetLineSegmentForOffset(int offset)
		{
			return lineCollection.GetByOffset(offset);
		}
		
		public LineSegment GetLineSegment(int lineNr)
		{
			return lineCollection[lineNr];
		}
		
		public void Insert(int offset, string text)
		{
			Replace(offset, 0, text);
		}
		
		public void Remove(int offset, int length)
		{
			Replace(offset, length, String.Empty);
		}
		
		public void Replace(int offset, int length, string text)
		{
//			Console.WriteLine("Replace offset="+offset+" length="+length+" text.Length="+text.Length);
			int lineStart = GetLineNumberForOffset(offset);
			int oldNumberOfLines = this.TotalNumberOfLines;
			RemoveInternal(offset, length);
			int numberOfLinesAfterRemoving = this.TotalNumberOfLines;
			if (!string.IsNullOrEmpty(text)) {
				InsertInternal(offset, text);
			}
//			#if DEBUG
//			Console.WriteLine("New line collection:");
//			Console.WriteLine(lineCollection.GetTreeAsString());
//			Console.WriteLine("New text:");
//			Console.WriteLine("'" + document.TextContent + "'");
//			#endif
			RunHighlighter(lineStart, 1 + Math.Max(0, this.TotalNumberOfLines - numberOfLinesAfterRemoving));
			if (this.TotalNumberOfLines != oldNumberOfLines) {
				OnLineCountChanged(new LineManagerEventArgs(document, lineStart, this.TotalNumberOfLines - oldNumberOfLines));
			}
		}
		
		void RemoveInternal(int offset, int length)
		{
			Debug.Assert(length >= 0);
			if (length == 0) return;
			LineSegmentTree.Enumerator it = lineCollection.GetEnumeratorForOffset(offset);
			LineSegment startSegment = it.Current;
			int startSegmentOffset = startSegment.Offset;
			if (offset + length < startSegmentOffset + startSegment.TotalLength) {
				// just removing a part of this line segment
				SetSegmentLength(startSegment, startSegment.TotalLength - length);
				return;
			}
			// merge startSegment with another line segment because startSegment's delimiter was deleted
			// possibly remove lines in between if multiple delimiters were deleted
			int charactersRemovedInStartLine = startSegmentOffset + startSegment.TotalLength - offset;
			Debug.Assert(charactersRemovedInStartLine > 0);
			
			
			LineSegment endSegment = lineCollection.GetByOffset(offset + length);
			if (endSegment == startSegment) {
				// special case: we are removing a part of the last line up to the
				// end of the document
				SetSegmentLength(startSegment, startSegment.TotalLength - length);
				return;
			}
			int endSegmentOffset = endSegment.Offset;
			int charactersLeftInEndLine = endSegmentOffset + endSegment.TotalLength - (offset + length);
			SetSegmentLength(startSegment, startSegment.TotalLength - charactersRemovedInStartLine + charactersLeftInEndLine);
			startSegment.DelimiterLength = endSegment.DelimiterLength;
			// remove all segments between startSegment (excl.) and endSegment (incl.)
			it.MoveNext();
			LineSegment segmentToRemove;
			do {
				segmentToRemove = it.Current;
				it.MoveNext();
				lineCollection.RemoveSegment(segmentToRemove);
			} while (segmentToRemove != endSegment);
		}
		
		void InsertInternal(int offset, string text)
		{
			LineSegment segment = lineCollection.GetByOffset(offset);
			int lastDelimiterEnd = 0;
			DelimiterSegment ds;
			while ((ds = NextDelimiter(text, lastDelimiterEnd)) != null) {
				// split line segment at line delimiter
				int lineBreakOffset = offset + ds.Offset + ds.Length;
				int lengthAfterInsertionPos = segment.Offset + segment.TotalLength - (offset + lastDelimiterEnd);
				lineCollection.SetSegmentLength(segment, lineBreakOffset - segment.Offset);
				LineSegment newSegment = lineCollection.InsertSegmentAfter(segment, lengthAfterInsertionPos);
				segment.DelimiterLength = ds.Length;
				
				segment = newSegment;
				lastDelimiterEnd = ds.Offset + ds.Length;
			}
			// insert rest after last delimiter
			if (lastDelimiterEnd != text.Length) {
				SetSegmentLength(segment, segment.TotalLength + text.Length - lastDelimiterEnd);
			}
		}
		
		void SetSegmentLength(LineSegment segment, int newTotalLength)
		{
			int delta = newTotalLength - segment.TotalLength;
			lineCollection.SetSegmentLength(segment, newTotalLength);
			OnLineLengthChanged(new LineLengthEventArgs(document, segment, delta));
		}
		
		void RunHighlighter(int firstLine, int lineCount)
		{
			if (highlightingStrategy != null) {
				List<LineSegment> markLines = new List<LineSegment>();
				LineSegmentTree.Enumerator it = lineCollection.GetEnumeratorForIndex(firstLine);
				for (int i = 0; i < lineCount && it.IsValid; i++) {
					markLines.Add(it.Current);
					it.MoveNext();
				}
				highlightingStrategy.MarkTokens(document, markLines);
			}
		}
		
		public void SetContent(string text)
		{
			lineCollection.Clear();
			if (text != null) {
				Replace(0, 0, text);
			}
		}
		
		
		
		public int GetVisibleLine(int logicalLineNumber)
		{
			if (!document.TextEditorProperties.EnableFolding) {
				return logicalLineNumber;
			}
			
			int visibleLine = 0;
			int foldEnd = 0;
			List<FoldMarker> foldings = document.FoldingManager.GetTopLevelFoldedFoldings();
			foreach (FoldMarker fm in foldings) {
				if (fm.StartLine >= logicalLineNumber) {
					break;
				}
				if (fm.StartLine >= foldEnd) {
					visibleLine += fm.StartLine - foldEnd;
					if (fm.EndLine > logicalLineNumber) {
						return visibleLine;
					}
					foldEnd = fm.EndLine;
				}
			}
//			Debug.Assert(logicalLineNumber >= foldEnd);
			visibleLine += logicalLineNumber - foldEnd;
			return visibleLine;
		}
		
		public int GetFirstLogicalLine(int visibleLineNumber)
		{
			if (!document.TextEditorProperties.EnableFolding) {
				return visibleLineNumber;
			}
			int v = 0;
			int foldEnd = 0;
			List<FoldMarker> foldings = document.FoldingManager.GetTopLevelFoldedFoldings();
			foreach (FoldMarker fm in foldings) {
				if (fm.StartLine >= foldEnd) {
					if (v + fm.StartLine - foldEnd >= visibleLineNumber) {
						break;
					}
					v += fm.StartLine - foldEnd;
					foldEnd = fm.EndLine;
				}
			}
			// help GC
			foldings.Clear();
			foldings = null;
			return foldEnd + visibleLineNumber - v;
		}
		
		public int GetLastLogicalLine(int visibleLineNumber)
		{
			if (!document.TextEditorProperties.EnableFolding) {
				return visibleLineNumber;
			}
			return GetFirstLogicalLine(visibleLineNumber + 1) - 1;
		}
		
		// TODO : speedup the next/prev visible line search
		// HOW? : save the foldings in a sorted list and lookup the
		//        line numbers in this list
		public int GetNextVisibleLineAbove(int lineNumber, int lineCount)
		{
			int curLineNumber = lineNumber;
			if (document.TextEditorProperties.EnableFolding) {
				for (int i = 0; i < lineCount && curLineNumber < TotalNumberOfLines; ++i) {
					++curLineNumber;
					while (curLineNumber < TotalNumberOfLines && (curLineNumber >= lineCollection.Count || !document.FoldingManager.IsLineVisible(curLineNumber))) {
						++curLineNumber;
					}
				}
			} else {
				curLineNumber += lineCount;
			}
			return Math.Min(TotalNumberOfLines - 1, curLineNumber);
		}
		
		public int GetNextVisibleLineBelow(int lineNumber, int lineCount)
		{
			int curLineNumber = lineNumber;
			if (document.TextEditorProperties.EnableFolding) {
				for (int i = 0; i < lineCount; ++i) {
					--curLineNumber;
					while (curLineNumber >= 0 && !document.FoldingManager.IsLineVisible(curLineNumber)) {
						--curLineNumber;
					}
				}
			} else {
				curLineNumber -= lineCount;
			}
			return Math.Max(0, curLineNumber);
		}
		
		protected virtual void OnLineCountChanged(LineManagerEventArgs e)
		{
			if (LineCountChanged != null) {
				LineCountChanged(this, e);
			}
		}
		
		// use always the same ISegment object for the DelimiterInfo
		DelimiterSegment delimiterSegment = new DelimiterSegment();
		DelimiterSegment NextDelimiter(string text, int offset)
		{
			for (int i = offset; i < text.Length; i++) {
				switch (text[i]) {
					case '\r':
						if (i + 1 < text.Length) {
							if (text[i + 1] == '\n') {
								delimiterSegment.Offset = i;
								delimiterSegment.Length = 2;
								return delimiterSegment;
							}
						}
						goto case '\n';
					case '\n':
						delimiterSegment.Offset = i;
						delimiterSegment.Length = 1;
						return delimiterSegment;
				}
			}
			return null;
		}
		
		protected virtual void OnLineLengthChanged(LineLengthEventArgs e)
		{
			if (LineLengthChanged != null) {
				LineLengthChanged(this, e);
			}
		}
		
		public event LineLengthEventHandler LineLengthChanged;
		public event LineManagerEventHandler LineCountChanged;
		
		sealed class DelimiterSegment : ISegment
		{
			int offset;
			int length;
			
			public int Offset {
				get {
					return offset;
				}
				set {
					offset = value;
				}
			}
			
			public int Length {
				get {
					return length;
				}
				set {
					length = value;
				}
			}
		}
	}
}

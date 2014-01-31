// 
// InsertionPoint.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Refactoring
{
	public enum NewLineInsertion
	{
		None,
		Eol,
		BlankLine
	}
	
	public class InsertionPoint
	{
		public TextLocation Location {
			get;
			set;
		}
		
		public NewLineInsertion LineBefore { get; set; }
		public NewLineInsertion LineAfter { get; set; }
		
		public InsertionPoint (TextLocation location, NewLineInsertion lineBefore, NewLineInsertion lineAfter)
		{
			this.Location = location;
			this.LineBefore = lineBefore;
			this.LineAfter = lineAfter;
		}
		
		public override string ToString ()
		{
			return string.Format ("[InsertionPoint: Location={0}, LineBefore={1}, LineAfter={2}]", Location, LineBefore, LineAfter);
		}
		
		void InsertNewLine (IDocument document, NewLineInsertion insertion, ref int offset)
		{
			string eolMarker = DocumentUtilities.GetLineTerminator(document, 1);
			string str = null;
			switch (insertion) {
				case NewLineInsertion.Eol:
					str = eolMarker;
					break;
				case NewLineInsertion.BlankLine:
					str = eolMarker + eolMarker;
					break;
				default:
					return;
			}
			
			document.Insert (offset, str);
			offset += str.Length;
		}
		
		public int Insert (IDocument document, string text)
		{
			int offset = document.GetOffset (Location);
			using (var undo = document.OpenUndoGroup ()) {
				text = DocumentUtilities.NormalizeNewLines(text, document, Location.Line);
				
				var line = document.GetLineByOffset (offset);
				int insertionOffset = line.Offset + Location.Column - 1;
				offset = insertionOffset;
				InsertNewLine (document, LineBefore, ref offset);
				int result = offset - insertionOffset;
				
				document.Insert (offset, text);
				offset += text.Length;
				InsertNewLine (document, LineAfter, ref offset);
				return result;
			}
		}
		
		public static List<InsertionPoint> GetInsertionPoints (IDocument document, IUnresolvedTypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");
			
			// update type from parsed document, since this is always newer.
			//type = parsedDocument.GetInnermostTypeDefinition (type.GetLocation ()) ?? type;
			
			List<InsertionPoint> result = new List<InsertionPoint> ();
			int offset = document.GetOffset (type.Region.Begin);
			if (offset < 0)
				return result;
			while (offset < document.TextLength && document.GetCharAt (offset) != '{') {
				offset++;
			}
			
			var realStartLocation = document.GetLocation (offset);
			result.Add (GetInsertionPosition (document, realStartLocation.Line, realStartLocation.Column));
			result [0].LineBefore = NewLineInsertion.None;
			
			foreach (var member in type.Members) {
				TextLocation domLocation = member.BodyRegion.End;
				if (domLocation.Line <= 0) {
					domLocation = member.Region.End;
				}
				result.Add (GetInsertionPosition (document, domLocation.Line, domLocation.Column));
			}
			result [result.Count - 1].LineAfter = NewLineInsertion.None;
			CheckStartPoint (document, result [0], result.Count == 1);
			if (result.Count > 1) {
				result.RemoveAt (result.Count - 1);
				NewLineInsertion insertLine;
				var lineBefore = GetLineOrNull(document, type.BodyRegion.EndLine - 1);
				if (lineBefore != null && lineBefore.Length == GetLineIndent(document, lineBefore).Length) {
					insertLine = NewLineInsertion.None;
				} else {
					insertLine = NewLineInsertion.Eol;
				}
				// search for line start
				var line = document.GetLineByNumber (type.BodyRegion.EndLine);
				int col = type.BodyRegion.EndColumn - 1;
				while (col > 1 && char.IsWhiteSpace (document.GetCharAt (line.Offset + col - 2)))
					col--;
				result.Add (new InsertionPoint (new TextLocation (type.BodyRegion.EndLine, col), insertLine, NewLineInsertion.Eol));
				CheckEndPoint (document, result [result.Count - 1], result.Count == 1);
			}
			
			/*foreach (var region in parsedDocument.UserRegions.Where (r => type.BodyRegion.IsInside (r.Region.Begin))) {
				result.Add (new InsertionPoint (new DocumentLocation (region.Region.BeginLine + 1, 1), NewLineInsertion.Eol, NewLineInsertion.Eol));
				result.Add (new InsertionPoint (new DocumentLocation (region.Region.EndLine, 1), NewLineInsertion.Eol, NewLineInsertion.Eol));
				result.Add (new InsertionPoint (new DocumentLocation (region.Region.EndLine + 1, 1), NewLineInsertion.Eol, NewLineInsertion.Eol));
			}*/
			result.Sort ((left, right) => left.Location.CompareTo (right.Location));
			return result;
		}
		
		static void CheckEndPoint (IDocument doc, InsertionPoint point, bool isStartPoint)
		{
			var line = GetLineOrNull(doc, point.Location.Line);
			if (line == null)
				return;
			
			if (GetLineIndent (doc, line).Length + 1 < point.Location.Column)
				point.LineBefore = NewLineInsertion.BlankLine;
			if (point.Location.Column < line.Length + 1)
				point.LineAfter = NewLineInsertion.Eol;
		}
		
		static void CheckStartPoint (IDocument doc, InsertionPoint point, bool isEndPoint)
		{
			var line = GetLineOrNull(doc, point.Location.Line);
			if (line == null)
				return;
			if (GetLineIndent (doc, line).Length + 1 == point.Location.Column) {
				int lineNr = point.Location.Line;
				while (lineNr > 1 && GetLineIndent(doc, lineNr - 1).Length == doc.GetLineByNumber (lineNr - 1).Length) {
					lineNr--;
				}
				line = GetLineOrNull(doc, lineNr);
				point.Location = new TextLocation (lineNr, GetLineIndent (doc, line).Length + 1);
			}
			
			if (GetLineIndent (doc, line).Length + 1 < point.Location.Column)
				point.LineBefore = NewLineInsertion.Eol;
			if (point.Location.Column < line.Length + 1)
				point.LineAfter = isEndPoint ? NewLineInsertion.Eol : NewLineInsertion.BlankLine;
		}
		
		static string GetLineIndent(IDocument doc, int lineNumber)
		{
			return GetLineIndent(doc, GetLineOrNull(doc, lineNumber));
		}
		
		static string GetLineIndent(IDocument doc, IDocumentLine line)
		{
			if (line == null)
				return string.Empty;
			else
				return DocumentUtilities.GetWhitespaceAfter(doc, line.Offset);
		}
		
		static InsertionPoint GetInsertionPosition (IDocument doc, int line, int column)
		{
			int bodyEndOffset = doc.GetOffset (line, column) + 1;
			var curLine = GetLineOrNull(doc, line);
			if (curLine != null) {
				if (bodyEndOffset < curLine.EndOffset) {
					// case1: positition is somewhere inside the start line
					return new InsertionPoint (new TextLocation (line, column + 1), NewLineInsertion.Eol, NewLineInsertion.BlankLine);
				}
			}
			
			// -> if position is at line end check next line
			var nextLine = GetLineOrNull (doc, line + 1);
			if (nextLine == null) // check for 1 line case.
				return new InsertionPoint (new TextLocation (line, column + 1), NewLineInsertion.BlankLine, NewLineInsertion.BlankLine);
			
			for (int i = nextLine.Offset; i < nextLine.EndOffset; i++) {
				char ch = doc.GetCharAt (i);
				if (!char.IsWhiteSpace (ch)) {
					// case2: next line contains non ws chars.
					return new InsertionPoint (new TextLocation (line + 1, 1), NewLineInsertion.Eol, NewLineInsertion.BlankLine);
				}
			}
			// case3: whitespace line
			return new InsertionPoint (new TextLocation (line + 1, 1), NewLineInsertion.Eol, NewLineInsertion.None);
		}
		
		static IDocumentLine GetLineOrNull(IDocument doc, int lineNumber)
		{
			if (lineNumber >= 1 && lineNumber <= doc.LineCount)
				return doc.GetLineByNumber(lineNumber);
			else
				return null;
		}
	}
}

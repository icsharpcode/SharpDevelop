// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Wraps the AvalonEdit TextDocument to provide the IDocument interface.
	/// </summary>
	public class AvalonEditDocumentAdapter : IDocument
	{
		readonly TextDocument document;
		
		public AvalonEditDocumentAdapter(TextDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
		}
		
		sealed class LineAdapter : IDocumentLine
		{
			readonly DocumentLine line;
			
			public LineAdapter(DocumentLine line)
			{
				Debug.Assert(line != null);
				this.line = line;
			}
			
			public int Offset {
				get { return line.Offset; }
			}
			
			public int Length {
				get { return line.Length; }
			}
			
			public int TotalLength {
				get { return line.TotalLength; }
			}
			
			public int LineNumber {
				get { return line.LineNumber; }
			}
			
			public string Text {
				get { return line.Text; }
			}
		}
		
		public int TextLength {
			get { return document.TextLength; }
		}
		
		public int TotalNumberOfLines {
			get { return document.LineCount; }
		}
		
		public string Text {
			get { return document.Text; }
			set { document.Text = value; }
		}
		
		public event EventHandler TextChanged {
			add { document.TextChanged += value; }
			remove { document.TextChanged -= value; }
		}
		
		public IDocumentLine GetLine(int lineNumber)
		{
			return new LineAdapter(document.GetLineByNumber(lineNumber));
		}
		
		public IDocumentLine GetLineForOffset(int offset)
		{
			return new LineAdapter(document.GetLineByOffset(offset));
		}
		
		public int PositionToOffset(int line, int column)
		{
			return document.GetOffset(new TextLocation(line, column));
		}
		
		public Location OffsetToPosition(int offset)
		{
			return ToLocation(document.GetLocation(offset));
		}
		
		internal static Location ToLocation(TextLocation position)
		{
			return new Location(position.Column, position.Line);
		}
		
		internal static TextLocation ToPosition(Location location)
		{
			return new TextLocation(location.Line, location.Column);
		}
		
		public void Insert(int offset, string text)
		{
			document.Insert(offset, text);
		}
		
		public void Remove(int offset, int length)
		{
			document.Remove(offset, length);
		}
		
		public void Replace(int offset, int length, string newText)
		{
			document.Replace(offset, length, newText);
		}
		
		public char GetCharAt(int offset)
		{
			return document.GetCharAt(offset);
		}
		
		public string GetText(int offset, int length)
		{
			return document.GetText(offset, length);
		}
		
		public void StartUndoableAction()
		{
			document.BeginUpdate();
		}
		
		public void EndUndoableAction()
		{
			document.EndUpdate();
		}
		
		public IDisposable OpenUndoGroup()
		{
			return document.RunUpdate();
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.NRefactory.Editor
{
	/// <summary>
	/// Document based on a string builder.
	/// </summary>
	public class StringBuilderDocument : IDocument
	{
		readonly StringBuilder b;
		readonly TextSourceVersionProvider versionProvider = new TextSourceVersionProvider();
		
		public StringBuilderDocument(string text = "")
		{
			b = new StringBuilder(text);
		}
		
		public event EventHandler<TextChangeEventArgs> TextChanging;
		
		public event EventHandler<TextChangeEventArgs> TextChanged;
		
		public event EventHandler ChangeCompleted {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		
		public string Text {
			get { return b.ToString(); }
			set { Replace(0, b.Length, value); }
		}
		
		public int LineCount {
			get { return CreateDocumentSnapshot().LineCount; }
		}
		
		public ITextSourceVersion Version {
			get { return versionProvider.CurrentVersion; }
		}
		
		public int TextLength {
			get { return b.Length; }
		}
		
		ReadOnlyDocument documentSnapshot;
		
		public IDocument CreateDocumentSnapshot()
		{
			if (documentSnapshot == null)
				documentSnapshot = new ReadOnlyDocument(this);
			return documentSnapshot;
		}
		
		public IDocumentLine GetLineByNumber(int lineNumber)
		{
			return CreateDocumentSnapshot().GetLineByNumber(lineNumber);
		}
		
		public IDocumentLine GetLineByOffset(int offset)
		{
			return CreateDocumentSnapshot().GetLineByOffset(offset);
		}
		
		public int GetOffset(int line, int column)
		{
			return CreateDocumentSnapshot().GetOffset(line, column);
		}
		
		public int GetOffset(TextLocation location)
		{
			return CreateDocumentSnapshot().GetOffset(location);
		}
		
		public TextLocation GetLocation(int offset)
		{
			return CreateDocumentSnapshot().GetLocation(offset);
		}
		
		public void Insert(int offset, string text)
		{
			Replace(offset, 0, text);
		}
		
		public void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
		{
			Replace(offset, 0, text);
		}
		
		public void Remove(int offset, int length)
		{
			Replace(offset, length, string.Empty);
		}
		
		public void Replace(int offset, int length, string newText)
		{
			var change = new TextChangeEventArgs(offset, b.ToString(offset, length), newText);
			if (TextChanging != null)
				TextChanging(this, change);
			
			documentSnapshot = null;
			b.Remove(offset, length);
			b.Insert(offset, newText);
			versionProvider.AppendChange(change);
			
			if (TextChanged != null)
				TextChanged(this, change);
		}
		
		public void StartUndoableAction()
		{
		}
		
		public void EndUndoableAction()
		{
		}
		
		public IDisposable OpenUndoGroup()
		{
			return null;
		}
		
		public ITextAnchor CreateAnchor(int offset)
		{
			throw new NotImplementedException();
		}
		
		public ITextSource CreateSnapshot()
		{
			return new StringTextSource(this.Text, versionProvider.CurrentVersion);
		}
		
		public ITextSource CreateSnapshot(int offset, int length)
		{
			return new StringTextSource(GetText(offset, length));
		}
		
		public TextReader CreateReader()
		{
			return new StringReader(this.Text);
		}
		
		public TextReader CreateReader(int offset, int length)
		{
			return new StringReader(GetText(offset, length));
		}
		
		public char GetCharAt(int offset)
		{
			return b[offset];
		}
		
		public string GetText(int offset, int length)
		{
			return b.ToString(offset, length);
		}
		
		public string GetText(ISegment segment)
		{
			return b.ToString(segment.Offset, segment.Length);
		}
		
		public int IndexOf(char c, int startIndex, int count)
		{
			return this.Text.IndexOf(c, startIndex, count);
		}
		
		public int IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			return this.Text.IndexOfAny(anyOf, startIndex, count);
		}
		
		public int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
		{
			return this.Text.IndexOf(searchText, startIndex, count, comparisonType);
		}
		
		public int LastIndexOf(char c, int startIndex, int count)
		{
			return this.Text.LastIndexOf(c, startIndex, count);
		}
		
		public int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
		{
			return this.Text.LastIndexOf(searchText, startIndex, count, comparisonType);
		}
		
		public object GetService(Type serviceType)
		{
			return null;
		}
	}
}

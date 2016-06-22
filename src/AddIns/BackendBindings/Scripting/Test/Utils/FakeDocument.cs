// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeDocument : IDocument
	{
		#pragma warning disable 0067
		public event EventHandler<TextChangeEventArgs> TextChanging;
		public event EventHandler TextChanged;
		public event EventHandler ChangeCompleted;
		public event EventHandler FileNameChanged;
		#pragma warning restore 0067
		
		public FakeDocumentLine DocumentLineToReturnFromGetLine;
		public int LineNumberPassedToGetLine;
		
		public string Text { get; set; }
		
		public IDocumentLine GetLineByNumber(int lineNumber)
		{
			LineNumberPassedToGetLine = lineNumber;
			return DocumentLineToReturnFromGetLine;
		}
		
		event EventHandler<TextChangeEventArgs> IDocument.TextChanged {
			add {
				throw new NotImplementedException();
			}
			remove {
				throw new NotImplementedException();
			}
		}
		
		public int LineCount {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string FileName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ITextSourceVersion Version {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int TextLength {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IDocument CreateDocumentSnapshot()
		{
			throw new NotImplementedException();
		}
		
		public IDocumentLine GetLineByOffset(int offset)
		{
			throw new NotImplementedException();
		}
		
		public int GetOffset(int line, int column)
		{
			throw new NotImplementedException();
		}
		
		public int GetOffset(ICSharpCode.NRefactory.TextLocation location)
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.NRefactory.TextLocation GetLocation(int offset)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, string text)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, ITextSource text)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, ITextSource text, AnchorMovementType defaultAnchorMovementType)
		{
			throw new NotImplementedException();
		}
		
		public void Remove(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public void Replace(int offset, int length, string newText)
		{
			throw new NotImplementedException();
		}
		
		public void Replace(int offset, int length, ITextSource newText)
		{
			throw new NotImplementedException();
		}
		
		public void StartUndoableAction()
		{
			throw new NotImplementedException();
		}
		
		public void EndUndoableAction()
		{
			throw new NotImplementedException();
		}
		
		public IDisposable OpenUndoGroup()
		{
			throw new NotImplementedException();
		}
		
		public ITextAnchor CreateAnchor(int offset)
		{
			throw new NotImplementedException();
		}
		
		public ITextSource CreateSnapshot()
		{
			throw new NotImplementedException();
		}
		
		public ITextSource CreateSnapshot(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public TextReader CreateReader()
		{
			throw new NotImplementedException();
		}
		
		public TextReader CreateReader(int offset, int length)
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
			return ((FakeDocumentLine)segment).Text;
		}
		
		public void WriteTextTo(TextWriter writer)
		{
			throw new NotImplementedException();
		}
		
		public void WriteTextTo(TextWriter writer, int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public int IndexOf(char c, int startIndex, int count)
		{
			throw new NotImplementedException();
		}
		
		public int IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			throw new NotImplementedException();
		}
		
		public int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
		{
			throw new NotImplementedException();
		}
		
		public int LastIndexOf(char c, int startIndex, int count)
		{
			throw new NotImplementedException();
		}
		
		public int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}
}

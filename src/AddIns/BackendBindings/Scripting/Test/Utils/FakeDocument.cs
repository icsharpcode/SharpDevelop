// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeDocument : IDocument
	{		
		#pragma warning disable 0067
		public event EventHandler<TextChangeEventArgs> Changing;
		public event EventHandler<TextChangeEventArgs> Changed;		
		public event EventHandler TextChanged;
		#pragma warning restore 0067
		
		public FakeDocumentLine DocumentLineToReturnFromGetLine;
		public int LineNumberPassedToGetLine;
		
		public string Text { get; set; }
		
		public int TotalNumberOfLines {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ITextBufferVersion Version {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int TextLength {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IDocumentLine GetLine(int lineNumber)
		{
			LineNumberPassedToGetLine = lineNumber;
			return DocumentLineToReturnFromGetLine;
		}
		
		public IDocumentLine GetLineForOffset(int offset)
		{
			throw new NotImplementedException();
		}
		
		public int PositionToOffset(int line, int column)
		{
			throw new NotImplementedException();
		}
		
		public Location OffsetToPosition(int offset)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, string text)
		{
			throw new NotImplementedException();			
		}
		
		public void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
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
		
		public ITextBuffer CreateSnapshot()
		{
			throw new NotImplementedException();
		}
		
		public ITextBuffer CreateSnapshot(int offset, int length)
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
			return Text.Substring(offset, length);
		}
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}
}

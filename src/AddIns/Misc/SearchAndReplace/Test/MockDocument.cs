// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop;
using System;
using ICSharpCode.SharpDevelop.Editor;

namespace SearchAndReplace.Tests.Utils
{
	public class MockDocument : IDocument
	{
		public MockDocument()
		{
			this.Text = string.Empty;
		}
		
		public int TextLength {
			get {
				return this.Text.Length;
			}
		}
		
		public event EventHandler TextChanged {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		
		public int TotalNumberOfLines {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ITextBufferVersion Version {
			get { throw new NotImplementedException(); }
		}
		
		public string Text { get; set; }
		
		public IDocumentLine GetLine(int lineNumber)
		{
			throw new NotImplementedException();
		}
		
		public IDocumentLine GetLineForOffset(int offset)
		{
			throw new NotImplementedException();
		}
		
		public int PositionToOffset(int line, int column)
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.NRefactory.Location OffsetToPosition(int offset)
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
		
		public char GetCharAt(int offset)
		{
			return this.Text[offset];
		}
		
		public string GetText(int offset, int length)
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
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
		
		public System.IO.TextReader CreateReader()
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
		
		public event EventHandler<TextChangeEventArgs> Changing {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		
		public event EventHandler<TextChangeEventArgs> Changed {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		
		public System.IO.TextReader CreateReader(int offset, int length)
		{
			throw new NotImplementedException();
		}
	}
}

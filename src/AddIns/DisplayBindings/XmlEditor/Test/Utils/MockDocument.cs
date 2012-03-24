// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace XmlEditor.Tests.Utils
{
	public class MockDocument : IDocument
	{
		string text = String.Empty;
		TextSection textSectionUsedWithGetTextMethod;
		ITextBuffer snapshot;
		
		public List<int> PositionToOffsetReturnValues = new List<int>();
		
		MockTextEditor editor;
		
		public MockDocument(MockTextEditor editor = null)
		{
			this.editor = editor;
		}
		public event EventHandler TextChanged;
		
		protected virtual void OnTextChanged(EventArgs e)
		{
			if (TextChanged != null) {
				TextChanged(this, e);
			}
		}
		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
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
			get { return text.Length; }
		}
		
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
			int offset = PositionToOffsetReturnValues[0];
			PositionToOffsetReturnValues.RemoveAt(0);
			return offset;
		}
		
		public Location OffsetToPosition(int offset)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, string text)
		{
			this.text = this.text.Insert(offset, text);
			if (editor != null && editor.Caret.Offset == offset)
				editor.Caret.Offset += text.Length;
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
		
		public void SetSnapshot(ITextBuffer snapshot)
		{
			this.snapshot = snapshot;
		}
		
		public ITextBuffer CreateSnapshot()
		{
			return snapshot;
		}
		
		public ITextBuffer CreateSnapshot(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public TextReader CreateReader()
		{
			throw new NotImplementedException();
		}
		
		public char GetCharAt(int offset)
		{
			return text[offset];
		}
		
		public string GetText(int offset, int length)
		{
			textSectionUsedWithGetTextMethod = new TextSection(offset, length);
			return text.Substring(offset, length);
		}
		
		public TextSection GetTextSectionUsedWithGetTextMethod()
		{
			return textSectionUsedWithGetTextMethod;
		}
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
		
		public event EventHandler<TextChangeEventArgs> Changing {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		
		public event EventHandler<TextChangeEventArgs> Changed;
		
		public void RaiseChangedEvent()
		{
			TextChangeEventArgs e = new TextChangeEventArgs(0, "", "a");
			if (Changed != null) {
				Changed(this, e);
			}
		}

		public TextReader CreateReader(int offset, int length)
		{
			throw new NotImplementedException();
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class ThreadSafeDocument : IDocument
	{
		IDocument document;
		
		public ThreadSafeDocument(IDocument document)
		{
			this.document = document;
		}
		
		#pragma warning disable 0067
		public event EventHandler<TextChangeEventArgs> Changing;
		public event EventHandler<TextChangeEventArgs> Changed;
		public event EventHandler TextChanged;
		#pragma warning restore 0067
		
		public string Text {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public int TotalNumberOfLines {
			get {
				if (WorkbenchSingleton.InvokeRequired) {
					return WorkbenchSingleton.SafeThreadFunction(() => TotalNumberOfLines);
				}
				return document.TotalNumberOfLines;
			}
		}
		
		public ITextBufferVersion Version {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int TextLength {
			get {
				if (WorkbenchSingleton.InvokeRequired) {
					return WorkbenchSingleton.SafeThreadFunction(() => TextLength);
				}
				return document.TextLength;
			}
		}
		
		public IDocumentLine GetLine(int lineNumber)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => GetLine(lineNumber));
			}
			return new ThreadSafeDocumentLine(document.GetLine(lineNumber));
		}
		
		public IDocumentLine GetLineForOffset(int offset)
		{
			throw new NotImplementedException();
		}
		
		public int PositionToOffset(int line, int column)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => PositionToOffset(line, column));
			}
			return document.PositionToOffset(line, column);
		}
		
		public Location OffsetToPosition(int offset)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, string text)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall(() => Insert(offset, text));
			} else {
				document.Insert(offset, text);
			}
		}
		
		public void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
		{
			throw new NotImplementedException();
		}
		
		public void Remove(int offset, int length)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall(() => Remove(offset, length));
			} else {
				document.Remove(offset, length);
			}
		}
		
		public void Replace(int offset, int length, string newText)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall(() => Replace(offset, length, newText));
			} else {
				document.Replace(offset, length, newText);
			}
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
		
		public ICSharpCode.SharpDevelop.ITextBuffer CreateSnapshot()
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.SharpDevelop.ITextBuffer CreateSnapshot(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public System.IO.TextReader CreateReader()
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
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}
}

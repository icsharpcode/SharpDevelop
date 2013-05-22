// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
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
		public event EventHandler<TextChangeEventArgs> TextChanging;
		public event EventHandler<TextChangeEventArgs> TextChanged;
		public event EventHandler ChangeCompleted;
		public event EventHandler FileNameChanged;
		#pragma warning restore 0067
		
		public string Text {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		void InvokeIfRequired(Action action)
		{
			SD.MainThread.InvokeIfRequired(action);
		}
		
		T InvokeIfRequired<T>(Func<T> callback)
		{
			return SD.MainThread.InvokeIfRequired(callback);
		}
		
		public int LineCount {
			get { return InvokeIfRequired(() => document.LineCount); }
		}
		
		public ITextSourceVersion Version {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int TextLength {
			get { return InvokeIfRequired(() => document.TextLength); }
		}
		
		public IDocumentLine GetLineByNumber(int lineNumber)
		{
			return InvokeIfRequired(() => new ThreadSafeDocumentLine(document.GetLine(lineNumber)));
		}
		
		public IDocumentLine GetLineByOffset(int offset)
		{
			throw new NotImplementedException();
		}
		
		public int GetOffset(int line, int column)
		{
			return InvokeIfRequired(() => document.PositionToOffset(line, column));
		}
		
		public TextLocation GetLocation(int offset)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, string text)
		{
			InvokeIfRequired(() => document.Insert(offset, text));
		}
		
		public void Insert(int offset, ITextSource text, AnchorMovementType defaultAnchorMovementType)
		{
			throw new NotImplementedException();
		}
		
		public void Remove(int offset, int length)
		{
			InvokeIfRequired(() => document.Remove(offset, length));
		}
		
		public void Replace(int offset, int length, string text)
		{
			InvokeIfRequired(() => document.Replace(offset, length, text));
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
		
		public string FileName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IDocument CreateDocumentSnapshot()
		{
			throw new NotImplementedException();
		}
		
		public int GetOffset(TextLocation location)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
		{
			throw new NotImplementedException();
		}
		
		public void Replace(int offset, int length, ITextSource newText)
		{
			throw new NotImplementedException();
		}
		
		public string GetText(ISegment segment)
		{
			throw new NotImplementedException();
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
		
		public void Insert(int offset, ITextSource text)
		{
			throw new NotImplementedException();
		}
	}
}

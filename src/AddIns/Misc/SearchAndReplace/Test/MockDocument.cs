// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom.Refactoring;
using System;

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
		
		public int TotalNumberOfLines {
			get {
				throw new NotImplementedException();
			}
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
	}
}

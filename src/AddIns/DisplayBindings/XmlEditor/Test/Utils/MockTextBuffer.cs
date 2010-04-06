// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop;

namespace XmlEditor.Tests.Utils
{
	public class MockTextBuffer : ITextBuffer
	{
		string text = String.Empty;
		
		public MockTextBuffer(string text)
		{
			this.text = text;
		}
		
		public event EventHandler TextChanged;
		
		protected virtual void OnTextChanged(EventArgs e)
		{
			if (TextChanged != null) {
				TextChanged(this, e);
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
		
		public string Text {
			get { return text; }
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
			return new StringReader(text);
		}
		
		public TextReader CreateReader(int offset, int length)
		{
			return new StringReader(text);
		}
		
		public char GetCharAt(int offset)
		{
			throw new NotImplementedException();
		}
		
		public string GetText(int offset, int length)
		{
			throw new NotImplementedException();
		}
	}
}

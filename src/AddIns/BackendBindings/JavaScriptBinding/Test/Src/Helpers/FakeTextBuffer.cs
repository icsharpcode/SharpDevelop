// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;

namespace JavaScriptBinding.Tests.Helpers
{
	public class FakeTextBuffer : ITextBuffer
	{
		public FakeTextBuffer(string text)
		{
			this.Text = text;
		}
		
		#pragma warning disable 0067
		public event EventHandler TextChanged;
		#pragma warning restore
		
		public int TextLength {
			get { return Text.Length; }
		}
		
		public string Text { get; set; }
		
		public ITextBufferVersion Version {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ITextBuffer CreateSnapshot()
		{
			throw new NotImplementedException();
		}
		
		public ITextBuffer CreateSnapshot(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public System.IO.TextReader CreateReader()
		{
			throw new NotImplementedException();
		}
		
		public System.IO.TextReader CreateReader(int offset, int length)
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
	}
}

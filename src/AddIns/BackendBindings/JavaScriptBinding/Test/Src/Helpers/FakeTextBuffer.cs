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

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
using System.Text;

namespace ICSharpCode.Scripting
{
	public class ScriptingConsoleOutputStream : Stream
	{
		IScriptingConsoleTextEditor textEditor;
		IControlDispatcher dispatcher;
		
		public ScriptingConsoleOutputStream(IScriptingConsoleTextEditor textEditor, IControlDispatcher dispatcher)
		{
			this.textEditor = textEditor;
			this.dispatcher = dispatcher;
		}
		
		public override bool CanRead {
			get { return false; }
		}
		
		public override bool CanSeek {
			get { return false; }
		}
		
		public override bool CanWrite {
			get { return true; }
		}
		
		public override long Length {
			get { return 0; }
		}
		
		public override long Position {
			get { return 0; }
			set { }
		}
		
		public override void Flush()
		{
		}
		
		public override long Seek(long offset, SeekOrigin origin)
		{
			return 0;
		}
		
		public override void SetLength(long value)
		{
		}
		
		public override int Read(byte[] buffer, int offset, int count)
		{
			return 0;
		}
		
		/// <summary>
		/// Assumes the bytes are UTF8 and writes them to the text editor.
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count)
		{
			string text = UTF8Encoding.UTF8.GetString(buffer, offset, count);
			ThreadSafeTextEditorWrite(text);
		}
		
		void ThreadSafeTextEditorWrite(string text)
		{
			if (dispatcher.CheckAccess()) {
				textEditor.Append(text);
			} else {
				Action<string> action = ThreadSafeTextEditorWrite;
				dispatcher.Invoke(action, text);
			}
		}
	}
}

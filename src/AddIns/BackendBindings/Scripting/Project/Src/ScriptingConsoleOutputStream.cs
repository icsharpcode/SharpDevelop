// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

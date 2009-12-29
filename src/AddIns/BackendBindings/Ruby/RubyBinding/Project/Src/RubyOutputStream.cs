// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.RubyBinding
{
	public class RubyOutputStream : Stream
	{
		ITextEditor textEditor;
		
		public RubyOutputStream(ITextEditor textEditor)
		{
			this.textEditor = textEditor;
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
			textEditor.Write(text);
		}		
	}
}

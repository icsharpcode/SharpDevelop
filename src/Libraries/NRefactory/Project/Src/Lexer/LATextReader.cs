// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.NRefactory.Parser
{
	public class LATextReader : TextReader
	{
		List<int> buffer;
		TextReader reader;
		
		public LATextReader(TextReader reader)
		{
			this.buffer = new List<int>();
			this.reader = reader;
		}
		
		public override int Peek()
		{
			return Peek(0);
		}
		
		public override int Read()
		{
			int c = Peek();
			buffer.RemoveAt(0);
			return c;
		}
		
		public int Peek(int step)
		{
			while (step >= buffer.Count) {
				buffer.Add(reader.Read());
			}
			
			if (step < 0)
				return -1;
			
			return buffer[step];
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				reader.Dispose();
			base.Dispose(disposing);
		}
	}
}

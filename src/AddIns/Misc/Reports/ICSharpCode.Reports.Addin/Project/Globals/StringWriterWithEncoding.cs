// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of StringWriterWithEncoding.
	/// </summary>
	internal class StringWriterWithEncoding:System.IO.StringWriter
	{
		private Encoding encoding;
		
		public StringWriterWithEncoding(Encoding encoding)
		{
			if (encoding == null) {
				throw new ArgumentNullException("encoding");
			}
			this.encoding = encoding;
		}
		
		public override Encoding Encoding {
			get { return encoding; }
		}
		
	}
}

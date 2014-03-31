/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.03.2014
 * Time: 18:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

namespace ICSharpCode.Reporting.Addin.Globals
{
	/// <summary>
	/// Description of StringWriterWithEncoding.
	/// </summary>
	class StringWriterWithEncoding:System.IO.StringWriter
	{
		private readonly Encoding encoding;
		
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

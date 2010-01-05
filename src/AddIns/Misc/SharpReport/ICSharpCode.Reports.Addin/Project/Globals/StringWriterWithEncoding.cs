/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 21.06.2009
 * Zeit: 18:26
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
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

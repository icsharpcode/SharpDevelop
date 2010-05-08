/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 21.06.2009
 * Zeit: 19:29
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Reports.Addin
{
	[Serializable()]
	public class FormsDesignerLoadException : Exception
	{
		public FormsDesignerLoadException() : base()
		{
		}
		
		public FormsDesignerLoadException(string message) : base(message)
		{
		}
		
		public FormsDesignerLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected FormsDesignerLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

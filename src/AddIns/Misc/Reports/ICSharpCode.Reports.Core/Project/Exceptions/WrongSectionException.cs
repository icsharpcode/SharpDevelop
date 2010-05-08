/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 24.03.2010
 * Zeit: 19:31
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of WrongSectionException.
	/// </summary>
	[Serializable()]
	public class WrongSectionException: Exception
	{
		
		public WrongSectionException():base()
		{
		}
		
		
		public WrongSectionException(string errorMessage) :base (errorMessage)
		{
		}
		
		public WrongSectionException(string errorMessage,
		                             Exception exception):base (errorMessage,exception)
		{
		}
		
		protected WrongSectionException(SerializationInfo info,
		                                StreamingContext context) : base(info, context)
		{
			// Implement type-specific serialization constructor logic.
		}
	}
}

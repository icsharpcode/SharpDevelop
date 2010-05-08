/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 09.11.2008
 * Zeit: 11:49
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of MissingParentException.
	/// </summary>
	[Serializable()]
	public class MissingParentException: Exception
	{
		
		public MissingParentException():base(){
			
		}
		public MissingParentException(string errorMessage) :base (errorMessage){
			
		}
		public MissingParentException(string errorMessage,
		                              Exception exception):base (errorMessage,exception){
			
		}
		
		protected MissingParentException(SerializationInfo info,
		                                 StreamingContext context) : base(info, context){
			// Implement type-specific serialization constructor logic.
		}
	}
}

/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 11.01.2007
 * Zeit: 22:42
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of WrongColumnException.
	/// </summary>
	[Serializable()]
	public class WrongColumnException: Exception
	{
		
		public WrongColumnException():base()
		{
		}
		public WrongColumnException(string errorMessage) :base (errorMessage)
		{
		}
		public WrongColumnException(string errorMessage,
		                             Exception exception):base (errorMessage,exception)
		{
		}
		
		protected WrongColumnException(SerializationInfo info,
		                                StreamingContext context) : base(info, context)
		{
			// Implement type-specific serialization constructor logic.
		}
		
		[SecurityPermissionAttribute(SecurityAction.Demand, 
          SerializationFormatter = true)]

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

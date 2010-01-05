/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 08.02.2009
 * Zeit: 13:07
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of UnknownFunctionException.
	/// </summary>
	[Serializable()]
	public class UnknownFunctionException: Exception
	{
		
		public UnknownFunctionException():base()
		{
		}
		public UnknownFunctionException(string errorMessage) :base (errorMessage)
		{
		}
		public UnknownFunctionException(string errorMessage,
		                             Exception exception):base (errorMessage,exception)
		{
		}
		
		protected UnknownFunctionException(SerializationInfo info,
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

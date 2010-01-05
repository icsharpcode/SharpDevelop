/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 22.08.2009
 * Zeit: 20:01
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of AsyncPageBuilderException.
	/// </summary>
	public class AsyncPageBuilderException:Exception
	{
		public AsyncPageBuilderException():base ()
		{
		}
		
		public AsyncPageBuilderException(string errorMessage) :base (errorMessage)
		{
		}
		
		public AsyncPageBuilderException(string errorMessage,
		                           Exception exception):base (errorMessage,exception)
		{
		}

		
		protected AsyncPageBuilderException(SerializationInfo info,
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

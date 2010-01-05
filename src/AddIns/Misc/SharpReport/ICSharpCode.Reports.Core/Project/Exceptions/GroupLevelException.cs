/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 15.11.2008
 * Zeit: 19:45
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

/// <summary>
/// This exception is throw'n when something is wrong with the File Format
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 25.04.2005 14:29:20
/// </remarks>
namespace ICSharpCode.Reports.Core {
	
	[Serializable()]
	public class GroupLevelException : System.Exception {
		
		public GroupLevelException():base ()
		{
		}
		
		public GroupLevelException(string errorMessage) :base (errorMessage)
		{
		}
		
		public GroupLevelException(string errorMessage,
		                           Exception exception):base (errorMessage,exception)
		{
		}

		
		protected GroupLevelException(SerializationInfo info,
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

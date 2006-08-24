/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 03.02.2006
 * Time: 13:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

/// <summary>
/// Description of MissingModelException.
/// </summary>
namespace SharpReportCore{

	[Serializable()]
	public class MissingModelException: System.Exception
	{
		
		public MissingModelException():base(){
			
		}
		public MissingModelException(string errorMessage) :base (errorMessage){
			
		}
		public MissingModelException(string errorMessage,
		                             Exception exception):base (errorMessage,exception){
			
		}
		
		protected MissingModelException(SerializationInfo info,
		                                StreamingContext context) : base(info, context){
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

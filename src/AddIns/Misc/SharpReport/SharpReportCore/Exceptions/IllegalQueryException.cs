/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 15.06.2006
 * Time: 17:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SharpReportCore
{
	/// <summary>
	/// IllegalQueryException is thrown when a QueryString contains other statements as
	/// Select. So it should not happen do alter a Database while doing a Query with SharpReport
	/// </summary>
	
	[SerializableAttribute]
	public class IllegalQueryException: System.Exception{
		private const  string message = "Query should start with 'Select'";
		
		string errorMessage;
		
		public IllegalQueryException():base(message){
			
		}
		public IllegalQueryException(string errorMessage) :base (errorMessage){
			this.errorMessage = errorMessage;
		}
		public IllegalQueryException(string errorMessage,
		                            Exception exception):base (errorMessage,exception){
			
		}
		
		protected IllegalQueryException(SerializationInfo info,
         StreamingContext context) : base(info, context){
         // Implement type-specific serialization constructor logic.
      	}
		
		public string ErrorMessage {
			get {
				if (String.IsNullOrEmpty(this.errorMessage)) {
					return IllegalQueryException.message;
				} else {
					return this.errorMessage;
				}
			}
		}
 		
		[SecurityPermissionAttribute(SecurityAction.Demand,
          SerializationFormatter = true)]

		public override void GetObjectData(SerializationInfo info, StreamingContext context){
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue("errorMessage", this.ErrorMessage);
			base.GetObjectData(info, context);
		}
	}
}

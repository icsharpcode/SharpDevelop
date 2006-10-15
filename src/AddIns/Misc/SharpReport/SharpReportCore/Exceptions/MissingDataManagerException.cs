/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 05.10.2006
 * Time: 16:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
namespace SharpReportCore {	
	[Serializable()]
	public class MissingDataManagerException : System.Exception {
		
		string errorMessage = String.Empty;
		
		public MissingDataManagerException():base() {
			
		}
		
		public MissingDataManagerException(string errorMessage):base(errorMessage ){
			this.errorMessage = errorMessage;
		}
		
		public MissingDataManagerException(string errorMessage,
		                            Exception exception):base (errorMessage,exception){
			
		}
		
		protected MissingDataManagerException(SerializationInfo info, 
         StreamingContext context) : base(info, context){
         // Implement type-specific serialization constructor logic.
      	}
		public string ErrorMessage {
			get {
				return errorMessage;
			}
		}
		
		[SecurityPermissionAttribute(SecurityAction.Demand,
          SerializationFormatter = true)]

		public override void GetObjectData(SerializationInfo info, StreamingContext context){
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue("errorMessage", this.errorMessage);
			base.GetObjectData(info, context);
		}
	}
}


// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
namespace ICSharpCode.Reports.Core {	
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

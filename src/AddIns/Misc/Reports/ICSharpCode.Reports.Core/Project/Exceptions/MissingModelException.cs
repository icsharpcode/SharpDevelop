// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

/// <summary>
/// Description of MissingModelException.
/// </summary>
namespace ICSharpCode.Reports.Core{

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

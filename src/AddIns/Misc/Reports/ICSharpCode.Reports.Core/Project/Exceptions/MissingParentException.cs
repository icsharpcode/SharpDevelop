// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;
/// <summary>
/// Description of MissingSectionException.
/// </summary>
namespace ICSharpCode.Reports.Core
{

	[Serializable()]
	public class MissingSectionException: Exception
	{
		
		public MissingSectionException():base(){
			
		}
		public MissingSectionException(string errorMessage) :base (errorMessage){
			
		}
		public MissingSectionException(string errorMessage,
		                             Exception exception):base (errorMessage,exception){
			
		}
		
		protected MissingSectionException(SerializationInfo info,
		                                StreamingContext context) : base(info, context){
			// Implement type-specific serialization constructor logic.
		}
	}
}

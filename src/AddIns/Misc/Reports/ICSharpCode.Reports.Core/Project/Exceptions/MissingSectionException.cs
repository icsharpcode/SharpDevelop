// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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

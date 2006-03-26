/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 12.03.2006
 * Time: 22:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Runtime.Serialization;
/// <summary>
/// Description of MissingSectionException.
/// </summary>
namespace SharpReportCore{

	[Serializable()]
	public class MissingSectionException: System.Exception
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


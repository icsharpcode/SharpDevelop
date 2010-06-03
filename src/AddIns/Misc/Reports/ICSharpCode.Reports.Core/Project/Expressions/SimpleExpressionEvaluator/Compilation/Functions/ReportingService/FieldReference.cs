/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.06.2010
 * Time: 11:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SimpleExpressionEvaluator.Compilation.Functions.ReportingService
{
	[Tokens("Fields","fields")]
	public class FieldReference:Function<string>
	{
		public FieldReference ()
		{
			
		}
		protected override int ExpectedArgumentCount {
			get { return 1; }
		}
		
		public override Type ReturnType {
			get { return base.ReturnType; }
		}
		
		protected override string EvaluateFunction(params object[] args)
		{
			return Environment.UserName;
		}
		
	}
}

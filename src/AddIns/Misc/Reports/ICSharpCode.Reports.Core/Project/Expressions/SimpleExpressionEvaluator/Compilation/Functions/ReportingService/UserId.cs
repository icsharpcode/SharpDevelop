/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 31.05.2009
 * Zeit: 19:16
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;

namespace SimpleExpressionEvaluator.Compilation.Functions.ReportingService
{
	/// <summary>
	/// Description of UserId.
	/// </summary>
	 [Tokens("UserId")]
	 public class UserId:Function<string>
	{
		public UserId()
		{
		}
		
		
		protected override int ExpectedArgumentCount {
			get { return 0; }
		}
	 	
		public override Type ReturnType {
			get { return base.ReturnType; }
		}
		
		
	 	
//		public override string Evaluate(SimpleExpressionEvaluator.Evaluation.IExpressionContext context)
//		{
//			return base.Evaluate(context);
//		}
	 	
		protected override string EvaluateFunction(params object[] args)
		{
			return Environment.UserName;
		}
		
	}
}

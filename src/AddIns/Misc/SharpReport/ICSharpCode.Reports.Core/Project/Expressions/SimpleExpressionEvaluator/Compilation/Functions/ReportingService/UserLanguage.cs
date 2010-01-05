/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 31.05.2009
 * Zeit: 19:31
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;

namespace SimpleExpressionEvaluator.Compilation.Functions.ReportingService
{
	/// <summary>
	/// Description of UserLanguage.
	/// </summary>
	[Tokens("Language")]
	public class UserLanguage:Function<string>
	{
		public UserLanguage()
		{
		}
		
		protected override int ExpectedArgumentCount {
			get { return 0; }
		}
	 	
		public override Type ReturnType {
			get { return base.ReturnType; }
		}
		
		protected override string EvaluateFunction(params object[] args)
		{
			return System.Globalization.CultureInfo.CurrentCulture.ThreeLetterISOLanguageName;
				
		}
	}
}

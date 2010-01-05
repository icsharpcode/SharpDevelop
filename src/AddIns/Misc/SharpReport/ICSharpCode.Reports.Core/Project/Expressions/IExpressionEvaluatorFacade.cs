/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 28.06.2009
 * Zeit: 19:53
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using ICSharpCode.Reports.Core.Interfaces;
using System;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
	/// <summary>
	/// Description of IExpressionEvaluatorFassade.
	/// </summary>
	public interface IExpressionEvaluatorFacade
	{
		string Evaluate (string expression);
		ISinglePage SinglePage {get;set;}
	}
}

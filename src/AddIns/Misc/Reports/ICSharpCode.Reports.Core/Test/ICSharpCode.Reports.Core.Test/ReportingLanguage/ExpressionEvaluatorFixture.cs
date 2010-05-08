/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 06.12.2009
 * Zeit: 19:17
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using NUnit.Framework;
using ICSharpCode.Reports.Expressions.ReportingLanguage;


namespace ICSharpCode.Reports.Core.Test.ReportingLanguage
{
	[TestFixture]
	public class ExpressionEvaluatorFixture
	{
		IExpressionEvaluatorFacade facade;
		
		[Test]
		public void CanInitExpressionEvaluatorFassade()
		{
			IExpressionEvaluatorFacade f = new ExpressionEvaluatorFacade();
			Assert.IsNotNull (f);
		}
		
		[Test]
		public void NoEvaluationOfPlainText ()
		{
			 string jpt = "just plain text";
			 string retVal = facade.Evaluate(jpt);
			 Assert.AreEqual(jpt,retVal);
		}
		
		[Test]
		public void NoEvaluationOfFormulaLikeText ()
		{
			string fakeFormula = "A - B";
			string retVal = facade.Evaluate(fakeFormula);
			Assert.AreEqual(retVal,fakeFormula);
		}
		
		[TestFixtureSetUp]
		public void Init()
		{
			facade = new ExpressionEvaluatorFacade();
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}

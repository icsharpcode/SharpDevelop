/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 06.12.2009
 * Zeit: 19:17
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage
{
	[TestFixture]
	public class ExpressionEvaluatorFixture:ConcernOf<ExpressionEvaluatorFacade>
	{
		
		[Test]
		public void Can_Init_ExpressionEvaluatorFassade()
		{
			IExpressionEvaluatorFacade f = new ExpressionEvaluatorFacade();
			Assert.IsNotNull (f);
		}
		
		[Test]
		public void Plain_Text_Returns_Unmodified ()
		{
			 string expression = "just plain text";
			 string retVal = Sut.Evaluate(expression);
			 Assert.AreEqual(expression,retVal);
		}
		
		
		[Test]
		public void SimpleEvaluation ()
		{
			string expression = "=5 + 2";
			string result = "7";
			string retVal = Sut.Evaluate(expression);
			Assert.AreEqual(result,retVal);
		}
		
		
		public override void Setup()
		{
			Sut = new ExpressionEvaluatorFacade();
		}
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}

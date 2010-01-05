/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 06.07.2009
 * Zeit: 19:56
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;
using SimpleExpressionEvaluator;
using SimpleExpressionEvaluator.Evaluation;
using SimpleExpressionEvaluator.Utilities;


namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.IntegrationTests
{
	[TestFixture]
	public class FieldReferenceFixture
	{
		
		private IExpressionEvaluatorFacade evaluator;
		
		[Test]
		public void Can_Compile_Simple_FieldReference()
		{
			const string expression = "=Fields!Field1";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("Field1"));
		}
		
		
		[Test]
		public void Can_Concat_Simple_FieldReference()
		{
			const string expression = "=Fields!Field1 + 'SharpDevelopReporting'";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("Field1SharpDevelopReporting"));
		}
		
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.evaluator = new ExpressionEvaluatorFacade();
			this.evaluator.SinglePage = TestHelper.CreateSinglePage();
		}
	}
}

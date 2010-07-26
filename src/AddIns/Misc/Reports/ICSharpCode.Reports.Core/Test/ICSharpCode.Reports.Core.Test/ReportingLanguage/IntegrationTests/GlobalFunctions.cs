/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 12.06.2009
 * Zeit: 20:06
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
	public class GlobalFunctions
	{
		private SinglePage singlePage;
		private IExpressionEvaluatorFacade evaluator;
		
		[Test]
		public void Can_Compile_PageNumber()
		{
			const string expression = "=Globals!PageNumber";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.singlePage.PageNumber.ToString()));
		}
		
		
		[Test]
		public void Can_Compile_TotalPages()
		{
			const string expression = "=Globals!TotalPages";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.singlePage.TotalPages.ToString()));
		}
		
		
		[Test]
		public void Can_Compile_ReportName()
		{
			const string expression = "=Globals!ReportName";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.singlePage.ReportName));
		}
		
		
		[Test]
		public void Can_Compile_ReportFolder()
		{
			const string expression = "=Globals!ReportFolder";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.singlePage.ReportFolder));
		}
				
		
		[Test]
		public void Can_Compile_ExecutionTime()
		{
			const string expression = "=Globals!ExecutionTime";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.singlePage.ExecutionTime.ToString()));
		}
		
		
		[Test]
		
		#region Template for Resolution
/*		
//			const string expression = "PageNumber()";
//			var compiler = new ReportingLanguageCompiler();
//			IExpression compiled = compiler.CompileExpression<string>(expression);
			
//			var context = new ExpressionContext(null);
//			context.ResolveUnknownVariable += VariableStore;
            
			
//			context.ResolveUnknownVariable += (sender, args) =>
//                                                  {
//                                                      Assert.That(args.VariableName, Is.EqualTo("pagenumber"));
//                                                      //args.VariableValue = 123.456;
//                                                  };
            //Assert.That(compiled.Evaluate(context), Is.EqualTo("15"));
            Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("15"));
            //this.evaluator.Evaluate(expression)
  */          
		#endregion
		
		public void Can_Compile_PageNumber_AsFunction()
		{
			const string expression = "=PageNumber()";
            Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("15"));
		}

		
		[Test]
		public void Can_Compile_Function_From_Pageinfo()
		{
			const string expression = "=TotalPages()";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.singlePage.TotalPages.ToString()));
		}
		
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Throw_On_UnknownFunction()
		{
			const string expression = "=TotalWrongFunction()";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.singlePage.TotalPages.ToString()));
		}
		
		
		[SetUp]
		public void Init()
		{
			this.singlePage = TestHelper.CreateSinglePage();
			this.evaluator = new ExpressionEvaluatorFacade();
			this.evaluator.SinglePage = this.singlePage;
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core.BaseClasses;
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
		public void UnknownFunction_ErrorMessage()
		{
			const string expression = "=TotalWrongFunction()";
			string s  = this.evaluator.Evaluate(expression);
			Assert.That(s.Contains("TotalWrongFunction"));
		}
		
		
		[SetUp]
		public void Init()
		{
			this.singlePage = TestHelper.CreateSinglePage();
			this.evaluator = new ExpressionEvaluatorFacade(this.singlePage);
		}
	}
}

// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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

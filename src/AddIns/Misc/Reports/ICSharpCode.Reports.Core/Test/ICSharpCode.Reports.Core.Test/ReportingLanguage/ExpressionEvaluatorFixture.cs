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
			IExpressionEvaluatorFacade f = new ExpressionEvaluatorFacade(null);
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
		public void ExtractExpressionPartFromString()
		{
			const string expression = "c = 5 + 3";
			Assert.That(EvaluationHelper.ExtractExpressionPart(expression),Is.EqualTo("5 + 3"));
		}
		
		
		[Test]
		public void MissingEqualSignReturnsEmptyString ()
		{
			const string expression = "5 + 3";
			Assert.That(EvaluationHelper.ExtractExpressionPart(expression),Is.EqualTo(String.Empty));
		}
		
		
		[Test]
		public void ExtractResultPartFromString()
		{
			const string expression = "c = 5 + 3";
			Assert.That(EvaluationHelper.ExtractResultPart(expression),Is.EqualTo("c"));
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
			Sut = new ExpressionEvaluatorFacade(null);
		}
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}

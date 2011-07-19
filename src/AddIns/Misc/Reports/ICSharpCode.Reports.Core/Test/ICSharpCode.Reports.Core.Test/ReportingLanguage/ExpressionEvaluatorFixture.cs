// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

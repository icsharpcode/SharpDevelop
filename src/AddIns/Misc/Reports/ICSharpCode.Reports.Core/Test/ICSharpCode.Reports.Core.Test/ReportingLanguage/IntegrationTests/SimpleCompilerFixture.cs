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
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;
using SimpleExpressionEvaluator;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage
{
	
	[TestFixture]
	public class SimpleCompilerFixture
	{
		
		private IExpressionEvaluatorFacade evaluator;
		
		#region Empty and null Expressions 
		
		[Test]
		public void Can_Handle_Empty_Expression()
		{
			const string expression = "";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(String.Empty));
		}
		
		
		[Test]
		public void Can_Handle_Empty_Expression_After_Cleanup()
		{
			const string expression = "= ";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("= "));
		}
		
		
		#endregion
		
		#region SyntaxError
		
		[Test]
		public void Can_Handle_Wrong_Syntax_Returns_Expression()
		{
			const string expression = "User!hh";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("User!hh"));
		}
		
		
		#endregion
		
		#region Simple Literals
		
		[Test]
		public void Can_Compile_Simple_Number()
		{
			const string expression = " =1.1";
			Assert.That(this.evaluator.Evaluate(expression),Is.StringStarting("1"));
		}
		
		
		[Test]
		public void Can_Compile_Simple_String()
		{
			const string expression = "'SharpReport'";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(expression));
		}
		
		#endregion
		
		
		#region Setup
		
		[SetUp]
		public void Init()
		{
			this.evaluator = new ExpressionEvaluatorFacade(null);
		}
		
		#endregion
	}
	
}

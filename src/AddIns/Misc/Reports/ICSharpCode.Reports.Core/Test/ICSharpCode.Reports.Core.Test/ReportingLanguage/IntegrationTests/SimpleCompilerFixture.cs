// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

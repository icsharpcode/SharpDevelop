// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Reporting.Expressions.Irony;
using Irony.Interpreter.Evaluator;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions
{
	[TestFixture]
	public class IronyBasics
	{
		ExpressionEvaluatorGrammar grammar;
		ExpressionEvaluator evaluator;
		
		#region Calculation
		
		[Test]
		public void CanAddNumber() {
		
			var script = "2 + 3";
			var result = evaluator.Evaluate(script);
			Assert.That(result,Is.EqualTo(5));
		}
		
		#endregion
		
		
		#region Stringhandling
		
		[Test]
		public void CanConcatString() {
			var result = evaluator.Evaluate("'SharpDevelop' + ' ' + 'is great'");
			Assert.That(result,Is.EqualTo("SharpDevelop is great"));
		}
		#endregion
		
		#region System.Environment
		
		[Test]
		public void CanUserSystemEnvironment() {
			
			//Using methods imported from System.Environment
			var script = @"report = '#{MachineName}-#{OSVersion}-#{UserName}'";
			var result = evaluator.Evaluate(script);
			var expected = string.Format("{0}-{1}-{2}", Environment.MachineName, Environment.OSVersion, Environment.UserName);
			Assert.AreEqual(expected, result, "Unexpected computation result");
		}
		
		#endregion
		
		#region System.Math
		
		[Test]
		public void CanRunSystemMath () {
			//Using methods imported from System.Math class
			var script = @"abs(-1.0) + Log10(100.0) + sqrt(9) + floor(4.5) + sin(PI/2)";
			var result = evaluator.Evaluate(script);
			Assert.IsTrue(result is double, "Result is not double.");
			Assert.AreEqual(11.0, (double) result, 0.001, "Unexpected computation result");
		}
		
		#endregion
		
		[SetUp]
		public void Initialize() {
			grammar = new ReportingLanguageGrammer();
			evaluator = new ExpressionEvaluator(grammar);
		}
	}
}

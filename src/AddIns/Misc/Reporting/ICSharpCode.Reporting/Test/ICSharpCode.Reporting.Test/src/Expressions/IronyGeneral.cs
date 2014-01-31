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
using ICSharpCode.Reporting.Expressions.Irony;
using Irony.Interpreter.Evaluator;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions
{
	[TestFixture]
	public class IronyBasics
	{
		
		ReportingLanguageGrammer grammar;
		ReportingExpressionEvaluator evaluator;
		
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
			evaluator = new ReportingExpressionEvaluator(grammar);
		}
	}
}

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

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.LanguageTests
{
	/// <summary>
	/// Description of SimpleMath.
	/// </summary>
	[TestFixture]
	public class SimpleMath
	{
	
		[Test]
		public void Can_Add_Two_Numbers()
		{
			string expression = "1 + 2";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo(3));
		}
		
		
		[Test]
		public void Can_Subtract_Two_Numbers ()
		{
			string expression = "15 -3";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo(12.0));
		}
		
		
		[Test]
		public void Can_Multiply_Two_Numbers ()
		{
			string expression = "3 * 2";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo(6.0));
		}
		
		[Test]
		public void Can_Divide_Two_Numbers ()
		{
			string expression = "21 / 3";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo(7.0));
		}
		
		[Test]
		public void Can_Divide_Two_Numbers_With_Parenthesis ()
		{
			string expression = "(21 / 3) + 2";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo(9.0));
		}
		
		[Test]
		public void Can_Divide_Two_Numbers_With_Double_Parenthesis ()
		{
			string expression = "(21 / 3) + (2 * 5)";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo(17.0));
		}
	}
}

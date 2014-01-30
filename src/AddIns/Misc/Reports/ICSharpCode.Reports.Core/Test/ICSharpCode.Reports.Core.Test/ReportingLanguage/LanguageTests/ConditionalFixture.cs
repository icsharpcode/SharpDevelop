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
	[TestFixture]
	public class ConditionalFixture
	{
		
		[Test]
        public void Can_Parse_Basic_Conditional()
        {
            const string expression = "if (true) then 1";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<int>(expression);
            Assert.That(expr.Evaluate(null), Is.EqualTo(1));
        }
        
       
         [Test]
        public void Can_Parse_Basic_Conditional_False_Result()
        {
            const string expression = "if (false) then 1";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<int>(expression);
            Assert.That(expr.Evaluate(null), Is.EqualTo(0));
        }
       
       
        [Test]
        public void Can_Parse_If_Then_Else()
        {
            const string expression = "if (false) then 1 else 2";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<int>(expression);
            Assert.That(expr.Evaluate(null), Is.EqualTo(2));
        }
        
        [Test]
        public void Can_Parse_If_Then_Else_2()
        {
            const string expression = "if (true) then 'red' else 'black'";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<string>(expression);
            Assert.That(expr.Evaluate(null), Is.EqualTo("red"));
        }
        
        
        [Test]
        public void Can_Parse_If_Then_Else_In_Complex_Context()
        {
            const string expression = "2 * (if (true) then 9 otherwise 5 + 3)^2";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<int>(expression);
            Assert.That(expr.Evaluate(null),Is.EqualTo(288));
        }

	}
}

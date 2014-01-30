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
	public class DateTimeFunctionsFixture
	{
	
		[Test]
        public void Can_Compile_Function()
        {
        	const string expression = "today()";
        	IExpression expr = CreateExpression(expression);
        	Assert.That(expr.Evaluate(null), Is.EqualTo(DateTime.Today));
        }
		
		
		[Test]
		public void Can_Compile_Expr_With_Generic_Return()
		{
			const string expression = "Today()";
			IExpression expr = CreateExpression(expression);
			Assert.That(expr.Evaluate(null), Is.EqualTo(DateTime.Today));
		}
        
        [Test]
        public void Can_Compile_DateAdd()
        {
        	const string expression = "dateAdd(today(),d,1)";
        	IExpression expr = CreateExpression(expression);
        	Assert.That(expr.Evaluate(null), Is.EqualTo(DateTime.Today.AddDays(1)));
        }
        
        [Test]
        public void Can_Compile_DateDiff ()
        {
        	const string expression = "dateSubtract('10.02.2013','09.02.2013')";
        	IExpression expr = CreateExpression(expression);
        	Assert.That(expr.Evaluate(null), Is.EqualTo(new TimeSpan(1,0,0,0).Duration()));
        }
        
        
        [Test]
        public void Can_Compile_DateDiff_Start_Greater_End ()
        {
        	const string expression = "dateSubtract('09.02.2013','10.02.2013')";
        	IExpression expr = CreateExpression(expression);
        	Assert.That(expr.Evaluate(null), Is.EqualTo(new TimeSpan(-1,0,0,0)));
        }
        
        [Test]
        public void Can_Compile_DateDiff_Include_Hours_Minutes()
        {

        	//       	Example found at:
        	//        	http://msdn.microsoft.com/en-us/library/ae6246z1.aspx

        	System.DateTime date1 = new System.DateTime(1996, 6, 3, 22, 15, 0);
        	System.DateTime date2 = new System.DateTime(1996, 12, 6, 13, 2, 0);
        	// diff1 gets 185 days, 14 hours, and 47 minutes.
        	System.TimeSpan diff1 = date2.Subtract(date1);

        	const string expression = "dateSubtract('1996.12.6 13:2:0','1996.6.3 22:15:0')";
        	
        	IExpression expr = CreateExpression(expression);
        	Assert.That(expr.Evaluate(null), Is.EqualTo(diff1));
        }
        
        
        private static IExpression CreateExpression (string expression)
        {
        	var compiler = new ReportingLanguageCompiler();
        	return compiler.CompileExpression<object>(expression);
        }
	}
}

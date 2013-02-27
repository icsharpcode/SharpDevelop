// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

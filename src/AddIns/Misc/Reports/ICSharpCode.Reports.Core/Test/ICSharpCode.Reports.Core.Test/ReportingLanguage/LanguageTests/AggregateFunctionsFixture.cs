// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Reports.Expressions.ReportingLanguage;
using System;
using NUnit.Framework;
using SimpleExpressionEvaluator;
using SimpleExpressionEvaluator.Evaluation;
using SimpleExpressionEvaluator.Utilities;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.LanguageTests
{
	[TestFixture]
	public class AggregateFunctionsFixture
	{
		
		[Test]
        public void Array_Sum()
        {
            var data = new[] {1, 2, 4, 8, 16, 32, 64, 128};
            var eStr = "sum()";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<double>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(255));
        }
        
        [Test]
        public void Array_Average()
        {
            var data = new[] {2, 4, 6};
            var eStr = "avg()";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<double>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(4));
        }
		
        
        [Test]
        public void Array_Handles_Divide_By_Zero()
        {
            var data = new int[]{};
            var eStr = "average()";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<double>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(0));
        }

        [Test]
        public void Average_Maintains_Precision()
        {
            var data = new[] {1.5, 2.0};
            var eStr = "avg()";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<double>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)),Is.EqualTo(1.75));
        }

        [Test]
        public void Matches_Any_Positive_Match()
        {
            var data = new[] {1, 2, 3, 4};
            var eStr = "matches_any(current,current > 3)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<bool>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)),Is.EqualTo(true));
        }

        [Test]
        public void Matches_Any_Negative_Match()
        {
            var data = new[] { 1, 2, 3, 4 };
            var eStr = "MatchesAny(nothing,current > 4)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<bool>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(false));
        }

        [Test]
        public void Matches_All_Positive_Match()
        {
            var data = new[] { 1, 2, 3, 4 };
            var eStr = "matches_all(current,current >= 1)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<bool>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(true));
        }

        [Test]
        public void Matches_All_Negative_Match()
        {
            var data = new[] { 1, 2, 3, 4 };
            var eStr = "MatchesAll(null,current > 1)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<bool>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(false));
        }

        
        
        [Test]
        public void Count_Will_Only_Count_Matches()
        {
            var data = new[] { 1, 2, 3, 4 };
            var eStr = "count(null,current > 1)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<int>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(3));
        }

        [Test]
        public void Count_Simple_Integers()
        {
            var data = new[] { 1, 2, 3, 4 };
            var eStr = "count()";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<int>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(4));
        }
        
        
        [Test]
        public void Array_Min()
        {
            var data = new[] { 10, 7, 3, 9 };
            var eStr = "min()";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<int>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(3));
        }

        [Test]
        public void Array_Max()
        {
            var data = new[] { 10, 7, 3, 9 };
            var eStr = "max(current,current * 2)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<int>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo(20));
        }

        [Test]
        public void ItemAtIndex_First_Position_Test()
        {
            var data = new[] {"a", "c", "f"};
            var eStr = "item_at_index(current,0)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<string>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo("a"));
        }

        [Test]
        public void ItemAtIndex_Middle_Position_Test()
        {
            var data = new[] { "a", "c", "f" };
            var eStr = "item_at_index(current,1)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<string>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo("c"));
        }

        [Test]
        public void ItemAtIndex_Last_Position_Test()
        {
            var data = new[] { "a", "c", "f" };
            var eStr = "item_at_index(current,2)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<string>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.EqualTo("f"));
        }

        [Test]
        public void ItemAtIndex_Out_Of_Range_Position_Test()
        {
            var data = new[] { "a", "c", "f" };
            var eStr = "item_at_index(current,3)";
            var compiler = new ReportingLanguageCompiler();
            var expr = compiler.CompileExpression<string>(eStr);
            Assert.That(expr.Evaluate(new ExpressionContext(data)), Is.Null);
        }
	}
}

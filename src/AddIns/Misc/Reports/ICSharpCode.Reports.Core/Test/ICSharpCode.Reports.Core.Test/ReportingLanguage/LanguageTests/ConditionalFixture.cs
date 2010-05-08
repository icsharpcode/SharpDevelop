/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 07.06.2009
 * Zeit: 11:23
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

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

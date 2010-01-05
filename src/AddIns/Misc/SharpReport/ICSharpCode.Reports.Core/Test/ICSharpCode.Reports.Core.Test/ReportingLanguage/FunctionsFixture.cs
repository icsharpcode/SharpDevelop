/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 02.06.2009
 * Zeit: 19:51
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
	public class FunctionsFixture
	{
	
		[Test]
        public void Can_Compile_Function()
        {
            const string expression = "today()";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<DateTime>(expression);
            Assert.That(expr.Evaluate(null), Is.EqualTo(DateTime.Today));
        }
		
		
		[Test]
        public void Can_Compile_Expr_With_Generic_Return()
        {
            const string expression = "Today()";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<object>(expression);
            Assert.That(expr.Evaluate(null), Is.EqualTo(DateTime.Today));
        }
        
        [Test]
        public void Can_Compile_DateAdd()
        {
            const string expression = "dateAdd(today(),d,1)";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<object>(expression);
            Assert.That(expr.Evaluate(null), Is.EqualTo(DateTime.Today.AddDays(1)));
        }
	}
}

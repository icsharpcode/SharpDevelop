/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 31.05.2009
 * Zeit: 19:48
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
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

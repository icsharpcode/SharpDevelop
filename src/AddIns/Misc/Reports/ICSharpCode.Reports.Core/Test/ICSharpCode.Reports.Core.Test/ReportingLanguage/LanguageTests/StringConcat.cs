// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;
using SimpleExpressionEvaluator;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.LanguageTests
{
	[TestFixture]
	[SetCulture("de-DE")]
	public class StringConcat
	{
		[Test]
		public void Concat_Two_Strings ()
		{
			string expression = "'Sharp' + 'Develop'";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo("SharpDevelop"));
		}
		
		[Test]
		public void Concat_Three_Strings ()
		{
			string expression = "'Sharp' + 'Develop' + 'Reporting'";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo("SharpDevelopReporting"));
		}
		
		[Test]
		public void Concat_String_UserId ()
		{
			string expression = "'Hello:' + User!UserId";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo("Hello:" + Environment.UserName));
		}
		
		[Test]
		public void Concat_UserId_String ()
		{
			string expression = "User!Language + ' is my motherlanguage'";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo("deu is my motherlanguage"));
		}
			
		[Test]
		public void Concat_String_UserId_String ()	
		{
			string expression = "'a' + User!Language + 'b'";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo("adeub"));
		}
		
		[Test]
		public void Concat_String_UserId_String_UserLan ()	
		{
			string expression = "'a' + User!Language + 'b' + User!Language";
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo("adeubdeu"));
		}
		
		
		[Test]
		public void Concat_String_FuntResult ()	
		{
			string expression = "'peter = ' + (5 * 10)";
				
			var compiler = new ReportingLanguageCompiler();
			IExpression compiled = compiler.CompileExpression<object>(expression);
			Assert.That(compiled.Evaluate(null), Is.EqualTo("peter = 50"));
		}
	}
}

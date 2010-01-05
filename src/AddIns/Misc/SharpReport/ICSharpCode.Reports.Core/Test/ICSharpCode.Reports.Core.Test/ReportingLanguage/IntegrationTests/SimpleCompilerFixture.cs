/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 10.06.2009
 * Zeit: 19:24
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;
using SimpleExpressionEvaluator;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage
{
	
	[TestFixture]
	public class SimpleCompilerFixture
	{
		
		private IExpressionEvaluatorFacade evaluator;
		
		#region Empty and null Expressions 
		
		[Test]
		public void Can_Handle_Empty_Expression()
		{
			const string expression = "";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(String.Empty));
		}
		
		
		[Test]
		public void Can_Handle_Empty_Expression_After_Cleanup()
		{
			const string expression = "= ";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("= "));
		}
		
		
		[Test]
		public void Can_Handle_Null_Expression()
		{
			const string expression = null;			
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(null));
		}
		
		#endregion
		
		#region SyntaxError
		
		[Test]
		public void Can_Handle_Wrong_Syntax_Returns_Expression()
		{
			const string expression = "User!hh";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("User!hh"));
		}
		
		
		#endregion
		
		#region Simple Literals
		
		[Test]
		public void Can_Compile_Simple_Number()
		{
			const string expression = " =1.1";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(expression));
		}
		
		
		[Test]
		public void Can_Compile_Simple_String()
		{
			const string expression = "'SharpReport'";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(expression));
		}
		
		#endregion
		
		#region Setup
		
		[SetUp]
		public void Init()
		{
			this.evaluator = new ExpressionEvaluatorFacade();
		}
		
		#endregion
	}
	
}

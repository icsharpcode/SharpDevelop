/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 12.06.2009
 * Zeit: 19:49
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;
using SimpleExpressionEvaluator;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.IntegrationTests
{
	
	[TestFixture]
	public class UserFunctionsFixture
	{
		private ReportingLanguageCompiler compiler;
		
		[Test]
		public void UserNameFunction ()
		{
			BaseTextItem bti = new BaseTextItem();
			bti.Text = "=User!UserID";			
			IExpression compiled = this.compiler.CompileExpression<object>(bti.Text);
			Assert.That(compiled.Evaluate(null), Is.EqualTo(Environment.UserName));
		}
		
		
		[Test]
		public void UserLanguageFunction ()
		{
			BaseTextItem bti = new BaseTextItem();
			bti.Text = "=User!Language";
			IExpression compiled = this.compiler.CompileExpression<object>(bti.Text);
			Assert.That(compiled.Evaluate(null), Is.EqualTo(System.Globalization.CultureInfo.CurrentCulture.ThreeLetterISOLanguageName));
		}
		
		
		#region Setup/Teardown
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.compiler = new ReportingLanguageCompiler();
		}
		#endregion
		
	}
}

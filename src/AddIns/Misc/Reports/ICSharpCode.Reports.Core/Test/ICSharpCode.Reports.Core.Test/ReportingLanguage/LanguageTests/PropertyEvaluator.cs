/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 07.06.2009
 * Zeit: 13:35
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;
using SimpleExpressionEvaluator;
using SimpleExpressionEvaluator.Evaluation;
using SimpleExpressionEvaluator.Utilities;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.LanguageTests
{
	[TestFixture]
	[SetCulture("de-DE")]
	public class PropertyEvaluator
	{
		
		[Test]
        public void Can_Compile_Uknown_Variable()
        {
            const string expression = "varName";
            var compiler = new ReportingLanguageCompiler();
            IExpression compiled = compiler.CompileExpression<string>(expression);

            var context = new ExpressionContext(null);
            context.ResolveUnknownVariable += (sender, args) =>
                                                  {
                                                      Assert.That(args.VariableName, Is.EqualTo("varName"));
                                                      args.VariableValue = 123.456;
                                                  };
            Assert.That(compiled.Evaluate(context), Is.EqualTo("123,456"));

        }
		
		
		[Test]
		 public void PropertyPathWillEvaluateFieldsAndProperties()
		 {
		 	var data = TestHelper.CreateSinglePage();
		 	PropertyPath path = data.ParsePropertyPath("PageNumber");
		 	Assert.That(path.Evaluate(data), Is.EqualTo(15));
		 }
	}
}

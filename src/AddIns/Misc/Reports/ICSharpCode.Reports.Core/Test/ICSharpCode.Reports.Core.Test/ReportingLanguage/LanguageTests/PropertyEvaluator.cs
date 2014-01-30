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

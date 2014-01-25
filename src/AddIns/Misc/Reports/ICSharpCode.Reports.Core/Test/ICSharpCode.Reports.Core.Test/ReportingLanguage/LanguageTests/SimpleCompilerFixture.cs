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
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Evaluation;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.LanguageTests
{
	[TestFixture]
	public class SimpleCompilerFixture
	{
		
		#region Simple Literals
		
		[Test]
        public void Can_Compile_Simple_Number()
        {
            const string expression = "1.1";
            var compiler = new ReportingLanguageCompiler();;
            IExpression compiled = compiler.CompileExpression<double>(expression);
            Assert.That(compiled.Evaluate(null), Is.EqualTo(1.1));
        }
		
		[Test]
        public void Can_Compile_Simple_String()
        {
            const string expression = "'SharpReport'";
            var compiler = new ReportingLanguageCompiler();
            IExpression compiled = compiler.CompileExpression<string>(expression);
            Assert.That(compiled.Evaluate(null), Is.EqualTo("SharpReport"));
        }
        
        #endregion
        
        #region bool
       
        [Test]
        public void Can_Compile_Simple_Bool()
        {
            const string expression = "true";
            var compiler = new ReportingLanguageCompiler();
            IExpression compiled = compiler.CompileExpression<bool>(expression);
            Assert.That(compiled.Evaluate(null), Is.EqualTo(true));
        }
        
        
        [Test]
        public void Can_Compile_Simple_String_As_bool()
        {
            const string expression = "'tRue'";
            var compiler = new ReportingLanguageCompiler();
            IExpression compiled = compiler.CompileExpression<bool>(expression);
            Assert.That(compiled.Evaluate(null), Is.EqualTo(true));
        }
        
        
        [Test]
        public void Can_Compile_Unary_Expression()
        {
            const string expression = "!true";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<bool>(expression);
            Assert.That(expr.Evaluate(null), Is.False);
        }
        
        #endregion
        
        
        [Test]
        public void Can_Compile_Simple_Variable()
        {
            const string expression = "varName";
            var compiler = new ReportingLanguageCompiler();
            IExpression compiled = compiler.CompileExpression<string>(expression);
            Assert.That(compiled.Evaluate(null), Is.EqualTo("varName"));
        }
        
        #region Qualified_Name
        
        [Test]
        public void Can_Compile_Qualified_Name()
        {
        	const string expression = "hello.goodbye.darlin.boy";
        	
        	var compiler = new ReportingLanguageCompiler();
        	IExpression expr = compiler.CompileExpression<string>(expression);
        	var context = new ExpressionContext(null);
        	context.ResolveUnknownQualifiedName += (sender, args) =>
        	{
        		Assert.That(args.QualifiedName.Length, Is.EqualTo(4));
        		Assert.That(args.QualifiedName[0], Is.EqualTo("hello"));
        		Assert.That(args.QualifiedName[1], Is.EqualTo("goodbye"));
        		Assert.That(args.QualifiedName[2], Is.EqualTo("darlin"));
        		Assert.That(args.QualifiedName[3], Is.EqualTo("boy"));
        		args.Value = "GitErDone";
        	};
        	Assert.That(expr.Evaluate(context), Is.EqualTo("GitErDone"));
        }
        
        
        [Test]
        
        public void Will_Treat_Identifier_With_Pound_Prefix_As_Qualified_Name()
        {
            const string expression = "#test";
            var compiler = new ReportingLanguageCompiler();
            IExpression expr = compiler.CompileExpression<string>(expression);
            var name = ((Expression<string>) expr).Root as QualifiedName;
            Assert.IsNotNull(name);
            Assert.That(name.Name.Length, Is.EqualTo(1));
            Assert.That(name.Name[0], Is.EqualTo("test")); 
        }
        
        #endregion
	}
}

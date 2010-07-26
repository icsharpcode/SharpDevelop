/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 30.05.2009
 * Zeit: 19:10
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

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

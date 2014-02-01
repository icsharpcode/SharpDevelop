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
using System.Diagnostics;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using NUnit.Framework;
using Ast = ICSharpCode.NRefactory.Ast;

namespace SharpRefactoring.Tests
{
	[TestFixture]
	[Ignore]
	public class IntroduceMethodTests
	{
		string simpleStart = @"
public class TestClass {
	void FirstMethod() {
		";

		string simpleEnd = @"
	}
}
		";
		
		string otherClassStart = @"
class Test2 {
";
		
		string otherClassEnd = @"
}
";
		
		string interfaceStart = @"
interface ITest {
";

		string interfaceEnd = @"
}
";
		
		MockTextEditor editor;
		IExpressionFinder expressionFinder;
		
		[SetUp]
		[STAThread]
		public void SetupTests()
		{
			editor = new MockTextEditor();
		}
		
		#region SimpleTests
		[Test]
		[STAThread]
		public void SimpleVoidTest()
		{
			string expected = @"
	}
	
	void SomeCall()
	{
		throw new NotImplementedException();";
			
			RunSimpleTest("", "SomeCall()", expected);
		}
		
		[Test]
		[STAThread]
		public void SimpleArgsTest1()
		{
			string expected = @"
	}
	
	void SomeCall(int num0)
	{
		throw new NotImplementedException();";
			
			RunSimpleTest("", "SomeCall(2)", expected);
		}
		
		[Test]
		[STAThread]
		public void SimpleArgsTest2()
		{
			string expected = @"
	}
	
	void SomeCall(int num0, char char1)
	{
		throw new NotImplementedException();";
			
			RunSimpleTest("", "SomeCall(2, 'c')", expected);
		}
		#endregion
		
		#region OtherClassTests
		[Test]
		[STAThread]
		public void SimpleOtherClassTest()
		{
			string expected = @"
	
	public static void SomeCall()
	{
		throw new NotImplementedException();
	}";
			
			RunOtherTest("", "Test2.SomeCall()", expected, "");
		}
		#endregion
		
		#region InterfaceTests
		[Test]
		[STAThread]
		public void EmptyInterfaceTest()
		{
			string expected = @"
	
	void SomeCall();";
			
			RunInterfaceTest("ITest test;", "test.SomeCall()", expected, "");
		}
		
		[Test]
		[STAThread]
		public void SimpleInterfaceTest()
		{
			string expected = @"
	
	void SomeCall2();";
			
			RunInterfaceTest("ITest test;", "test.SomeCall2()", expected, @"	void SomeCall();");
		}
		#endregion
		
		void RunSimpleTest(string definitions, string call, string expected)
		{
			editor.Document.Text = simpleStart + definitions + call + simpleEnd;
			editor.Caret.Offset = simpleStart.Length + definitions.Length + 2;
			
			editor.CreateParseInformation();
			
			expressionFinder = new CSharpExpressionFinder(ParserService.GetParseInformation(editor.FileName));
			
			ExpressionResult expression = FindFullExpressionAtCaret(editor, expressionFinder);
			ResolveResult rr = ResolveExpressionAtCaret(editor, expression);
			
			//Ast.Expression ex = GenerateCode.GetExpressionInContext(rr as UnknownMethodResolveResult, editor);
			
			var introduceMethodAction = GenerateCode.GetContextAction(new EditorContext(editor));
			Assert.IsNotNull(introduceMethodAction);
			introduceMethodAction.Execute();// .ExecuteIntroduceMethod(rr as UnknownMethodResolveResult, ex, editor, false, null);
			
			Assert.AreEqual(simpleStart + definitions + call + expected + simpleEnd, editor.Document.Text);
		}
		
		void RunOtherTest(string definitions, string call, string expected, string existingDefinitions)
		{
			editor.Document.Text = otherClassStart + existingDefinitions + otherClassEnd + simpleStart + definitions + call + simpleEnd;
			editor.Caret.Offset = otherClassStart.Length + existingDefinitions.Length + otherClassEnd.Length + simpleStart.Length + definitions.Length + call.Length / 2;
			
			var line = editor.Document.GetLineForOffset(editor.Caret.Offset);
			
			Debug.Print("line: '" + line.Text + "'");
			
			editor.CreateParseInformation();
			
			expressionFinder = new CSharpExpressionFinder(ParserService.GetParseInformation(editor.FileName));
			
			ExpressionResult expression = FindFullExpressionAtCaret(editor, expressionFinder);
			ResolveResult rr = ResolveExpressionAtCaret(editor, expression);
			
			var introduceMethodAction = GenerateCode.GetContextAction(new EditorContext(editor));
			Assert.IsNotNull(introduceMethodAction);
			introduceMethodAction.Execute(); //ExecuteIntroduceMethod(rr as UnknownMethodResolveResult, ex, editor, false, null);
			
			Assert.AreEqual(otherClassStart + existingDefinitions + expected + otherClassEnd + simpleStart + definitions + call + simpleEnd, editor.Document.Text);
		}
		
		void RunInterfaceTest(string definitions, string call, string expected, string existingDefinitions)
		{
			editor.Document.Text = interfaceStart + existingDefinitions + interfaceEnd + simpleStart + definitions + call + simpleEnd;
			editor.Caret.Offset = interfaceStart.Length + existingDefinitions.Length + interfaceEnd.Length + simpleStart.Length + definitions.Length + call.Length / 2;
			
			var line = editor.Document.GetLineForOffset(editor.Caret.Offset);
			
			Debug.Print("line: '" + line.Text + "'");
			
			editor.CreateParseInformation();
			
			expressionFinder = new CSharpExpressionFinder(ParserService.GetParseInformation(editor.FileName));
			
			ExpressionResult expression = FindFullExpressionAtCaret(editor, expressionFinder);
			ResolveResult rr = ResolveExpressionAtCaret(editor, expression);
			
			var introduceMethodAction = GenerateCode.GetContextAction(new EditorContext(editor));
			Assert.IsNotNull(introduceMethodAction);
			introduceMethodAction.Execute(); //ExecuteIntroduceMethod(rr as UnknownMethodResolveResult, ex, editor, false, null);
			
			Assert.AreEqual(interfaceStart + existingDefinitions + expected + interfaceEnd + simpleStart + definitions + call + simpleEnd, editor.Document.Text);
		}
		
		void RunExtensionMethodTest()
		{
			
		}
		
		#region Helpers
		static ExpressionResult FindFullExpressionAtCaret(ITextEditor textArea, IExpressionFinder expressionFinder)
		{
			if (expressionFinder != null) {
				return expressionFinder.FindFullExpression(textArea.Document.Text, textArea.Caret.Offset);
			} else {
				return new ExpressionResult(null);
			}
		}
		
		static IResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
		
		static ResolveResult ResolveExpressionAtCaret(ITextEditor textArea, ExpressionResult expressionResult)
		{
			if (expressionResult.Expression != null) {
				if (expressionResult.Region.IsEmpty) {
					expressionResult.Region = new DomRegion(textArea.Caret.Line, textArea.Caret.Column);
				}
				return resolver.Resolve(expressionResult, ParserService.GetParseInformation(textArea.FileName), textArea.Document.Text);
			}
			return null;
		}
		#endregion
	}
}

// 
// TestFormattingVisitor.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
/* 
using System;
using NUnit.Framework;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.CSharp.Parser;
using MonoDevelop.CSharp.Resolver;
using MonoDevelop.CSharp.Completion;
using Mono.TextEditor;
using MonoDevelop.CSharp.Formatting;

namespace MonoDevelop.CSharpBinding.FormattingTests
{
	[TestFixture()]
	public class TestSpacingVisitor : UnitTests.TestBase
	{
		[Test()]
		public void TestFieldSpacesBeforeComma1 ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	int a           ,                   b,          c;
}";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.ClassBraceStyle =  BraceStyle.EndOfLine;
			policy.SpacesAfterComma = false;
			policy.SpacesBeforeComma = false;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			Assert.AreEqual (@"class Test {
	int a,b,c;
}", data.Document.Text);
		}
		
		[Test()]
		public void TestFieldSpacesBeforeComma2 ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	int a           ,                   b,          c;
}";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.ClassBraceStyle =  BraceStyle.EndOfLine;
			policy.SpacesAfterComma = true;
			policy.SpacesBeforeComma = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			Assert.AreEqual (@"class Test {
	int a , b , c;
}", data.Document.Text);
		}
		
		[Test()]
		public void TestFixedFieldSpacesBeforeComma ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	fixed int a[10]           ,                   b[10],          c[10];
}";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.ClassBraceStyle =  BraceStyle.EndOfLine;
			policy.SpacesAfterComma = true;
			policy.SpacesBeforeComma = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			Assert.AreEqual (@"class Test {
	fixed int a[10] , b[10] , c[10];
}", data.Document.Text);
		}

		[Test()]
		public void TestConstFieldSpacesBeforeComma ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	const int a = 1           ,                   b = 2,          c = 3;
}";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.ClassBraceStyle =  BraceStyle.EndOfLine;
			policy.SpacesAfterComma = false;
			policy.SpacesBeforeComma = false;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			Assert.AreEqual (@"class Test {
	const int a = 1,b = 2,c = 3;
}", data.Document.Text);
		}
		
		[Test()]
		public void TestSpaceBeforeDelegateDeclarationParentheses ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = "delegate void TestDelegate();";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeDelegateDeclarationParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			Assert.AreEqual (@"delegate void TestDelegate ();", data.Document.Text);
		}
		
		[Test()]
		public void TestSpaceBeforeDelegateDeclarationParenthesesComplex ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = "delegate void TestDelegate\n\t\t\t();";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeDelegateDeclarationParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			Assert.AreEqual (@"delegate void TestDelegate ();", data.Document.Text);
		}
		
		[Test()]
		public void TestBeforeMethodDeclarationParentheses ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"public abstract class Test
{
	public abstract Test TestMethod();
}";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.BeforeMethodDeclarationParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			Console.WriteLine (data.Document.Text);
			Assert.AreEqual (@"public abstract class Test
{
	public abstract Test TestMethod ();
}", data.Document.Text);
		}
		
		[Test()]
		public void TestSpaceBeforeConstructorDeclarationParentheses ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test
{
	Test()
	{
	}
}";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeConstructorDeclarationParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			Assert.AreEqual (@"class Test
{
	Test ()
	{
	}
}", data.Document.Text);
		}
		
		
		
		[Test()]
		public void TestSpaceBeforeConstructorDeclarationParenthesesDestructorCase ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test
{
	~Test()
	{
	}
}";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeConstructorDeclarationParentheses = true;
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			Assert.AreEqual (@"class Test
{
	~Test ()
	{
	}
}", data.Document.Text);
		}
		
		
		
		static void TestBinaryOperator (CSharpFormattingPolicy policy, string op)
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = "class Test { void TestMe () { result = left" +op+"right; } }";
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("left");
			int i2 = data.Document.Text.IndexOf ("right") + "right".Length;
			if (i1 < 0 || i2 < 0)
				Assert.Fail ("text invalid:" + data.Document.Text);
			Assert.AreEqual ("left " + op + " right", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesAroundMultiplicativeOperator ()
		{
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAroundMultiplicativeOperator = true;
			
			TestBinaryOperator (policy, "*");
			TestBinaryOperator (policy, "/");
		}
		
		[Test()]
		public void TestSpacesAroundShiftOperator ()
		{
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAroundShiftOperator = true;
			TestBinaryOperator (policy, "<<");
			TestBinaryOperator (policy, ">>");
		}
		
		[Test()]
		public void TestSpacesAroundAdditiveOperator ()
		{
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAroundAdditiveOperator = true;
			
			TestBinaryOperator (policy, "+");
			TestBinaryOperator (policy, "-");
		}
		
		[Test()]
		public void TestSpacesAroundBitwiseOperator ()
		{
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAroundBitwiseOperator = true;
			
			TestBinaryOperator (policy, "&");
			TestBinaryOperator (policy, "|");
			TestBinaryOperator (policy, "^");
		}
		
		[Test()]
		public void TestSpacesAroundRelationalOperator ()
		{
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAroundRelationalOperator = true;
			
			TestBinaryOperator (policy, "<");
			TestBinaryOperator (policy, "<=");
			TestBinaryOperator (policy, ">");
			TestBinaryOperator (policy, ">=");
		}
		
		[Test()]
		public void TestSpacesAroundEqualityOperator ()
		{
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAroundEqualityOperator = true;
			
			TestBinaryOperator (policy, "==");
			TestBinaryOperator (policy, "!=");
		}
		
		[Test()]
		public void TestSpacesAroundLogicalOperator ()
		{
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAroundLogicalOperator = true;
			
			TestBinaryOperator (policy, "&&");
			TestBinaryOperator (policy, "||");
		}
		
		[Test()]
		public void TestConditionalOperator ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		result = condition?trueexpr:falseexpr;
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAfterConditionalOperatorCondition = true;
			policy.SpaceAfterConditionalOperatorSeparator = true;
			policy.SpaceBeforeConditionalOperatorCondition = true;
			policy.SpaceBeforeConditionalOperatorSeparator = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			
			int i1 = data.Document.Text.IndexOf ("condition");
			int i2 = data.Document.Text.IndexOf ("falseexpr") + "falseexpr".Length;
			Assert.AreEqual (@"condition ? trueexpr : falseexpr", data.Document.GetTextBetween (i1, i2));
			
			
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		result = true ? trueexpr : falseexpr;
	}
}";
			policy.SpaceAfterConditionalOperatorCondition = false;
			policy.SpaceAfterConditionalOperatorSeparator = false;
			policy.SpaceBeforeConditionalOperatorCondition = false;
			policy.SpaceBeforeConditionalOperatorSeparator = false;
			
			compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			i1 = data.Document.Text.IndexOf ("true");
			i2 = data.Document.Text.IndexOf ("falseexpr") + "falseexpr".Length;
			Assert.AreEqual (@"true?trueexpr:falseexpr", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpaceBeforeMethodCallParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		MethodCall();
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeMethodCallParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("MethodCall");
			int i2 = data.Document.Text.IndexOf (";") + ";".Length;
			Assert.AreEqual (@"MethodCall ();", data.Document.GetTextBetween (i1, i2));
			
			
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		MethodCall                         				();
	}
}";
			policy.SpaceBeforeMethodCallParentheses = false;
			
			compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			i1 = data.Document.Text.IndexOf ("MethodCall");
			i2 = data.Document.Text.IndexOf (";") + ";".Length;
			Assert.AreEqual (@"MethodCall();", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpaceWithinMethodCallParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		MethodCall(true);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceWithinMethodCallParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( true )", data.Document.GetTextBetween (i1, i2));
			
			
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		MethodCall( true );
	}
}";
			policy.SpaceWithinMethodCallParentheses = false;
			
			compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			i1 = data.Document.Text.LastIndexOf ("(");
			i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"(true)", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestBeforeSpaceBeforeIfParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		if(true);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeIfParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("if");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"if (true)", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinIfParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		if (true);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinIfParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( true )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestBeforeSpaceBeforeWhileParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		while(true);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeWhileParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("while");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"while (true)", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinWhileParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		while (true);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinWhileParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( true )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestBeforeSpaceBeforeForParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		for(;;);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeForParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("for");
			int i2 = data.Document.Text.LastIndexOf ("(") + "(".Length;
			Assert.AreEqual (@"for (", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinForParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		for(;;);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinForParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( ;; )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestBeforeSpaceBeforeForeachParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		foreach(var o in list);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeForeachParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("foreach");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"foreach (var o in list)", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinForeachParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		foreach(var o in list);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinForeachParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( var o in list )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestBeforeSpaceBeforeCatchParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
  try {} catch(Exception) {}
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeCatchParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("catch");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"catch (Exception)", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinCatchParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		try {} catch(Exception) {}
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinCatchParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( Exception )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestBeforeSpaceBeforeLockParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		lock(this) {}
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeLockParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("lock");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"lock (this)", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinLockParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		lock(this) {}
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinLockParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( this )", data.Document.GetTextBetween (i1, i2));
		}
		
		
		[Test()]
		public void TestSpacesAfterSemicolon ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		for (int i;true;i++) ;
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesAfterSemicolon = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("for");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			
			Assert.AreEqual (@"for (int i; true; i++)", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesAfterTypecast ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	Test TestMe ()
	{
return (Test)null;
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAfterTypecast = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("return");
			int i2 = data.Document.Text.LastIndexOf ("null") + "null".Length;
			
			Assert.AreEqual (@"return (Test) null", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestBeforeSpaceBeforeUsingParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		using(a) {}
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeUsingParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("using");
			int i2 = data.Document.Text.LastIndexOf ("(") + "(".Length;
			Assert.AreEqual (@"using (", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinUsingParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		using(a) {}
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinUsingParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( a )", data.Document.GetTextBetween (i1, i2));
		}
		
		static void TestAssignmentOperator (CSharpFormattingPolicy policy, string op)
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = "class Test { void TestMe () { left" +op+"right; } }";
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("left");
			int i2 = data.Document.Text.IndexOf ("right") + "right".Length;
			if (i1 < 0 || i2 < 0)
				Assert.Fail ("text invalid:" + data.Document.Text);
			Assert.AreEqual ("left " + op + " right", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpaceAroundAssignmentSpace ()
		{
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAroundAssignmentParentheses = true;
			
			TestAssignmentOperator (policy, "=");
			TestAssignmentOperator (policy, "*=");
			TestAssignmentOperator (policy, "/=");
			TestAssignmentOperator (policy, "+=");
			TestAssignmentOperator (policy, "%=");
			TestAssignmentOperator (policy, "-=");
			TestAssignmentOperator (policy, "<<=");
			TestAssignmentOperator (policy, ">>=");
			TestAssignmentOperator (policy, "&=");
			TestAssignmentOperator (policy, "|=");
			TestAssignmentOperator (policy, "^=");
		}
		
		[Test()]
		public void TestSpaceAroundAssignmentSpaceInDeclarations ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		int left=right;
	}
}";
			
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceAroundAssignmentParentheses = true;
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("left");
			int i2 = data.Document.Text.LastIndexOf ("right") + "right".Length;
			Assert.AreEqual (@"left = right", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestBeforeSpaceBeforeSwitchParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		switch (test) { default: break; }
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeSwitchParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("switch");
			int i2 = data.Document.Text.LastIndexOf ("(") + "(".Length;
			Assert.AreEqual (@"switch (", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinSwitchParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		switch (test) { default: break; }
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinSwitchParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( test )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		c = (test);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( test )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpaceWithinMethodDeclarationParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe (int a)
	{
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceWithinMethodDeclarationParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( int a )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinCastParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		a = (int)b;
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinCastParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( int )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinSizeOfParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		a = sizeof(int);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinSizeOfParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( int )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinTypeOfParenthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		a = typeof(int);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinTypeOfParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( int )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestWithinCheckedExpressionParanthesesSpace ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		a = checked(a + b);
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinCheckedExpressionParantheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("(");
			int i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( a + b )", data.Document.GetTextBetween (i1, i2));
			
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		a = unchecked(a + b);
	}
}";
			
			compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			i1 = data.Document.Text.LastIndexOf ("(");
			i2 = data.Document.Text.LastIndexOf (")") + ")".Length;
			Assert.AreEqual (@"( a + b )", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesWithinBrackets ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	Test this[int i] {
		get {}
		set {}
	}
	
	void TestMe ()
	{
		this[0] = 5;
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinBrackets = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			
			Assert.AreEqual (@"class Test {
	Test this[ int i ] {
		get {}
		set {}
	}
	
	void TestMe ()
	{
		this[ 0 ] = 5;
	}
}", data.Document.Text);
			
			policy.SpacesWithinBrackets = false;
			
			compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			
			Assert.AreEqual (@"class Test {
	Test this[int i] {
		get {}
		set {}
	}
	
	void TestMe ()
	{
		this[0] = 5;
	}
}", data.Document.Text);
		}
		
		
		[Test()]
		public void TestSpaceBeforeSpaceBeforeNewParentheses ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		new Test();
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpaceBeforeNewParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("new");
			int i2 = data.Document.Text.LastIndexOf (";") + ";".Length;
			Assert.AreEqual (@"new Test ();", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesBeforeComma ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		int[] i = new int[] { 1,3,3,7 };
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesBeforeComma = true;
			policy.SpacesAfterComma = false;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("new");
			int i2 = data.Document.Text.LastIndexOf (";") + ";".Length;
			Assert.AreEqual (@"new int[] { 1 ,3 ,3 ,7 };", data.Document.GetTextBetween (i1, i2));
			policy.SpacesBeforeComma = false;
			
			compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			i1 = data.Document.Text.LastIndexOf ("new");
			i2 = data.Document.Text.LastIndexOf (";") + ";".Length;
			Assert.AreEqual (@"new int[] { 1,3,3,7 };", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesAfterComma ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		int[] i = new int[] { 1,3,3,7 };
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesBeforeComma = false;
			policy.SpacesAfterComma = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.LastIndexOf ("new");
			int i2 = data.Document.Text.LastIndexOf (";") + ";".Length;
			Assert.AreEqual (@"new int[] { 1, 3, 3, 7 };", data.Document.GetTextBetween (i1, i2));
			policy.SpacesAfterComma = false;
			
			compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			i1 = data.Document.Text.LastIndexOf ("new");
			i2 = data.Document.Text.LastIndexOf (";") + ";".Length;
			Assert.AreEqual (@"new int[] { 1,3,3,7 };", data.Document.GetTextBetween (i1, i2));
		}
		
		[Test()]
		public void TestSpacesInLambdaExpression ()
		{
			TextEditorData data = new TextEditorData ();
			data.Document.FileName = "a.cs";
			data.Document.Text = @"class Test {
	void TestMe ()
	{
		var v = x=>x!=null;
	}
}";
			CSharpFormattingPolicy policy = new CSharpFormattingPolicy ();
			policy.SpacesWithinWhileParentheses = true;
			
			CSharp.Dom.CompilationUnit compilationUnit = new CSharpParser ().Parse (data);
			compilationUnit.AcceptVisitor (new DomSpacingVisitor (policy, data), null);
			int i1 = data.Document.Text.IndexOf ("x");
			int i2 = data.Document.Text.LastIndexOf ("null") + "null".Length;
			Assert.AreEqual (@"x => x != null", data.Document.GetTextBetween (i1, i2));
		}
		
	}
}
 */
//
// TestStatementIndentation.cs
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
using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.CSharp.FormattingTests
{
	[TestFixture()]
	public class TestStatements : TestBase
	{
		[Test]
		public void TestInvocationIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy,
			     @"class Test {
	Test TestMethod ()
	{
this.TestMethod ();
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		this.TestMethod ();
	}
}");
		}

		[Test]
		public void TestIndentBlocks()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentBlocks = true;
			
			var adapter = Test(policy,
			                   @"class Test {
	Test TestMethod ()
	{
{
{}
}
	}
}",
			                   @"class Test
{
	Test TestMethod ()
	{
		{
			{}
		}
	}
}");
			policy.IndentBlocks = false;
			Continue(policy, adapter, @"class Test
{
	Test TestMethod ()
	{
		{
		{}
		}
	}
}");
		}

		[Test]
		public void TestIndentBlocksCase2()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentBlocks = true;
			
			var adapter = Test(policy,
			                   @"class Test {
	Test TestMethod ()
	{
		if (true) {
		Something ();
		}
	}
}",
			                   @"class Test
{
	Test TestMethod ()
	{
		if (true) {
			Something ();
		}
	}
}");
			policy.IndentBlocks = false;
			Continue(policy, adapter, @"class Test
{
	Test TestMethod ()
	{
		if (true) {
		Something ();
		}
	}
}");
		}

		[Test]
		public void TestBreakIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, 
			     @"class Test {
	Test TestMethod ()
	{
                              break;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		break;
	}
}");
		}

		[Test]
		public void TestBreakSemicolon()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, 
			     @"class Test
{
	Test TestMethod ()
	{
		break     ;
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		break;
	}
}");
		}

		[Test]
		public void TestCheckedIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
checked {
}
	}
}", @"class Test {
	Test TestMethod ()
	{
		checked {
		}
	}
}");
		}

		[Test]
		public void TestBaseIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
                              base.FooBar();
	}
}", @"class Test {
	Test TestMethod ()
	{
		base.FooBar ();
	}
}");
		}

		[Test]
		public void TestUncheckedIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
unchecked {
}
	}
}", 
			     @"class Test {
	Test TestMethod ()
	{
		unchecked {
		}
	}
}");
		}

		[Test]
		public void TestContinueIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
continue;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		continue;
	}
}");
		}

		[Test]
		public void TestContinueSemicolon()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		continue ;
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		continue;
	}
}");
		}

		[Test]
		public void TestEmptyStatementIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
;
	}
}", @"class Test {
	Test TestMethod ()
	{
		;
	}
}");
		}

		[Test]
		public void TestFixedStatementIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
fixed (object* obj = &obj)
;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		fixed (object* obj = &obj)
			;
	}
}");
		}

		[Test]
		public void TestForeachIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
foreach (var obj in col) {
}
	}
}", @"class Test
{
	Test TestMethod ()
	{
		foreach (var obj in col) {
		}
	}
}");
		}

		[Test]
		public void TestForIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
for (;;) {
}
	}
}", @"class Test {
	Test TestMethod ()
	{
		for (;;) {
		}
	}
}");
		}

		[Test]
		public void TestForInitializerIteratorConditionFormatting ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy, @"
class Test
{
	void Init ()
	{
		for (int i     =        12; i <           10; i               ++) {
		}
	}
}", @"
class Test
{
	void Init ()
	{
		for (int i = 12; i < 10; i++) {
		}
	}
}");
		}



		[Test]
		public void TestGotoIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
goto label;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		goto label;
	}
}");
		}

		[Test]
		public void TestGotoSemicolon()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		goto label
;
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		goto label;
	}
}");
		}

		[Test]
		public void TestReturnIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
		return;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		return;
	}
}");
		}

		[Test]
		public void TestReturnSemicolon()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		return ;
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		return;
	}
}");
		}

		[Test]
		public void TestLockIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
lock (this) {
}
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		lock (this) {
		}
	}
}");
		}

		[Test]
		public void TestThrowIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
throw new NotSupportedException ();
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		throw new NotSupportedException ();
	}
}");
		}

		[Test]
		public void TestThrowSemicolon()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		throw new NotSupportedException () 	 ;
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		throw new NotSupportedException ();
	}
}");
		}

		[Test]
		public void TestUnsafeIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
unsafe {
}
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		unsafe {
		}
	}
}");
		}

		[Test]
		public void TestUsingIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
using (var o = new MyObj ()) {
}
	}
}", @"class Test {
	Test TestMethod ()
	{
		using (var o = new MyObj ()) {
		}
	}
}");
		}

		[Test]
		public void TestUsingAlignment()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.AlignEmbeddedStatements = true;
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			var adapter = Test(policy, @"class Test {
	Test TestMethod ()
	{
using (var p = new MyObj ())
using (var o = new MyObj ()) {
}
	}
}",
			                   @"class Test {
	Test TestMethod ()
	{
		using (var p = new MyObj ())
		using (var o = new MyObj ()) {
		}
	}
}");
			policy.AlignEmbeddedStatements = false;
			Continue(policy, adapter, @"class Test {
	Test TestMethod ()
	{
		using (var p = new MyObj ())
			using (var o = new MyObj ()) {
			}
	}
}");
		}

		[Test]
		public void TestUsingExpressionFormatting ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy, @"
class Test
{
	void Init ()
	{
		using (var o = new object(              )) {
		}
	}
}", @"
class Test
{
	void Init ()
	{
		using (var o = new object ()) {
		}
	}
}");
		}



		[Test]
		public void TestVariableDeclarationIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
Test a;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		Test a;
	}
}");
		}

		[Test]
		public void TestConstantVariableDeclarationIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
const int a = 5;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		const int a = 5;
	}
}");
		}

		[Test]
		public void TestYieldReturnIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			Test(policy, @"class Test {
	Test TestMethod ()
	{
yield return null;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		yield return null;
	}
}");
		}

		[Test]
		public void TestYieldReturnSemicolon()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			Test(policy, @"class Test {
	Test TestMethod ()
	{
		yield return null     ;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		yield return null;
	}
}");
		}

		[Test]
		public void TestYieldBreakIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			Test(policy, @"class Test {
	Test TestMethod ()
	{
yield break;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		yield break;
	}
}");
		}

		[Test]
		public void TestYieldBreakSemicolon()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			Test(policy, @"class Test {
	Test TestMethod ()
	{
		yield break      ;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		yield break;
	}
}");
		}

		[Test]
		public void TestWhileIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test {
	Test TestMethod ()
	{
while (true)
;
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		while (true)
			;
	}
}");
		}

		[Test]
		public void TestDoWhileIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			Test(policy, @"class Test {
	Test TestMethod ()
	{
do {
} while (true);
	}
}",
			     @"class Test {
	Test TestMethod ()
	{
		do {
		} while (true);
	}
}");
		}

		[Test]
		public void TestDoWhileWithoutBrackets()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, @"class Test
{
	Test TestMethod ()
	{
do FooBar(); while (true);
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		do
			FooBar ();
		while (true);
	}
}");
		}

		[Test]
		public void TestForeachBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		foreach (var obj in col) {}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		foreach (var obj in col) {
		}
	}
}");
		}

		[Test]
		public void TestForeachBracketPlacement2()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.NextLineShifted2;
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		foreach (var obj in col) {;}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		foreach (var obj in col)
			{
				;
			}
	}
}");
		}

		[Test]
		public void TestIfBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		if (true) {}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		if (true) {
		}
	}
}");
		}

		[Test]
		public void TestAllowIfBlockInline()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			policy.AllowIfBlockInline = true;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		if (true) {}
	}
}", @"class Test
{
	Test TestMethod ()
	{
		if (true) {}
	}
}");
			
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		if (true) { Foo (); }
	}
}", @"class Test
{
	Test TestMethod ()
	{
		if (true) { Foo (); }
	}
}");
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		if (true)
			Foo ();
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		if (true)
			Foo ();
	}
}");
		}

		[Test]
		public void TestIfElseBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		if (true) {} else {}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		if (true) {
		} else {
		}
	}
}");
		}

		[Test]
		public void TestIfAlignment()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.AlignEmbeddedStatements = true;
			policy.ClassBraceStyle = BraceStyle.EndOfLine;
			
			var adapter = Test(policy, @"class Test {
	Test TestMethod ()
	{
if (a)
if (b) {
}
	}
}",
			                   @"class Test {
	Test TestMethod ()
	{
		if (a)
		if (b) {
		}
	}
}");
			policy.AlignEmbeddedStatements = false;
			Continue(policy, adapter, @"class Test {
	Test TestMethod ()
	{
		if (a)
			if (b) {
			}
	}
}");
		}

		[Test]
		public void TestLockAlignment()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.AlignEmbeddedStatements = true;
			policy.ClassBraceStyle = BraceStyle.EndOfLine;

			var adapter = Test(policy, @"class Test {
	Test TestMethod ()
	{
lock (a)
lock (b) {
}
	}
}",
				@"class Test {
	Test TestMethod ()
	{
		lock (a)
		lock (b) {
		}
	}
}");
			policy.AlignEmbeddedStatements = false;
			Continue(policy, adapter, @"class Test {
	Test TestMethod ()
	{
		lock (a)
			lock (b) {
			}
	}
}");
		}

		[Test]
		public void TestElseWithPreprocessorDirective()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			Test(policy, @"class Test
{
	void TestMethod ()
	{
		if (true) {
		}
		#if true
		else
			Console.WriteLine ();
				#endif
	}
}",
			     @"class Test
{
	void TestMethod ()
	{
		if (true) {
		}
		#if true
		else
			Console.WriteLine ();
		#endif
	}
}");
		}

		[Test]
		public void TestElseOnNewLine()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ElseNewLinePlacement = NewLinePlacement.NewLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		if (true) {
			;
		} else if (false) {
			;
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		if (true) {
			;
		}
		else if (false) {
			;
		}
	}
}");
		}

		[Test]
		public void TestElseIfOnNewLine()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ElseIfNewLinePlacement = NewLinePlacement.NewLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		if (true) {
			;
		} else if (false) {
			;
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		if (true) {
			;
		} else
		if (false) {
			;
		}
	}
}");
		}

		[Test]
		public void TestElseOnNewLineOff()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ElseNewLinePlacement = NewLinePlacement.SameLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		if (true) {
			;
		}
		else if (false) {
			;
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		if (true) {
			;
		} else if (false) {
			;
		}
	}
}");
		}

		[Test]
		public void TestSimpleIfElseComment()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			policy.ElseIfNewLinePlacement = NewLinePlacement.SameLine; // for simple statements it must be new line.
			
			Test(policy, @"class Test
{
	void TestMethod ()
	{
		if (true) Call (); else Call ();
	}
}",
			     @"class Test
{
	void TestMethod ()
	{
		if (true)
			Call ();
		else
			Call ();
	}
}");
		}

		[Test]
		public void TestIfElseIfCorrection()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateKRStyle();

			Test(policy, @"class Test
{
	void TestMethod()
	{
		int i = 0;
		if (i == 0) {
		} else
			if (i == 1) {
		} else
if (i == 2) {
		} else                     if (i == 2)	
		Foo();
		else {
		}
	}
}", @"class Test
{
	void TestMethod()
	{
		int i = 0;
		if (i == 0) {
		} else if (i == 1) {
		} else if (i == 2) {
		} else if (i == 2)
			Foo();
		else {
		}
	}
}");
		}

		[Test]
		public void TestIfElseNewLine()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateKRStyle();
			policy.ElseIfNewLinePlacement = NewLinePlacement.NewLine;
			Test(policy, @"class Test
{
	void TestMethod()
	{
		int i = 0;
		if (i == 0) {
		} else
			if (i == 1) {
		} else
if (i == 2) {
		} else                     if (i == 2)	
		Foo();
		else {
		}
	}
}", @"class Test
{
	void TestMethod()
	{
		int i = 0;
		if (i == 0) {
		} else
		if (i == 1) {
		} else
		if (i == 2) {
		} else
		if (i == 2)
			Foo();
		else {
		}
	}
}");
		}

		[Test]
		public void TestIfElseDoNotCare()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateKRStyle();
			policy.ElseIfNewLinePlacement = NewLinePlacement.DoNotCare;
			Test(policy, @"class Test
{
	void TestMethod()
	{
		int i = 0;
		if (i == 0) {
		} else
			if (i == 1) {
		} else
if (i == 2) {
		} else                     if (i == 2)	
		Foo();
		else {
		}
	}
}", @"class Test
{
	void TestMethod()
	{
		int i = 0;
		if (i == 0) {
		} else
		if (i == 1) {
		} else
		if (i == 2) {
		} else if (i == 2)
			Foo();
		else {
		}
	}
}");
		}

		[Test]
		public void TestFixedBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.NextLineShifted;

			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		fixed (object* obj = &obj){

;}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		fixed (object* obj = &obj)
			{

			;
			}
	}
}");
		}

		[Test]
		public void TestForBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLineWithoutSpace;
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		for (;;) {;}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		for (;;){
			;
		}
	}
}");
		}

		[Test]
		public void TestCheckedBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLineWithoutSpace;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		checked {;}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		checked{
			;
		}
	}
}");
		}

		[Test]
		public void TestUncheckedBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLineWithoutSpace;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		unchecked {;}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		unchecked{
			;
		}
	}
}");
		}

		[Test]
		public void TestLockBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		lock (this)
		{
			;
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		lock (this) {
			;
		}
	}
}");
		}

		[Test]
		public void TestUnsafeBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		unsafe
		{
			;
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		unsafe {
			;
		}
	}
}");
		}

		[Test]
		public void TestUsingBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		using (var e = new E ())
		{
			;
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		using (var e = new E ()) {
			;
		}
	}
}");
		}

		[Test]
		public void TestWhileBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		while (true)
		{
			;
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		while (true) {
			;
		}
	}
}");
		}

		[Test]
		public void TestDoWhileBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		do
		{
			;
		} while (true);
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		do {
			;
		} while (true);
	}
}");
		}

		[Test]
		public void TestSwitchFormatting1()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.IndentSwitchBody = true;
			policy.IndentCaseBody = true;
			policy.IndentBreakStatements = true;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		switch (a) { case 1: case 2: DoSomething(); break; default: Foo (); break;}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		switch (a) {
			case 1:
			case 2:
				DoSomething ();
				break;
			default:
				Foo ();
				break;
		}
	}
}");
		}

		[Test]
		public void TestSwitchFormatting2()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.IndentSwitchBody = false;
			policy.IndentCaseBody = false;
			policy.IndentBreakStatements = false;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		switch (a) { case 1: case 2: DoSomething(); break; default: Foo (); break;}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		switch (a) {
		case 1:
		case 2:
		DoSomething ();
		break;
		default:
		Foo ();
		break;
		}
	}
}");
		}

		[Test]
		public void TestSwitchIndentBreak()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.IndentSwitchBody = true;
			policy.IndentBreakStatements = true;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		switch (a) {
			case 1:
			case 2:
			DoSomething ();
			break;
			default:
			Foo ();
			break;
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		switch (a) {
			case 1:
			case 2:
				DoSomething ();
				break;
			default:
				Foo ();
				break;
		}
	}
}");
			policy.IndentSwitchBody = true;
			policy.IndentBreakStatements = false;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		switch (a) {
			case 1:
			case 2:
			DoSomething ();
			break;
			default:
			Foo ();
			break;
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		switch (a) {
			case 1:
			case 2:
				DoSomething ();
			break;
			default:
				Foo ();
			break;
		}
	}
}");
		}

		[Test]
		public void TestTryCatchBracketPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.StatementBraceStyle = BraceStyle.EndOfLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		try { ; } catch (Exception e) { } finally { }
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		try {
			;
		} catch (Exception e) {
		} finally {
		}
	}
}");
		}

		[Test]
		public void TestPlaceCatchOnNewLine()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.CatchNewLinePlacement = NewLinePlacement.NewLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		try {
			;
		} catch (Exception e) {
		} finally {
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		try {
			;
		}
		catch (Exception e) {
		} finally {
		}
	}
}");
		}

		[Test]
		public void TestPlaceFinallyOnNewLine()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.FinallyNewLinePlacement = NewLinePlacement.NewLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		try {
			;
		} catch (Exception e) {
		} finally {
		}
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		try {
			;
		} catch (Exception e) {
		}
		finally {
		}
	}
}");
		}

		[Test]
		public void TestPlaceWhileOnNewLine()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			policy.WhileNewLinePlacement = NewLinePlacement.NewLine;
			
			Test(policy, @"class Test
{
	Test TestMethod ()
	{
		do {
			;
		} while (true);
	}
}",
			     @"class Test
{
	Test TestMethod ()
	{
		do {
			;
		}
		while (true);
	}
}");
		}

		[Test]
		public void TestBlockStatementWithComments()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, @"class Test
{
	Test TestMethod ()
	{
{
 //CMT1
;
 /* cmt 2 */
}
	}
}", @"class Test
{
	Test TestMethod ()
	{
		{
			//CMT1
			;
			/* cmt 2 */
		}
	}
}");
		}

		[Test]
		public void TestBlockStatementWithPreProcessorDirective()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, @"class Test
{
	Test TestMethod ()
	{
{
" + @" #if true
;
" + @" #endif
}
	}
}", @"class Test
{
	Test TestMethod ()
	{
		{
" + @"			#if true
			;
" + @"			#endif
		}
	}
}");
		}

		[Test]
		public void TestUsingInsideNamespace()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.UsingPlacement = UsingPlacement.InsideNamespace;

			Test(policy, @"namespace TestNamespace
{
using System;

	class Test
	{
	}
}", @"namespace TestNamespace
{
	using System;

	class Test
	{
	}
}");
		}

		[Test]
		public void TestEmbeddedStatementPlacement()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.EmbeddedStatementPlacement = NewLinePlacement.SameLine;

			Test(policy, @"class Test
{
	void MyTest ()
	{
		if (true)                 ;
	}
}", @"class Test
{
	void MyTest ()
	{
		if (true) ;
	}
}");

			Test(policy, @"class Test
{
	void MyTest ()
	{
		if (true)
			;
	}
}", @"class Test
{
	void MyTest ()
	{
		if (true) ;
	}
}");
			policy.EmbeddedStatementPlacement = NewLinePlacement.NewLine;
			Test(policy, @"class Test
{
	void MyTest ()
	{
		if (true) ;
	}
}", @"class Test
{
	void MyTest ()
	{
		if (true)
			;
	}
}");
		}

		[Test]
		public void AlignmentTest_DoWhile()
		{
			var policy = FormattingOptionsFactory.CreateMono();

			Test(policy, @"
class Test
{
	void Foo ()
	{
		do {
		} while(a <
b);
	}
}
", @"
class Test
{
	void Foo ()
	{
		do {
		} while(a <
		        b);
	}
}
");
		}

		[Test]
		public void AlignmentTest_IfElse()
		{
			var policy = FormattingOptionsFactory.CreateMono();

			Test(policy, @"
class Test
{
	void Foo ()
	{
		if (a 
> b) {
		}
	}
}
", @"
class Test
{
	void Foo ()
	{
		if (a
		    > b) {
		}
	}
}
");
		}

		[Test]
		public void AlignmentTest_While()
		{
			var policy = FormattingOptionsFactory.CreateMono();

			Test(policy, @"
class Test
{
	void Foo()
	{
		while (a 
> b) {
		}
	}
}
", @"
class Test
{
	void Foo ()
	{
		while (a
		       > b) {
		}
	}
}
");
		}

		[Ignore("fixme")]
		[Test]
		public void AlignmentTest_StackedIfElse_ElseIf()
		{
			var policy = FormattingOptionsFactory.CreateMono();

			Test(policy, @"
class Foo
{
	void Test ()
	{ 
		if (true)
			FooBar ();
		else
		if (true) {
		}
	}
}
", @"
class Foo
{
	void Test ()
	{ 
		if (true)
			FooBar ();
		else 
			if (true) {
			}
	}
}
");
		}

		[Test]
		public void TestSpaceBeforeSemicolon()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			policy.SpaceBeforeSemicolon = true;
			Test(policy, @"using System;

class Test
{
	int field;

	event EventHandler foo;

	void Foo ()
	{
		int a = 2 + 3;
		Foo ();
		break;
		continue;
		goto foo;
		return;
		throw new Exception ();
		yield break;
		yield return 5;
		
	}
}
", @"using System ;

class Test
{
	int field ;

	event EventHandler foo ;

	void Foo ()
	{
		int a = 2 + 3 ;
		Foo () ;
		break ;
		continue ;
		goto foo ;
		return ;
		throw new Exception () ;
		yield break ;
		yield return 5 ;
		
	}
}
");
		}
	}
}

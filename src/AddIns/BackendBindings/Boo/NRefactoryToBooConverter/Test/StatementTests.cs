// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace NRefactoryToBooConverter.Tests
{
	[TestFixture]
	public class StatementTests : TestHelper
	{
		[Test]
		public void IfStatement()
		{
			TestStatement("if (a) B();", "if a:\n\tB()");
		}
		
		[Test]
		public void IfElseStatement()
		{
			TestStatement("if (a) B(); else C();", "if a:\n\tB()\nelse:\n\tC()");
		}
		
		[Test]
		public void IfElseIfElseStatement()
		{
			TestStatement("if (a) C(); else if (b) D(); else E();",
			              "if a:\n" +
			              "\tC()\n" +
			              "elif b:\n" +
			              "\tD()\n" +
			              "else:\n" +
			              "\tE()");
		}
		
		[Test]
		public void ForLoop()
		{
			TestStatement("for (int i = 0; i < 10; i++) {}", "for i in range(0, 10):\n\tpass");
		}
		
		[Test]
		public void ForLoopWithoutBody()
		{
			TestStatement("for (int i = 0; i < 10; i++);", "for i in range(0, 10):\n\tpass");
		}
		
		[Test]
		public void ForLoopWithStep()
		{
			TestStatement("for (int i = 0; i <= 10; i += 2);", "for i in range(0, 11, 2):\n\tpass");
		}
		
		[Test]
		public void ForLoopDecrementing()
		{
			TestStatement("for (int i = 10; i > 0; --i);", "for i in range(10, 0, -1):\n\tpass");
		}
		
		[Test]
		public void ForLoopDecrementingWithStep()
		{
			TestStatement("for (int i = 10; i >= 0; i -= 2);", "for i in range(10, -1, -2):\n\tpass");
		}
		
		[Test]
		public void ForLoop2()
		{
			TestStatement("for (i = 0; i < 10; i++) {}", "i = 0\ngoto ??1\nwhile true:\n\ti += 1\n\t:??1\n\tbreak unless (i <= 9)");
		}
		
		[Test]
		public void ForLoopWithStep2()
		{
			TestStatement("for (i = 0; i <= 10; i += 2);", "i = 0\ngoto ??1\nwhile true:\n\ti += 2\n\t:??1\n\tbreak unless (i <= 10)");
		}
		
		[Test]
		public void ForLoopDecrementing2()
		{
			TestStatement("for (i = 10; i > 0; --i);", "i = 10\ngoto ??1\nwhile true:\n\ti += -1\n\t:??1\n\tbreak unless (i >= 1)");
		}
		
		[Test]
		public void AdvancedForLoop()
		{
			TestStatement("for (f = Open(); (next = Peek()) != -1; Process(next));",
			              "f = Open()\ngoto ??1\nwhile true:\n\tProcess(next)\n\t:??1\n\tbreak unless ((next = Peek()) != (-1))");
		}
		
		[Test]
		public void WhileLoop()
		{
			TestStatement("while ((next = Peek()) != -1) { Process(next); }",
			              "while (next = Peek()) != (-1):\n\tProcess(next)");
		}
		
		[Test]
		public void DoLoop()
		{
			TestStatement("do { ok = Next(); } while (ok);",
			              "while true:\n\tok = Next()\n\tbreak unless ok");
		}
		
		[Test]
		public void ForeachLoop()
		{
			TestStatement("foreach (string s in list) {}",
			              "for s as System.String in list:\n\tpass");
		}
		
		[Test]
		public void Using()
		{
			TestStatement("using (StringReader r = file.Open()) { r.ReadLine(); }",
			              "using r = file.Open():\n\tr.ReadLine()");
		}
		
		[Test]
		public void Return()
		{
			TestStatement("return val;", "return val");
		}
		
		[Test]
		public void Throw()
		{
			TestStatement("throw cachedException;", "raise cachedException");
		}
		
		[Test]
		public void TryFinally()
		{
			TestStatement("try { Action(); } finally { CleanUp(); }",
			              "try:\n\tAction()\nensure:\n\tCleanUp()");
		}
		
		[Test]
		public void TryCatch()
		{
			TestStatement("try { Action(); } catch { DisplayError(); }",
			              "try:\n\tAction()\nexcept :\n\tDisplayError()");
		}
		
		[Test]
		public void TryCatchWithTypes()
		{
			TestStatement("try { Action(); }" +
			              "catch(FirstException ex) { DisplayError(ex); }" +
			              "catch(SecondException ex) { DisplayError(ex); }",
			              "try:\n\tAction()\n" +
			              "except ex as FirstException:\n\tDisplayError(ex)\n" +
			              "except ex as SecondException:\n\tDisplayError(ex)");
		}
		
		[Test]
		public void Switch()
		{
			TestStatement("switch (var) { case 1: A1(); break; default: A3(); break;  case 2: case 3: A2(); break; }",
			              "??1 = var\n" +
			              "if ??1 == 1:\n" +
			              "\tA1()\n" +
			              "elif (??1 == 2) or (??1 == 3):\n" +
			              "\tA2()\n" +
			              "else:\n" +
			              "\tA3()");
		}
		
		[Test]
		public void SwitchWithDefaultAtStart()
		{
			TestStatement("switch (var) { default: A3(); break; case 1: A1(); break;  case 2: case 3: A2(); break; }",
			              "??1 = var\n" +
			              "if ??1 == 1:\n" +
			              "\tA1()\n" +
			              "elif (??1 == 2) or (??1 == 3):\n" +
			              "\tA2()\n" +
			              "else:\n" +
			              "\tA3()");
		}
		
		[Test]
		public void SwitchWithOneCase()
		{
			TestStatement("switch (var) { case 1: A1(); break; }",
			              "??1 = var\n" +
			              "if ??1 == 1:\n" +
			              "\tA1()");
		}
		
		[Test]
		public void SwitchWithOneCaseWithMultipleLabels()
		{
			TestStatement("switch (var) { case 1: case 2: A1(); break; }",
			              "??1 = var\n" +
			              "if (??1 == 1) or (??1 == 2):\n" +
			              "\tA1()");
		}
		
		[Test]
		public void SwitchWithOnlyDefaultSection()
		{
			TestStatement("switch (var) { default: A1(); break; }",
			              "??1 = var\n" +
			              "A1()");
		}
		
		[Test]
		public void SwitchWithOnlyDefaultSectionAndEarlyBreak()
		{
			TestStatement("switch (var) { default: if (a) break; A1(); break; }",
			              "??1 = var\n" +
			              "if a:\n" +
			              "\tgoto ??1_end\n" +
			              "A1()\n" +
			              ":??1_end");
		}
		
		[Test]
		public void SwitchWithEarlyBreak()
		{
			TestStatement("switch (var) { case 1: if (a) break; B(); break; }",
			              "??1 = var\n" +
			              "if ??1 == 1:\n" +
			              "\tif a:\n" +
			              "\t\tgoto ??1_end\n" +
			              "\tB()\n" +
			              ":??1_end");
		}
		
		[Test]
		public void SwitchWithGotoCase()
		{
			TestStatement("switch (var) { case 1: B(); goto default; default: A(); break; }",
			              "??1 = var\n" +
			              "if ??1 == 1:\n" +
			              "\tB()\n" +
			              "\tgoto ??1_default\n" +
			              "else:\n" +
			              "\t:??1_default\n" +
			              "\tA()");
		}
		
		[Test]
		public void VBArrayDecl()
		{
			TestStatementVB("Dim A(10) As Integer", "A as (System.Int32) = array(System.Int32, 11)");
		}
		
		[Test]
		public void RedimTest()
		{
			TestStatementVB("Dim A As Integer()\nRedim A(5)", "A as (System.Int32)\nA = array(System.Int32, 6)");
		}
		
		[Test]
		public void VBMethodCall()
		{
			TestStatementVB("B = A(5)", "B = A(5)");
		}
		
		[Test]
		public void VBIndexerCall()
		{
			TestStatementVB("Dim A As Integer()\nB = A(5)", "A as (System.Int32)\nB = A[5]");
		}
		
		[Test]
		public void VBRaiseEvent()
		{
			TestStatementVB("RaiseEvent Test(4)", "Test(4)");
		}
		
		[Test]
		public void VBAddEventHandler()
		{
			TestStatementVB("AddHandler someObject.Test, AddressOf HandlerMethod", "someObject.Test += HandlerMethod");
		}
		
		[Test]
		public void VBAddRemoveHandler()
		{
			TestStatementVB("RemoveHandler someObject.Test, AddressOf HandlerMethod", "someObject.Test -= HandlerMethod");
		}
	}
}

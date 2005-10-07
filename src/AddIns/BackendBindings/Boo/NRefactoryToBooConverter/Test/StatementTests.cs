#region license
// Copyright (c) 2005, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// NRefactoryToBoo is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NRefactoryToBoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with NRefactoryToBoo; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

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
			              "while ((next = Peek()) != (-1)):\n\tProcess(next)");
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
			              "using (r = file.Open()):\n\tr.ReadLine()");
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
			              "try:\n\tAction()\nexcept:\n\tDisplayError()");
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
			              "if (??1 == 1):\n" +
			              "\tA1()\n" +
			              "else:\n" +
			              "\tif ((??1 == 2) or (??1 == 3)):\n" +
			              "\t\tA2()\n" +
			              "\telse:\n" +
			              "\t\tA3()");
		}
		
		[Test]
		public void SwitchWithEarlyBreak()
		{
			TestStatement("switch (var) { case 1: if (a) break; B(); break; }",
			              "??1 = var\n" +
			              "if (??1 == 1):\n" +
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
			              "if (??1 == 1):\n" +
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
	}
}

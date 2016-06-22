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
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class OutputTextLineParserTests
	{
		[Test]
		public void GetNUnitOutputFileLineReference_SingleNUnitOutputFollowedByNewLine_ReturnsExpectedFileLineReference()
		{
			string output = "   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 22\n";
			FileLineReference lineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(output, true);
			Assert.AreEqual(@"c:\test\NunitFoo\NunitFoo.Tests\FooTest.cs", lineRef.FileName);
			Assert.AreEqual(22, lineRef.Line);
			Assert.AreEqual(0, lineRef.Column);
		}
		
		[Test]
		public void GetNUnitOutputFileLineReference_MultilineNUnitOutputWithCarriageReturn_ReturnsExpectedFileLineReference()
		{
			string output = "   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 22\r\n";
			FileLineReference lineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(output, true);
			Assert.AreEqual(@"c:\test\NunitFoo\NunitFoo.Tests\FooTest.cs", lineRef.FileName);
			Assert.AreEqual(22, lineRef.Line);
			Assert.AreEqual(0, lineRef.Column);
		}

		[Test]
		public void GetNUnitOutputFileLineReference_MultilineNUnitOutput_ReturnsExpectedFileLineReference()
		{
			string output = 
				"   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 11\r\n" +
				"   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 22\r\n";
			FileLineReference lineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(output, true);
			Assert.AreEqual(lineRef.FileName, @"c:\test\NunitFoo\NunitFoo.Tests\FooTest.cs");
			Assert.AreEqual(22, lineRef.Line);
			Assert.AreEqual(0, lineRef.Column);
		}
		
		[Test]
		public void GetFileLineReference_NUnitConsoleFailure_ReturnsExpectedFileLineReference()
		{
			string output = @"c:\test\NunitFoo\NunitFoo.Tests\FooTest.cs(22)";
			FileLineReference lineRef = OutputTextLineParser.GetFileLineReference(output);
			Assert.AreEqual(@"c:\test\NunitFoo\NunitFoo.Tests\FooTest.cs", lineRef.FileName);
			Assert.AreEqual(22, lineRef.Line);
			Assert.AreEqual(0, lineRef.Column);
		}
		
		[Test]
		public void GetFileLineReference_CompilerFailure_ReturnsExpectedFileLineReference()
		{
			string output = @"c:\test\NunitFoo\NunitFoo.Tests\FooTest.cs(22,10)";
			FileLineReference lineRef = OutputTextLineParser.GetFileLineReference(output);
			Assert.AreEqual(@"c:\test\NunitFoo\NunitFoo.Tests\FooTest.cs", lineRef.FileName);
			Assert.AreEqual(22, lineRef.Line);
			Assert.AreEqual(10, lineRef.Column);
		}
		
		[Test]
		public void GetFileLineReference_NUnitWithTwoInStatementsWithOneInsideLessThanCharacter_ReturnsExpectedFileName()
		{
			string output = @" in < at NunitFoo.MyTest.Test() in c:\test\Test.cs:line 11";
			FileLineReference lineRef = OutputTextLineParser.GetFileLineReference(output);
			Assert.AreEqual(@"c:\test\Test.cs", lineRef.FileName);
		}
		
		[Test]
		public void GetFileLineReference_NUnitWithTwoInStatementsWithOneInsideGreaterThanCharacter_ReturnsExpectedFileName()
		{
			string output = @" in > at NunitFoo.MyTest.Test() in c:\test\Test.cs:line 11";
			FileLineReference lineRef = OutputTextLineParser.GetFileLineReference(output);
			Assert.AreEqual(@"c:\test\Test.cs", lineRef.FileName);
		}
		
		[Test]
		public void GetFileLineReference_NUnitWithTwoInStatementsWithOneInsidePipeCharacter_ReturnsExpectedFileName()
		{
			string output = @" in | at NunitFoo.MyTest.Test() in c:\test\Test.cs:line 11";
			FileLineReference lineRef = OutputTextLineParser.GetFileLineReference(output);
			Assert.AreEqual(@"c:\test\Test.cs", lineRef.FileName);
		}
		
		[Test]
		public void GetFileLineReference_NUnitWithTwoInStatementsWithOneInsideDoubleQuotesCharacter_ReturnsExpectedFileName()
		{
			string output = " in \" at NunitFoo.MyTest.Test() in c:\\test\\Test.cs:line 11";
			FileLineReference lineRef = OutputTextLineParser.GetFileLineReference(output);
			Assert.AreEqual(@"c:\test\Test.cs", lineRef.FileName);
		}
	}
}

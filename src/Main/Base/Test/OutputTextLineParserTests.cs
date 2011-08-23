// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

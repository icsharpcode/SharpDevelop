// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class OutputTextLineParserTests
	{
		[Test]
		public void Multiline()
		{
			string output = "   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 22\n";
			FileLineReference lineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(output, true);
			Assert.AreEqual(lineRef.FileName, "c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs");
			Assert.AreEqual(21, lineRef.Line);
			Assert.AreEqual(0, lineRef.Column);
		}
		
		[Test]
		public void MultilineWithCarriageReturn()
		{
			string output = "   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 22\r\n";
			FileLineReference lineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(output, true);
			Assert.AreEqual(lineRef.FileName, "c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs");
			Assert.AreEqual(21, lineRef.Line);
			Assert.AreEqual(0, lineRef.Column);
		}

		[Test]
		public void MultipleLines()
		{
			string output = "   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 11\r\n" +
				"   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 22\r\n";
			FileLineReference lineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(output, true);
			Assert.AreEqual(lineRef.FileName, "c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs");
			Assert.AreEqual(21, lineRef.Line);
			Assert.AreEqual(0, lineRef.Column);
		}
		
		[Test]
		public void NUnitConsoleFailure()
		{
			string output = "c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs(22)";
			FileLineReference lineRef = OutputTextLineParser.GetFileLineReference(output);
			Assert.AreEqual(lineRef.FileName, "c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs");
			Assert.AreEqual(21, lineRef.Line);
			Assert.AreEqual(0, lineRef.Column);
		}
		
		[Test]
		public void CompilerFailure()
		{
			string output = "c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs(22,10)";
			FileLineReference lineRef = OutputTextLineParser.GetFileLineReference(output);
			Assert.AreEqual(lineRef.FileName, "c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs");
			Assert.AreEqual(21, lineRef.Line);
			Assert.AreEqual(9, lineRef.Column);
		}
	}
}

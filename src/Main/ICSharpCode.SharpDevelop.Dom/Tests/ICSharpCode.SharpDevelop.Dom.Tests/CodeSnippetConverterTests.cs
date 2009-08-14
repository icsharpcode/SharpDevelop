// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class CodeSnippetConverterTests
	{
		List<IProjectContent> referencedContents;
		CodeSnippetConverter converter;
		string errors;
		
		public CodeSnippetConverterTests()
		{
			ProjectContentRegistry pcr = new ProjectContentRegistry();
			referencedContents = new List<IProjectContent> {
				pcr.Mscorlib,
				pcr.GetProjectContentForReference("System", "System")
			};
		}
		
		[SetUp]
		public void SetUp()
		{
			converter = new CodeSnippetConverter {
				ReferencedContents = referencedContents
			};
		}
		
		[Test]
		public void FixExpressionCase()
		{
			Assert.AreEqual("AppDomain.CurrentDomain", converter.VBToCSharp("appdomain.currentdomain", out errors));
		}
		
		[Test]
		public void Statements()
		{
			Assert.AreEqual("a = Console.Title;\n" +
			                "b = Console.ReadLine();",
			                Normalize(converter.VBToCSharp("a = Console.Title\n" +
			                                               "b = Console.Readline", out errors)));
		}
		
		[Test]
		public void UnknownPropertyReference()
		{
			Assert.AreEqual("a = Me.Font",
			                Normalize(converter.CSharpToVB("a = this.Font;", out errors)));
		}
		
		[Test]
		public void UnknownPropertyReference2()
		{
			Assert.AreEqual("X(Me.Font)",
			                Normalize(converter.CSharpToVB("X(this.Font);", out errors)));
		}
		
		[Test]
		public void FixReferenceToOtherMethodInSameClass()
		{
			Assert.AreEqual("public void A()\n" +
			                "{\n" +
			                "  Test();\n" +
			                "}\n" +
			                "public void Test()\n" +
			                "{\n" +
			                "}",
			                Normalize(converter.VBToCSharp("Sub A()\n" +
			                                               " test\n" +
			                                               "End Sub\n" +
			                                               "Sub Test\n" +
			                                               "End Sub",
			                                               out errors)));
		}
		
		[Test]
		public void ConvertFloatingPointToInteger()
		{
			Assert.AreEqual("CInt(Math.Truncate(-3.5))", converter.CSharpToVB("(int)(-3.5)", out errors));
			Assert.AreEqual("CInt(Math.Truncate(-3.5F))", converter.CSharpToVB("(int)(-3.5f)", out errors));
			Assert.AreEqual("CInt(Math.Truncate(-3.5D))", converter.CSharpToVB("(int)(-3.5m)", out errors));
		}
		
		[Test]
		public void ConvertLongToInteger()
		{
			Assert.AreEqual("CInt(-35L)", converter.CSharpToVB("(int)(-35L)", out errors));
		}
		
		string Normalize(string text)
		{
			return text.Replace("\t", "  ").Replace("\r", "").Trim();
		}
	}
}

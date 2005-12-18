// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Tests.PrettyPrinter
{
	[TestFixture]
	public class CSharpToVBConverterTest
	{
		public void TestProgram(string input, string expectedOutput)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(input));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			parser.CompilationUnit.AcceptVisitor(new CSharpToVBNetConvertVisitor(), null);
			VBNetOutputVisitor outputVisitor = new VBNetOutputVisitor();
			outputVisitor.Visit(parser.CompilationUnit, null);
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(expectedOutput, outputVisitor.Text);
		}
		
		public void TestStatement(string input, string expectedOutput)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("Class tmp1");
			b.AppendLine("\tSub tmp2()");
			using (StringReader r = new StringReader(expectedOutput)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("\t\t");
					b.AppendLine(line);
				}
			}
			b.AppendLine("\tEnd Sub");
			b.AppendLine("End Class");
			TestProgram("class tmp1 { void tmp2() {\n" + input + "\n}}", b.ToString());
		}
		
		[Test]
		public void ForWithUnknownConditionAndSingleStatement()
		{
			TestStatement("for (i = 0; unknownCondition; i++) b[i] = s[i];",
			              "i = 0\n" +
			              "While unknownCondition\n" +
			              "\tb(i) = s(i)\n" +
			              "\ti += 1\n" +
			              "End While");
		}
		
		[Test]
		public void ForWithUnknownConditionAndBlock()
		{
			TestStatement("for (i = 0; unknownCondition; i++) { b[i] = s[i]; }",
			              "i = 0\n" +
			              "While unknownCondition\n" +
			              "\tb(i) = s(i)\n" +
			              "\ti += 1\n" +
			              "End While");
		}
		
		[Test]
		public void ForWithSingleStatement()
		{
			TestStatement("for (i = 0; i < end; i++) b[i] = s[i];",
			              "For i = 0 To [end] - 1\n" +
			              "\tb(i) = s(i)\n" +
			              "Next");
		}
		[Test]
		public void ForWithBlock()
		{
			TestStatement("for (i = 0; i < end; i++) { b[i] = s[i]; }",
			              "For i = 0 To [end] - 1\n" +
			              "\tb(i) = s(i)\n" +
			              "Next");
		}
		[Test]
		public void AddEventHandler()
		{
			TestStatement("this.button1.Click += new System.EventHandler(this.OnButton1Click);",
			              "AddHandler Me.button1.Click, AddressOf Me.OnButton1Click");
		}
	}
}

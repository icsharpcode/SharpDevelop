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
	public class VBToCSharpConverterTest
	{
		public void TestProgram(string input, string expectedOutput)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(input));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			parser.CompilationUnit.AcceptVisitor(new VBNetToCSharpConvertVisitor(), null);
			CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor();
			outputVisitor.Visit(parser.CompilationUnit, null);
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(expectedOutput, outputVisitor.Text);
		}
		
		public void TestMember(string input, string expectedOutput)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("class tmp1");
			b.AppendLine("{");
			using (StringReader r = new StringReader(expectedOutput)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("\t");
					b.AppendLine(line);
				}
			}
			b.AppendLine("}");
			TestProgram("Class tmp1 \n" + input + "\nEnd Class", b.ToString());
		}
		
		public void TestStatement(string input, string expectedOutput)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("class tmp1");
			b.AppendLine("{");
			b.AppendLine("\tvoid tmp2()");
			b.AppendLine("\t{");
			using (StringReader r = new StringReader(expectedOutput)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("\t\t");
					b.AppendLine(line);
				}
			}
			b.AppendLine("\t}");
			b.AppendLine("}");
			TestProgram("Class tmp1 \n Sub tmp2() \n" + input + "\n End Sub \n End Class", b.ToString());
		}
		
		[Test]
		public void AddHandler()
		{
			TestStatement("AddHandler someEvent, AddressOf tmp2",
			              "someEvent += tmp2;");
			TestStatement("AddHandler someEvent, AddressOf Me.tmp2",
			              "someEvent += this.tmp2;");
		}
		
		[Test]
		public void RemoveHandler()
		{
			TestStatement("RemoveHandler someEvent, AddressOf tmp2",
			              "someEvent -= tmp2;");
			TestStatement("RemoveHandler someEvent, AddressOf Me.tmp2",
			              "someEvent -= this.tmp2;");
		}
		
		[Test]
		public void RaiseEvent()
		{
			TestStatement("RaiseEvent someEvent(Me, EventArgs.Empty)",
			              "if (someEvent != null) {\n\tsomeEvent(this, EventArgs.Empty);\n}");
		}
	}
}

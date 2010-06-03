// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>


using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using NUnit.Framework;

namespace VBParserExperiment
{
	[TestFixture]
	public class XmlModeLexerTests
	{
		ILexer GenerateLexer(StringReader sr)
		{
			return ParserFactory.CreateLexer(SupportedLanguage.VBNet, sr);
		}
		
		string TestStatement(string stmt)
		{
			return "Class Test\n" +
				"Sub A()\n" +
				stmt + "\n" +
				"End Sub\n" +
				"End Class";
		}
		
		[Test]
		public void TagWithContent()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("Dim x = <Test>Hello World</Test>")));
			
			CheckHead(lexer);
			
			Assert.AreEqual(Tokens.Dim, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Assign, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void HtmlText()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("Dim x = <div><h1>Title</h1>" +
			                                                            "<p>test test <br /> test</p></div>")));
			
			CheckHead(lexer);
			
			Assert.AreEqual(Tokens.Dim, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Assign, lexer.NextToken().Kind);
			
			// <div>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// <h1>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// Title
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// </h1>
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// <p>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// test test
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// <br />
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTagEmptyElement, lexer.NextToken().Kind);
			
			// test
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// </p>
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// </div>
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void XmlLiteralsExample1()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("Dim xml = <menu>\n" +
			                                                            "              <course name=\"appetizer\">\n" +
			                                                            "                  <dish>Shrimp Cocktail</dish>\n" +
			                                                            "                  <dish>Escargot</dish>\n" +
			                                                            "              </course>\n" +
			                                                            "              <course name=\"main\">\n" +
			                                                            "                  <dish>Filet Mignon</dish>\n" +
			                                                            "                  <dish>Garlic Potatoes</dish>\n" +
			                                                            "                  <dish>Broccoli</dish>\n" +
			                                                            "              </course>\n" +
			                                                            "              <course name=\"dessert\">\n" +
			                                                            "                  <dish>Chocolate Cheesecake</dish>\n" +
			                                                            "              </course>\n" +
			                                                            "          </menu>")));
			
			CheckHead(lexer);
			
			Assert.AreEqual(Tokens.Dim, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Assign, lexer.NextToken().Kind);
			
			#region <menu>
			// <menu>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			#region <course>
			// <course name=\"appetizer\">
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Assign, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.LiteralString, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// <dish>Shrimp Cocktail</dish>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// <dish>Escargot</dish>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// </course>
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			#endregion
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			#region <course>
			// <course name=\"main\">
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Assign, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.LiteralString, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// <dish>Filet Mignon</dish>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// <dish>Garlic Potatoes</dish>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// <dish>Broccoli</dish>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// </course>
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			#endregion
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			#region <course>
			// <course name=\"dessert\">
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Assign, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.LiteralString, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// <dish>Chocolate Cheesecake</dish>
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// </course>
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			#endregion
			
			// whitespaces
			Assert.AreEqual(Tokens.XmlContent, lexer.NextToken().Kind);
			
			// </menu>
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			#endregion
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void SimpleEmptyTag()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("Dim x = <Test />")));
			
			CheckHead(lexer);
			
			Assert.AreEqual(Tokens.Dim, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Assign, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTagEmptyElement, lexer.NextToken().Kind);
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void SimpleTag()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("Dim x = <Test></Test>")));
			
			CheckHead(lexer);
			
			Assert.AreEqual(Tokens.Dim, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Assign, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlOpenEndTag, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.XmlCloseTag, lexer.NextToken().Kind);
			
			CheckFoot(lexer);
		}

		void CheckFoot(ILexer lexer)
		{
			Assert.AreEqual(Tokens.EOL, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.End, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Sub, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.EOL, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.End, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Class, lexer.NextToken().Kind);
		}

		void CheckHead(ILexer lexer)
		{
			Assert.AreEqual(Tokens.Class, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.EOL, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Sub, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.Identifier, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.OpenParenthesis, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.CloseParenthesis, lexer.NextToken().Kind);
			Assert.AreEqual(Tokens.EOL, lexer.NextToken().Kind);
		}
	}
}

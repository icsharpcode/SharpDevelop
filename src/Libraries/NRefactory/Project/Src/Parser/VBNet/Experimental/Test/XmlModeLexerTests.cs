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
		#region Xml Tests
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
			
			CheckTokens(lexer, Tokens.Dim, Tokens.Identifier, Tokens.Assign,
			            // <menu>
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // <course name=\"appetizer\">
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.Identifier, Tokens.Assign, Tokens.LiteralString, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // <dish>Shrimp Cocktail</dish>
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // <dish>Escargot</dish>
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // </course>
			            Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // <course name=\"main\">
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.Identifier, Tokens.Assign, Tokens.LiteralString, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // <dish>Filet Mignon</dish>
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // <dish>Garlic Potatoes</dish>
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // <dish>Broccoli</dish>
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // </course>
			            Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // <course name=\"dessert\">
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.Identifier, Tokens.Assign, Tokens.LiteralString, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // <dish>Chocolate Cheesecake</dish>
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // </course>
			            Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            // whitespaces
			            Tokens.XmlContent,
			            // </menu>
			            Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag
			           );
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void SimpleXmlWithComments()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement(@"Dim x = <!-- Test file -->
			                                                                      <Test>
			                                                                        <!-- Test data -->
			                                                                        <Data />
			                                                                      </Test>
			                                                                      <!-- eof -->
			                                                                      <!-- hey, wait! -->")));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.Dim, Tokens.Identifier, Tokens.Assign,
			            Tokens.XmlComment, Tokens.XmlContent, Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            Tokens.XmlContent, Tokens.XmlComment, Tokens.XmlContent, Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTagEmptyElement,
			            Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlComment, Tokens.XmlComment);
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void SimpleEmptyTag()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("Dim x = <Test />")));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.Dim, Tokens.Identifier, Tokens.Assign,
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTagEmptyElement);
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void SimpleTag()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("Dim x = <Test></Test>")));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.Dim, Tokens.Identifier, Tokens.Assign, Tokens.XmlOpenTag,
			            Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlOpenEndTag,
			            Tokens.Identifier, Tokens.XmlCloseTag);
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void XmlImport()
		{
			string code = @"Imports System
Imports System.Linq

Imports <xmlns='http://icsharpcode.net/sharpdevelop/avalonedit'>
Imports <xmlns:h='http://www.w3.org/TR/html4/'>

Class TestClass
	Sub TestSub()
		Dim xml = <h:table>
					<h:tr>
						<h:td>1. Cell</h:td>
					</h:tr>
				  </h:table>
	End Sub
End Class";
			
			ILexer lexer = GenerateLexer(new StringReader(code));
			
			CheckTokens(lexer, Tokens.Imports, Tokens.Identifier, Tokens.EOL,
			            Tokens.Imports, Tokens.Identifier, Tokens.Dot, Tokens.Identifier, Tokens.EOL,
			            Tokens.Imports, Tokens.XmlOpenTag, Tokens.Identifier, Tokens.Assign, Tokens.LiteralString, Tokens.XmlCloseTag, Tokens.EOL,
			            Tokens.Imports, Tokens.XmlOpenTag, Tokens.Identifier, Tokens.Assign, Tokens.LiteralString, Tokens.XmlCloseTag, Tokens.EOL,
			            Tokens.Class, Tokens.Identifier, Tokens.EOL, Tokens.Sub, Tokens.Identifier, Tokens.OpenParenthesis, Tokens.CloseParenthesis, Tokens.EOL,
			            Tokens.Dim, Tokens.Identifier, Tokens.Assign, Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            Tokens.XmlContent, Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            Tokens.XmlContent, Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            Tokens.EOL, Tokens.End, Tokens.Sub, Tokens.EOL, Tokens.End, Tokens.Class
			           );
		}
		
		[Test]
		public void CDataSection()
		{
			string xml = @"Dim xml = <template>
				<name>test</name>
				<language>VB</languge>
				<file language='XAML'>
					<![CDATA[<Window x:Class='DefaultNamespace.Window1'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	Title='DefaultNamespace' Height='300' Width='300'>
	<Grid>
		
	</Grid>
</Window>]]>
				</file>
				<file language='CSharp'>
				<![CDATA[using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DefaultNamespace
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}
	}
}]]>
				</file>
			</template>
			";
			
			ILexer lexer = GenerateLexer(new StringReader(TestStatement(xml)));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.Dim, Tokens.Identifier, Tokens.Assign, // 2
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, // 6
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, // 10
			            Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, // 14
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, // 18
			            Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent, // 22
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.Identifier, Tokens.Assign, Tokens.LiteralString, Tokens.XmlCloseTag, // 28
			            Tokens.XmlContent, Tokens.XmlCData, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag, // 34
			            Tokens.XmlContent, Tokens.XmlOpenTag, Tokens.Identifier, Tokens.Identifier, Tokens.Assign, Tokens.LiteralString, Tokens.XmlCloseTag,
			            Tokens.XmlContent, Tokens.XmlCData, Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag,
			            Tokens.XmlContent, Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag
			           );
			
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void GetXmlNamespace()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("Dim name = GetXmlNamespace(x)")));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.Dim, Tokens.Identifier, Tokens.Assign,
			            Tokens.GetXmlNamespace, Tokens.OpenParenthesis, Tokens.Identifier, Tokens.CloseParenthesis);
			
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void GetXmlNamespace2()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("Dim name = GetXmlNamespace(db-name)")));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.Dim, Tokens.Identifier, Tokens.Assign,
			            Tokens.GetXmlNamespace, Tokens.OpenParenthesis, Tokens.Identifier, Tokens.CloseParenthesis);
			
			
			CheckFoot(lexer);
		}
		#endregion
		
		#region Context Tests
		[Test]
		public void MethodInvocation()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("DoSomething(<Test />, True)")));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.Identifier, Tokens.OpenParenthesis, Tokens.XmlOpenTag,
			            Tokens.Identifier, Tokens.XmlCloseTagEmptyElement, Tokens.Comma, Tokens.True,
			            Tokens.CloseParenthesis);
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void AddHandlerStatement()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("AddHandler <Test />, True")));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.AddHandler, Tokens.XmlOpenTag,
			            Tokens.Identifier, Tokens.XmlCloseTagEmptyElement, Tokens.Comma, Tokens.True);
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void AddHandlerStatement2()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("AddHandler <x />, <y />")));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.AddHandler, Tokens.XmlOpenTag,
			            Tokens.Identifier, Tokens.XmlCloseTagEmptyElement, Tokens.Comma, Tokens.XmlOpenTag,
			            Tokens.Identifier, Tokens.XmlCloseTagEmptyElement);
			
			CheckFoot(lexer);
		}
		
		[Test]
		public void RemoveHandlerStatement()
		{
			ILexer lexer = GenerateLexer(new StringReader(TestStatement("RemoveHandler <x />, <Data>5</Data>")));
			
			CheckHead(lexer);
			
			CheckTokens(lexer, Tokens.RemoveHandler, Tokens.XmlOpenTag,
			            Tokens.Identifier, Tokens.XmlCloseTagEmptyElement, Tokens.Comma,
			            Tokens.XmlOpenTag, Tokens.Identifier, Tokens.XmlCloseTag, Tokens.XmlContent,
			            Tokens.XmlOpenEndTag, Tokens.Identifier, Tokens.XmlCloseTag
			           );
			
			CheckFoot(lexer);
		}
		#endregion
		
		#region Helpers
		ILexer GenerateLexer(StringReader sr)
		{
			return ParserFactory.CreateLexer(SupportedLanguage.VBNet, sr);
		}
		
		string TestStatement(string stmt)
		{
			return "Class Test\n" +
				"Sub A\n" +
				stmt + "\n" +
				"End Sub\n" +
				"End Class";
		}
		
		void CheckFoot(ILexer lexer)
		{
			CheckTokens(lexer, Tokens.EOL, Tokens.End, Tokens.Sub, Tokens.EOL, Tokens.End, Tokens.Class);
		}
		
		void CheckHead(ILexer lexer)
		{
			CheckTokens(lexer, Tokens.Class, Tokens.Identifier, Tokens.EOL,
			            Tokens.Sub, Tokens.Identifier, Tokens.EOL);
		}
		
		void CheckTokens(ILexer lexer, params int[] tokens)
		{
			for (int i = 0; i < tokens.Length; i++) {
				int token = tokens[i];
				Token t = lexer.NextToken();
				int next = t.Kind;
				Assert.AreEqual(token, next, "{2} of {3}: {0} != {1}; at {4}", Tokens.GetTokenString(token), Tokens.GetTokenString(next), i + 1, tokens.Length, t.Location);
			}
		}
		#endregion
	}
}

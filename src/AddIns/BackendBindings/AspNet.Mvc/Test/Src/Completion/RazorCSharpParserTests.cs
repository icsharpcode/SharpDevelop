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

//using System;
//using ICSharpCode.AspNet.Mvc.Completion;
//using ICSharpCode.SharpDevelop;
//using ICSharpCode.SharpDevelop.Dom;
//using NUnit.Framework;
//
//namespace AspNet.Mvc.Tests.Completion
//{
//	[TestFixture]
//	public class RazorCSharpParserTests
//	{
//		RazorCSharpParser parser;
//		
//		void CreateParser()
//		{
//			parser = new RazorCSharpParser();
//		}
//		
//		ICompilationUnit Parse(string code)
//		{
//			var projectContent = new DefaultProjectContent();
//			var textBuffer = new StringTextBuffer(code);
//			return parser.Parse(projectContent, @"d:\MyProject\Views\Index.cshtml", textBuffer);
//		}
//		
//		[Test]
//		public void Parse_ModelDirectiveWithTypeName_ModelTypeNameFound()
//		{
//			CreateParser();
//			string code = "@model MvcApplication.MyModel\r\n";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual("MvcApplication.MyModel", compilationUnit.ModelTypeName);
//		}
//		
//		[Test]
//		public void Parse_ModelDirectiveWithTypeNameFollowedByHtmlMarkup_ModelTypeNameFound()
//		{
//			CreateParser();
//			string code =
//				"@model MvcApplication.LogonModel\r\n" +
//				"<h2>Index</h2>\r\n";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual("MvcApplication.LogonModel", compilationUnit.ModelTypeName);
//		}
//		
//		[Test]
//		public void Parse_SingleLineFileWithModelDirectiveAndTypeNameButNoNewLineAtEnd_ModelTypeNameFound()
//		{
//			CreateParser();
//			string code = "@model MvcApplication.MyModel";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual("MvcApplication.MyModel", compilationUnit.ModelTypeName);
//		}
//		
//		[Test]
//		public void Parse_ModelTypeDirectiveWithTypeNameFollowedByRazorBlock_ModelTypeNameFound()
//		{
//			CreateParser();
//			
//			string code =
//				"@model IEnumerable<MvcApplication1.Models.MyClass>\r\n" +
//				"\r\n" +
//				"@{\r\n" +
//				"    ViewBag.Title = \"Title1\";\r\n" +
//				"}\r\n" +
//				"\r\n";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual("IEnumerable<MvcApplication1.Models.MyClass>", compilationUnit.ModelTypeName);
//		}
//		
//		[Test]
//		public void Parse_UsingDirective_ModelTypeNameIsEmptyString()
//		{
//			CreateParser();
//			string code = "@using System.Xml\r\n";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
//		}
//		
//		[Test]
//		public void Parse_HelperDirective_ModelTypeNameIsEmptyString()
//		{
//			CreateParser();
//			string code = "@helper MyHelper\r\n";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
//		}
//		
//		[Test]
//		public void Parse_HtmlMarkupOnly_ModelTypeNameIsEmptyString()
//		{
//			CreateParser();
//			string code = "<h1>heading</h1>\r\n";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
//		}
//		
//		[Test]
//		public void Parse_ModelDirectiveOnly_ModelTypeNameIsEmptyString()
//		{
//			CreateParser();
//			string code = "@model";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
//		}
//		
//		[Test]
//		public void Parse_ModelStringInsideParagraphTags_ModelTypeNameIsEmptyString()
//		{
//			CreateParser();
//			string code = "<p>model</p>";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
//		}
//		
//		[Test]
//		public void Parse_ModelStringOnlyWithoutRazorTransition_ModelTypeNameIsEmptyString()
//		{
//			CreateParser();
//			string code = "model";
//			
//			var compilationUnit = Parse(code) as RazorCompilationUnit;
//			
//			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
//		}
//	}
//}

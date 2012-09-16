// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc.Completion;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Completion
{
	[TestFixture]
	public class RazorCSharpParserTests
	{
		RazorCSharpParser parser;
		
		void CreateParser()
		{
			parser = new RazorCSharpParser();
		}
		
		ICompilationUnit Parse(string code)
		{
			var projectContent = new DefaultProjectContent();
			var textBuffer = new StringTextBuffer(code);
			return parser.Parse(projectContent, @"d:\MyProject\Views\Index.cshtml", textBuffer);
		}
		
		[Test]
		public void Parse_ModelDirectiveWithTypeName_ModelTypeNameFound()
		{
			CreateParser();
			string code = "@model MvcApplication.MyModel\r\n";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual("MvcApplication.MyModel", compilationUnit.ModelTypeName);
		}
		
		[Test]
		public void Parse_ModelDirectiveWithTypeNameFollowedByHtmlMarkup_ModelTypeNameFound()
		{
			CreateParser();
			string code =
				"@model MvcApplication.LogonModel\r\n" +
				"<h2>Index</h2>\r\n";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual("MvcApplication.LogonModel", compilationUnit.ModelTypeName);
		}
		
		[Test]
		public void Parse_SingleLineFileWithModelDirectiveAndTypeNameButNoNewLineAtEnd_ModelTypeNameFound()
		{
			CreateParser();
			string code = "@model MvcApplication.MyModel";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual("MvcApplication.MyModel", compilationUnit.ModelTypeName);
		}
		
		[Test]
		public void Parse_ModelTypeDirectiveWithTypeNameFollowedByRazorBlock_ModelTypeNameFound()
		{
			CreateParser();
			
			string code =
				"@model IEnumerable<MvcApplication1.Models.MyClass>\r\n" +
				"\r\n" +
				"@{\r\n" +
				"    ViewBag.Title = \"Title1\";\r\n" +
				"}\r\n" +
				"\r\n";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual("IEnumerable<MvcApplication1.Models.MyClass>", compilationUnit.ModelTypeName);
		}
		
		[Test]
		public void Parse_UsingDirective_ModelTypeNameIsEmptyString()
		{
			CreateParser();
			string code = "@using System.Xml\r\n";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
		}
		
		[Test]
		public void Parse_HelperDirective_ModelTypeNameIsEmptyString()
		{
			CreateParser();
			string code = "@helper MyHelper\r\n";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
		}
		
		[Test]
		public void Parse_HtmlMarkupOnly_ModelTypeNameIsEmptyString()
		{
			CreateParser();
			string code = "<h1>heading</h1>\r\n";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
		}
		
		[Test]
		public void Parse_ModelDirectiveOnly_ModelTypeNameIsEmptyString()
		{
			CreateParser();
			string code = "@model";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
		}
		
		[Test]
		public void Parse_ModelStringInsideParagraphTags_ModelTypeNameIsEmptyString()
		{
			CreateParser();
			string code = "<p>model</p>";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
		}
		
		[Test]
		public void Parse_ModelStringOnlyWithoutRazorTransition_ModelTypeNameIsEmptyString()
		{
			CreateParser();
			string code = "model";
			
			var compilationUnit = Parse(code) as RazorCompilationUnit;
			
			Assert.AreEqual(String.Empty, compilationUnit.ModelTypeName);
		}
	}
}

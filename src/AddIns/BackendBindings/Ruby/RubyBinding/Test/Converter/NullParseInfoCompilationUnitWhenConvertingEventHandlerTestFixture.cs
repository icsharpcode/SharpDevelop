// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class NullParseInfoCompilationUnitWhenConvertingEventHandlerTestFixture
	{
		ParseInformation parseInfo;

		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public Foo()\r\n" +
						"    {" +
						"        button = new Button();\r\n" +
						"        button.Click += ButtonClick;\r\n" +
						"        button.MouseDown += self.OnMouseDown;\r\n" +
						"    }\r\n" +
						"}";

		[SetUp]
		public void Init()
		{
			parseInfo = new ParseInformation();
		}
		
		[Test]
		public void ConvertedRubyCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        button = Button.new()\r\n" +
				"        button.Click { self.ButtonClick() }\r\n" +
				"        button.MouseDown { self.OnMouseDown() }\r\n" +
				"    end\r\n" +
				"end";
			
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp, parseInfo);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code, code);
		}
		
		[Test]
		public void ParseInfoDefaultConstructorCreatesParseInfoWithNullBestCompilationUnit()
		{
			Assert.IsNull(parseInfo.BestCompilationUnit);
		}
		
		[Test]
		public void ParseInfoDefaultConstructorCreatesParseInfoWithNullMostRecentCompilationUnit()
		{
			Assert.IsNull(parseInfo.MostRecentCompilationUnit);
		}
		
		[Test]
		public void ParseInfoDefaultConstructorCreatesParseInfoWithNullValidCompilationUnit()
		{
			Assert.IsNull(parseInfo.ValidCompilationUnit);
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			parseInfo = new ParseInformation(new DefaultCompilationUnit(new DefaultProjectContent()));
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
	}
}

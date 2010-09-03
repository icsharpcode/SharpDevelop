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
	public class CannotFindMethodForEventHandlerConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public Foo()\r\n" +
						"    {" +
						"        button = new Button();\r\n" +
						"        button.Click += ButtonClick;\r\n" +
						"        button.MouseDown += self.OnMouseDown;\r\n" +
						"    }\r\n" +
						"}";
		
		/// <summary>
		/// When event handler method cannot be found the generated ruby code assumes the
		/// method has no parameters.
		/// </summary>
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
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			ParseInformation parseInfo = new ParseInformation(unit);
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp, parseInfo);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code, code);
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class ClassDestructorConversionTestFixture
	{		
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    int i = 10;\r\n" +
			"    public ~Foo()\r\n" +
			"    {\r\n" +
			"        i = 0;\r\n" +
			"    }\r\n" +
			"}";
	
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        @i = 10\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def finalize()\r\n" +
				"        @i = 0\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

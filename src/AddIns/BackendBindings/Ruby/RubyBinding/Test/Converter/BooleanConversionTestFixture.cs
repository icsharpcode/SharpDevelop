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
	public class BooleanVariableConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    bool a;\r\n" +
						"\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        a = true;\r\n" +
						"    }\r\n" +
						"}";
	
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby = "class Foo\r\n" +
									"    def initialize()\r\n" +
									"        @a = true\r\n" +
									"    end\r\n" +
									"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

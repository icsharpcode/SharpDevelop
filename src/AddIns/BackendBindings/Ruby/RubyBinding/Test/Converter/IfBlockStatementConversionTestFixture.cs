// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests the conversion of an if-else statement where the
	/// if and else blocks both have more than one statement.
	/// </summary>
	[TestFixture]
	public class IfBlockStatementConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    int i = 0;\r\n" +
						"    public int GetCount()\r\n" +
						"    {" +
						"        if (i == 0) {\r\n" +
						"        i = 10;\r\n" +
						"            return 10;\r\n" +
						"        } else {\r\n" +
						"        i = 4;\r\n" +
						"            return 4;\r\n" +
						"        }\r\n" +
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
				"        @i = 0\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def GetCount()\r\n" +
				"        if @i == 0 then\r\n" +
				"            @i = 10\r\n" +
				"            return 10\r\n" +
				"        else\r\n" +
				"            @i = 4\r\n" +
				"            return 4\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

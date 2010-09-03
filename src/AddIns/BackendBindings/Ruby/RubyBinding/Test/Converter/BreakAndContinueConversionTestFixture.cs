// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that a break statement is converted correctly.
	/// </summary>
	[TestFixture]	
	public class BreakAndContinueConversionTestFixture
	{		
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public void Run()\r\n" +
			"    {\r\n" +
			"        int i = 0;\r\n" +
			"        while (i < 10) {\r\n" +
			"            if (i == 5) {\r\n" +
			"                break;\r\n" +
			"            } else {\r\n" +
			"                continue;\r\n" +
			"            }\r\n" +
			"            i++;\r\n" +
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
				"    def Run()\r\n" +
				"        i = 0\r\n" +
				"        while i < 10\r\n" +
				"            if i == 5 then\r\n" +
				"                break\r\n" +
				"            else\r\n" +
				"                next\r\n" +
				"            end\r\n" +
				"            i += 1\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby, Ruby);
		}
	}
}

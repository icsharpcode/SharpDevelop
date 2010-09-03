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
	/// if and else block have a local variable defined.
	/// </summary>
	[TestFixture]
	public class LocalVariableDeclarationInIfStatementTestFixture
	{		
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    int i = 0;\r\n" +
			"    public int GetCount()\r\n" +
			"    {" +
			"        if (i == 0) {\r\n" +
			"            int j = 10;\r\n" +
			"            return j;\r\n" +
			"        } else {\r\n" +
			"        iint j = 4;\r\n" +
			"            return j;\r\n" +
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
				"            j = 10\r\n" +
				"            return j\r\n" +
				"        else\r\n" +
				"            j = 4\r\n" +
				"            return j\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

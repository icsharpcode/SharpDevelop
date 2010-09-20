// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class PropertyWithGetterTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    int count = 0;\r\n" +
						"    int i = 0;\r\n" +
						"    public int Count {\r\n" +
						"        get {\r\n" +
						"            if (i == 0) {\r\n" +
						"                return 10;\r\n" +
						"            } else {\r\n" +
						"                return count;\r\n" +
						"            }\r\n" +
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
				"        @count = 0\r\n" +
				"        @i = 0\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def Count\r\n" +
				"        if @i == 0 then\r\n" +
				"            return 10\r\n" +
				"        else\r\n" +
				"            return @count\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby, Ruby);
		}
	}
}

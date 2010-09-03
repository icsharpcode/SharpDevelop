// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class ElseIfStatementConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public int GetCount(i)\r\n" +
						"    {" +
						"        if (i == 0) {\r\n" +
						"            return 10;\r\n" +
						"        } else if (i < 1) {\r\n" +
						"            return 4;\r\n" +
						"        }\r\n" +
						"        return 2;\r\n" +
						"    }\r\n" +
						"}";
						
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby = "class Foo\r\n" +
									"    def GetCount(i)\r\n" +
									"        if i == 0 then\r\n" +
									"            return 10\r\n" +
									"        elsif i < 1 then\r\n" +
									"            return 4\r\n" +
									"        end\r\n" +
									"        return 2\r\n" +
									"    end\r\n" +
									"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

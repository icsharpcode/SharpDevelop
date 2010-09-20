// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that null is converted to None in Ruby.
	/// </summary>
	[TestFixture]	
	public class NullConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public int Run(string a)\r\n" +
						"    {\r\n" +
						"        if (a == null) {\r\n" +
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
									"    def Run(a)\r\n" +
									"        if a == nil then\r\n" +
									"            return 4\r\n" +
									"        end\r\n" +
									"        return 2\r\n" +
									"    end\r\n" +
									"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

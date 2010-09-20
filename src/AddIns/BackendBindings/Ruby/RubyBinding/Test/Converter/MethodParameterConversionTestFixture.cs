// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that method parameters are converted into Ruby
	/// correctly.
	/// </summary>
	[TestFixture]
	public class MethodParameterConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public int Run(int i)\r\n" +
						"    {\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";

		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			string expectedCode = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        return i\r\n" +
									"    end\r\n" +
									"end";
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}

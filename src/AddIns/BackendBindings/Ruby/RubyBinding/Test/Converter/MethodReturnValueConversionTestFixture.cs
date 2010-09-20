// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToRubyConverter class can convert a method that
	/// returns an integer.
	/// </summary>
	[TestFixture]
	public class MethodReturnValueConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public int Run()\r\n" +
						"    {\r\n" +
						"        return 10;\r\n" +
						"    }\r\n" +
						"}";

		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby = "class Foo\r\n" +
									"    def Run()\r\n" +
									"        return 10\r\n" +
									"    end\r\n" +
									"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

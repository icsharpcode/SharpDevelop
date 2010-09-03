// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that parameters passed to a constructor are converted to Ruby correctly.
	/// </summary>
	[TestFixture]
	public class CallConstructorWithParametersConversionTestFixture
	{	
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public Foo()\r\n" +
			"    {\r\n" +
			"        Bar b = new Bar(0, 0, 1, 10);\r\n" +
			"    }\r\n" +
			"}";
		
		[Test]
		public void ConvertedRubyCode()
		{
			string expectedRuby =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        b = Bar.new(0, 0, 1, 10)\r\n" +
				"    end\r\n" +
				"end";
			
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

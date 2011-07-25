// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToRubyConverter correctly converts a local
	/// variable in the constructor.
	/// </summary>
	[TestFixture]
	public class LocalVariableAssignedInConstructorTestFixture
	{		
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public Foo()\r\n" +
			"    {\r\n" +
			"        int i = 0;\r\n" +
			"        int i = 2;\r\n" +
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
				"        i = 0\r\n" +
				"        i = 2\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

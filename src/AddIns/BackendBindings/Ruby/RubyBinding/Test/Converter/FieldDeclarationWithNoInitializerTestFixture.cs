// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests the C# to Ruby converter does not add an 
	/// assignment for a variable declaration that has no 
	/// initial value assigned.
	/// </summary>
	[TestFixture]
	public class FieldDeclarationWithNoInitializerTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    private int i;\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        int j = 0;\r\n" +
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
				"        j = 0\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}		
	}
}

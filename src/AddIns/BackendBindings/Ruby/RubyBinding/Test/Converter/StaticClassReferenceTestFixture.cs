// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that C# code such as "System.Console.WriteLine("Test");"
	/// is converted to Ruby code correctly.
	/// </summary>
	[TestFixture]
	public class StaticClassReferenceTestFixture
	{	
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        System.Console.WriteLine(\"Test\");\r\n" +
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
				"        System.Console.WriteLine(\"Test\")\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

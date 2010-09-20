// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Was causing a null reference exception in the convertor's IsStatic method since the method
	/// being passed as a parameter is null.
	/// </summary>
	[TestFixture]
	public class MethodCallInConstructorTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        Init();\r\n" +
						"    }\r\n" +
						"}";
		
		[Test]
		public void GeneratedRubySourceCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        self.Init()\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

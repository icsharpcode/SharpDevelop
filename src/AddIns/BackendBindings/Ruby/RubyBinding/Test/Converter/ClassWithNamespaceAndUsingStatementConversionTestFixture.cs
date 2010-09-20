// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that there is a new line between the require statements and the module name.
	/// </summary>
	[TestFixture]
	public class ClassWithNamespaceAndUsingStatementsConversionTestFixture
	{
		string csharp =
			"using System.Windows.Forms;\r\n" +
			"\r\n" +
			"namespace MyNamespace\r\n" +
			"{\r\n" +
			"    class Foo\r\n" +
			"    {\r\n" +
			"    }\r\n" +
			"}";
			
		[Test]
		public void GeneratedRubySourceCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"require \"mscorlib\"\r\n" +
				"require \"System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n" +
				"\r\n" +
				"module MyNamespace\r\n" +
				"    class Foo\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class LocalVariableDefinitionsOnSameLineTests
	{
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public Foo()\r\n" +
			"    {\r\n" +
			"        int i = 0, i = 2;\r\n" +
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
		
		string vnetClassWithTwoArrayLocalVariablesOnSameLine =
			"class Foo\r\n" +
			"    Public Sub New()\r\n" +
			"    	Dim i(10), j(20) as integer\r\n" +
			"    End Sub\r\n" +
			"end class";
		
		[Test]
		public void ConvertVBNetClassWithTwoArrayVariablesOnSameLine()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string ruby = converter.Convert(vnetClassWithTwoArrayLocalVariablesOnSameLine);
			string expectedRuby =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        i = Array.CreateInstance(System::Int32, 10)\r\n" +
				"        j = Array.CreateInstance(System::Int32, 20)\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, ruby);
		}
	}
}

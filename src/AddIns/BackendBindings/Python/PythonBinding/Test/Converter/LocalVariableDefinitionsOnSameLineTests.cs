// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
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
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython =
				"class Foo(object):\r\n" +
				"    def __init__(self):\r\n" +
				"        i = 0\r\n" +
				"        i = 2";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		string vnetClassWithTwoArrayVariablesOnSameLine =
			"class Foo\r\n" +
			"    Public Sub New()\r\n" +
			"    	Dim i(10), j(20) as integer\r\n" +
			"    End Sub\r\n" +
			"end class";
		
		[Test]
		public void ConvertVBNetClassWithTwoArrayVariablesOnSameLine()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string python = converter.Convert(vnetClassWithTwoArrayVariablesOnSameLine);
			string expectedPython =
				"class Foo(object):\r\n" +
				"    def __init__(self):\r\n" +
				"        i = Array.CreateInstance(int, 10)\r\n" +
				"        j = Array.CreateInstance(int, 20)";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

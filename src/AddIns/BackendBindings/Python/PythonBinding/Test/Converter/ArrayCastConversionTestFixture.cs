// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that an array cast is correctly converted to Python.
	/// </summary>
	[TestFixture]
	public class ArrayCastConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public void Assign()\r\n" +
						"    {\r\n" +
						"        int[] elements = new int[10];\r\n" +
						"        List<int[]> list = new List<int[]>();\r\n" + 
						"        list.Add((int[])elements.Clone());\r\n" +
						"    }\r\n" +
						"}";
		
		[Test]
		public void GeneratedPythonSourceCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
				"    def Assign(self):\r\n" +
				"        elements = Array.CreateInstance(int, 10)\r\n" +
				"        list = List[Array[int]]()\r\n" +
				"        list.Add(elements.Clone())";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

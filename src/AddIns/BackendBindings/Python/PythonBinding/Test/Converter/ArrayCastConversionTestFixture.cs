// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
				"\tdef Assign(self):\r\n" +
				"\t\telements = System.Array.CreateInstance(System.Int32, 10)\r\n" +
				"\t\tlist = List()\r\n" +
				"\t\tlist.Add(elements.Clone())";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

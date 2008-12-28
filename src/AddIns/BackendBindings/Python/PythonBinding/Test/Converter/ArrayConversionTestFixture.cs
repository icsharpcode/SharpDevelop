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
	/// Tests that an array is converted from C# to Python.
	/// </summary>
	[TestFixture]
	public class ArrayConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic int[] Run()\r\n" +
						"\t{" +
						"\t\tint[] i = new int[] {1, 2, 3, 4};\r\n" +
						"\t\ti[0] = 5;\r\n" +
						"\t\treturn i;\r\n" +
						"\t}\r\n" +
						"}";
		
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\ti = System.Array[System.Int32]((1, 2, 3, 4))\r\n" +
									"\t\ti[0] = 5\r\n" +
									"\t\treturn i";
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}	
	}
}

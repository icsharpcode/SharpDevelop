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
	/// Tests the conversion of a foreach loop to Python.
	/// </summary>
	[TestFixture]
	public class ForeachConversionTestFixture
	{	
		string csharp = "class Foo\r\n" +
					"{\r\n" +
					"\tpublic int GetCount(int[] items)\r\n" +
					"\t{\r\n" +
					"\t\tint count = 0;\r\n" +
					"\t\tforeach (int item in items) {\r\n" +
					"\t\t\tcount++;\r\n" +
					"\t\t}\r\n" +
					"\t\treturn count;\r\n" +
					"\t}\r\n" +
					"}";
	
		[Test]
		public void ConvertedPythonCode()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string code = converter.Convert(csharp);
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef GetCount(self, items):\r\n" +
									"\t\tcount = 0\r\n" +
									"\t\tenumerator = items.GetEnumerator()\r\n" +
									"\t\twhile enumerator.MoveNext():\r\n" +
									"\t\t\titem = enumerator.Current\r\n" +
									"\t\t\tcount += 1\r\n" +
									"\t\treturn count";
		
			Assert.AreEqual(expectedCode, code);
		}
	}
}

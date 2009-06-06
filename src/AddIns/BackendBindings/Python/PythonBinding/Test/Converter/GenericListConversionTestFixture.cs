// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that the code to create an instance of a generic list is converted to python 
	/// correctly. 
	/// 
	/// C#: List<string> list = new List<string>();
	/// 
	/// Python: list = List[str]()
	/// </summary>
	[TestFixture]
	public class GenericListConversionTestFixture
	{
		string csharp = "using System.Collections.Generic;\r\n" +
						"\r\n" +
						"class Foo\r\n" +
						"{\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        List<string> list = new List<string>();\r\n" +
						"        Dictionary<string, int> dictionary = new Dictionary<string, int>();\r\n" +
						"    }\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython = "from System.Collections.Generic import *\r\n" +
									"\r\n" +
									"class Foo(object):\r\n" +
									"    def __init__(self):\r\n" +
									"        list = List[str]()\r\n" +
									"        dictionary = Dictionary[str, int]()"; 
			
			Assert.AreEqual(expectedPython, python);
		}	
	}
}

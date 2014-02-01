// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Object initializers should be converted to calling the appropriate property setters, but
	/// this means a single line statement would need to be replaced with multiple statements.
	/// </summary>
	[TestFixture]	
	public class ObjectInitializerConversionTestFixture
	{		
		string csharp = "class Class1\r\n" +
						"{\r\n" +
						"	string name = String.Empty;\r\n" +
						"	string lastName = String.Empty;\r\n" +
						"\r\n" +
						"	public string Name {\r\n" +
						"		get { return name; }\r\n" +
						"		set { name = value; }\r\n" +
						"	}\r\n" +
						"\r\n" +
						"	public string LastName {\r\n" +
						"		get { return lastName; }\r\n" +
						"		set { lastName = value; }\r\n" +
						"	}\r\n" +
						"\r\n" +
						"	public Class1 Clone()\r\n" +
						"	{\r\n" +
						"		return new Class1 { Name = \"First\", LastName = \"Last\" };\r\n" +
						"	}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Class1(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._name = String.Empty\r\n" +
									"\t\tself._lastName = String.Empty\r\n" +
									"\r\n" +
									"\tdef get_Name(self):\r\n" +
									"\t\treturn self._name\r\n" +
									"\r\n" +
									"\tdef set_Name(self, value):\r\n" +
									"\t\tself._name = value\r\n" +
									"\r\n" +
									"\tName = property(fget=get_Name, fset=set_Name)\r\n" +
									"\r\n" +
									"\tdef get_LastName(self):\r\n" +
									"\t\treturn self._lastName\r\n" +
									"\r\n" +
									"\tdef set_LastName(self, value):\r\n" +
									"\t\tself._lastName = value\r\n" +
									"\r\n" +
									"\tLastName = property(fget=get_LastName, fset=set_LastName)\r\n" +
									"\r\n" +
									"\tdef Clone(self):\r\n" +
									"\t\treturn Class1(Name = \"First\", LastName = \"Last\")";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

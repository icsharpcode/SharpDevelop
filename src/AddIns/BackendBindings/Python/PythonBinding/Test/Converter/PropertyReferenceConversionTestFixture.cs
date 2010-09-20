// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class PropertyReferenceConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    int count = 0;\r\n" +
						"    public int Count {\r\n" +
						"        get {\r\n" +
						"                return count;\r\n" +
						"        }\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    public void Increment()\r\n" +
						"    {\r\n" +
						"        Count++;\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    public void SetCount(int Count)\r\n" +
						"    {\r\n" +
						"        this.Count = Count;\r\n" +
						"    }\r\n" +	
						"}";
			
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"    def __init__(self):\r\n" +
									"        self._count = 0\r\n" +
									"\r\n" +
									"    def get_Count(self):\r\n" +
									"        return self._count\r\n" +
									"\r\n" +
									"    Count = property(fget=get_Count)\r\n" +
									"\r\n" +
									"    def Increment(self):\r\n" +
									"        self.Count += 1\r\n" +
									"\r\n" +
									"    def SetCount(self, Count):\r\n" +
									"        self.Count = Count";
			
			Assert.AreEqual(expectedPython, python, python);
		}
	}
}

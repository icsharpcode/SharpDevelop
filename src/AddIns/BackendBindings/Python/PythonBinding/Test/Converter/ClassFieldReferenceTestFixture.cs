// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that the keyword this is used when an explicit reference to a field is used.
	/// Also tests that any constructor parameters are generated.
	/// </summary>
	[TestFixture]	
	public class ClassFieldReferenceConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tint i;\r\n" +
						"\tint j;\r\n" +
						"\tpublic Foo(int i)\r\n" +
						"\t{\r\n" +
						"\t\tthis.i = i;\r\n" +
						"\t}\r\n" +
						"\r\n" +
						"\tpublic void SetInt(int j)\r\n" +
						"\t{\r\n" +
						"\t\tthis.j = j;\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self, i):\r\n" +
									"\t\tself._i = i\r\n" +
									"\r\n" +
									"\tdef SetInt(self, j):\r\n" +
									"\t\tself._j = j";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

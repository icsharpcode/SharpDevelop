// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToPythonConverter class does not add
	/// an extra constructor method to hold the statements to
	/// initialize the converted class's fields.
	/// </summary>
	[TestFixture]
	public class IntegerClassFieldWithConstructorTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tprivate int i = 0;\r\n" +
						"\tpublic Foo()\r\n" +
						"\t{\r\n" +
						"\t\tint j = 0;\r\n" +
						"\t}\r\n" +
						"}";
		
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._i = 0\r\n" +
									"\t\tj = 0";
			
			Assert.AreEqual(expectedPython, python);
		}	
	}
}

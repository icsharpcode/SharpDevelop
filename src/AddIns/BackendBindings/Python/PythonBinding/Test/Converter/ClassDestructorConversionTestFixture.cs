// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToPythonConverter class can convert a class destructor.
	/// </summary>
	[TestFixture]
	public class ClassDestructorConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tint i = 10;\r\n" +
						"\tpublic ~Foo()\r\n" +
						"\t{\r\n" +
						"\t\ti = 0;\r\n" +
						"\t}\r\n" +
						"}";
	
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._i = 10\r\n" +
									"\r\n" +
									"\tdef __del__(self):\r\n" +
									"\t\tself._i = 0";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

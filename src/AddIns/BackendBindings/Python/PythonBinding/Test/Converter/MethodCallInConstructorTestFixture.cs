// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Was causing a null reference exception in the convertor's IsStatic method since the method
	/// being passed as a parameter is null.
	/// </summary>
	[TestFixture]
	public class MethodCallInConstructorTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        Init();\r\n" +
						"    }\r\n" +
						"}";
		
		[Test]
		public void GeneratedPythonSourceCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
				"\tdef __init__(self):\r\n" +
				"\t\tself.Init()";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

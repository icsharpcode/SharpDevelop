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

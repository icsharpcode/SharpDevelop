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
	/// Tests the CSharpToPythonConverter class. Python's namespaces
	/// are determined by the file the code is in so there are no
	/// explicit namespace expression.
	/// </summary>
	[TestFixture]
	public class CSharpClassWithNamespaceConversionTestFixture
	{
		string csharp = "namespace MyNamespace\r\n" +
						"{\r\n" +
						"\tclass Foo\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"}";
			
		[Test]
		public void GeneratedPythonSourceCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

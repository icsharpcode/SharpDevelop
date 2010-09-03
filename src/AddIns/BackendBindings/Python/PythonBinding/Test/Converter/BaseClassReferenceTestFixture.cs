// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that a base class reference through the base keyword
	/// is converted to the self keyword in Python when converting
	/// from C# to Python correctly.
	/// </summary>
	[TestFixture]
	public class BaseClassReferenceTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic string Run()\r\n" +
						"\t{" +
						"\t\treturn base.ToString();\r\n" +
						"\t}\r\n" +
						"}";

		[Test]
		public void ConvertedPythonCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\treturn self.ToString()";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}

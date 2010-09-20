// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests a single class method is converted.
	/// </summary>
	[TestFixture]
	public class SingleClassMethodConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void Init()\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"}";
			
		[Test]
		public void GeneratedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Init(self):\r\n" +
									"\t\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

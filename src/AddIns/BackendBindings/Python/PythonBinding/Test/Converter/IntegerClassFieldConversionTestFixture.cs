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
	/// Tests the CSharpToPythonConverter class can convert a class with
	/// a private field that's an integer. What should happen is that
	/// since Python does not have the concept of class fields all 
	/// fields initialized in the class are moved to the start of the
	/// __init__ method. Note that this assumes there are no
	/// overloaded constructors otherwise we will get duplicated code.
	/// </summary>
	[TestFixture]
	public class IntegerClassFieldConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tprivate int i = 0;\r\n" +
						"}";
			
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._i = 0";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToPythonConverter class.
	/// </summary>
	[TestFixture]
	public class EmptyCSharpClassConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"}";
	
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void ConvertedPythonCodeUsingFourSpacesAsIndent()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = new String(' ', 4);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"    pass";
			
			Assert.AreEqual(expectedPython, python);
		}
		
	}
}

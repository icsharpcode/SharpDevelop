// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class BaseClassConversionTestFixture
	{
		string csharp = 
			"class Foo : Bar, IMyInterface\r\n" +
			"{\r\n" +
			"}";

		[Test]
		public void ConvertedRubyCode()
		{
			string expectedCode =
				"class Foo < Bar, IMyInterface\r\n" +
				"end";
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}

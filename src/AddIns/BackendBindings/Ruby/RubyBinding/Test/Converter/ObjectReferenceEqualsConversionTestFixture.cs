// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class ObjectReferenceEqualsConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public bool IsEqual(object o)\r\n" +
						"    {\r\n" +
						"        return object.ReferenceEquals(o, null);\r\n" +
						"    }\r\n" +
						"}";
				
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"class Foo\r\n" +
				"    def IsEqual(o)\r\n" +
				"        return System::Object.ReferenceEquals(o, nil)\r\n" +
				"    end\r\n" +
				"end";

			Assert.AreEqual(expectedRuby, Ruby, Ruby);
		}
	}
}

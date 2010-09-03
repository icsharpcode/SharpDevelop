// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class TypeofConversionTestFixture
	{
		string typeofIntCode = "class Foo\r\n" +
						"{\r\n" +
						"    public string ToString()\r\n" +
						"    {\r\n" +
						"        typeof(int).FullName;\r\n" +
						"    }\r\n" +
						"}";
				
		[Test]
		public void ConvertedTypeOfIntegerCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(typeofIntCode);
			string expectedRuby =
				"require \"mscorlib\"\r\n" +
				"\r\n" +
				"class Foo\r\n" +
				"    def ToString()\r\n" +
				"        System::Int32.to_clr_type.FullName\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

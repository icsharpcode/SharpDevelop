// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class BitShiftConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic int Convert()\r\n" +
						"\t{\r\n" +
						"\t\tint a = (b >> 16) & 0xffff;\r\n" +
						"\t\treturn a;\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Convert(self):\r\n" +
									"\t\ta = (b >> 16) & 0xffff\r\n" +
									"\t\treturn a";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

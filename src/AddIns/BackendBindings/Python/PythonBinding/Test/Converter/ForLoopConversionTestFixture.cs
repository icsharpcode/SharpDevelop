// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the conversion of a for loop to Python. 
	/// 
	/// C#:
	/// 
	/// for (int i = 0; i &lt; 5; ++i) {
	/// }
	/// 
	/// Python:
	/// 
	/// i = 0
	/// while i &lt; 5
	/// 	i = i + 1
	/// 
	/// Ideally we would convert it to:
	/// 
	/// for i in range(0, 5):
	/// 	pass
	/// </summary>
	[TestFixture]
	public class ForLoopConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
					"{\r\n" +
					"\tpublic int GetCount()\r\n" +
					"\t{\r\n" +
					"\t\tint count = 0;\r\n" +
					"\t\tfor (int i = 0; i < 5; i = i + 1) {\r\n" +
					"\t\t\tcount++;\r\n" +
					"\t\t}\r\n" +
					"\t\treturn count;\r\n" +
					"\t}\r\n" +
					"}";

		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef GetCount(self):\r\n" +
									"\t\tcount = 0\r\n" +
									"\t\ti = 0\r\n" +
									"\t\twhile i < 5:\r\n" +
									"\t\t\tcount += 1\r\n" +
									"\t\t\ti = i + 1\r\n" +
									"\t\treturn count";
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}

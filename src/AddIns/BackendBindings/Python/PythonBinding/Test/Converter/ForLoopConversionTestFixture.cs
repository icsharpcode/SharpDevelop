// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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

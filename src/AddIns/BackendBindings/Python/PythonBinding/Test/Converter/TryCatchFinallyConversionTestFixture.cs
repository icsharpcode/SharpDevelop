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
	/// Converts a C# try-catch-finally to python.
	/// </summary>
	[TestFixture]
	public class TryCatchFinallyConversionTestFixture
	{
		string csharp = "class Loader\r\n" +
						"{\r\n" +
						"\tpublic void load(string xml)\r\n" +
						"\t{\r\n" +
						"\t\ttry {\r\n" +
						"\t\t\tXmlDocument doc = new XmlDocument();\r\n" +
						"\t\t\tdoc.LoadXml(xml);\r\n" +
						"\t\t} catch (XmlException ex) {\r\n" +
						"\t\t\tConsole.WriteLine(ex.ToString());\r\n" +
						"\t\t} finally {\r\n" +
						"\t\t\tConsole.WriteLine(xml);\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"}";
		
		/// <summary>
		/// Note that Python seems to need to nest the try-catch
		/// inside a try-finally if the finally exists.
		/// </summary>
		[Test]
		public void ConvertedCode()
		{
			string expectedPython = "class Loader(object):\r\n" +
									"\tdef load(self, xml):\r\n" +
									"\t\ttry:\r\n" +
									"\t\t\tdoc = XmlDocument()\r\n" +
									"\t\t\tdoc.LoadXml(xml)\r\n" +
									"\t\texcept XmlException, ex:\r\n" +
									"\t\t\tConsole.WriteLine(ex.ToString())\r\n" +
									"\t\tfinally:\r\n" +
									"\t\t\tConsole.WriteLine(xml)";
			
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
		
			Assert.AreEqual(expectedPython, python);
		}
	}
}

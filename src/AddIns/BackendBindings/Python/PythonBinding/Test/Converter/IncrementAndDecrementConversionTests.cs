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
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that an integer increments and decrements are converted
	/// from C# to Python correctly.
	/// </summary>
	[TestFixture]
	public class IncrementAndDecrementConversionTests
	{	
		[Test]
		public void PreIncrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"\tpublic void Run(int i)\r\n" +
							"\t{\r\n" +
							"\t\t++i;\r\n" +
							"\t}\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\ti += 1";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}
		
		[Test]
		public void PostIncrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"\tpublic void Run(int i)\r\n" +
							"\t{\r\n" +
							"\t\ti++;\r\n" +
							"\t}\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\ti += 1";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}		
		
		[Test]
		public void PreDecrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"\tpublic void Run(int i)\r\n" +
							"\t{\r\n" +
							"\t\t--i\r\n" +
							"\t}\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\ti -= 1";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}

		[Test]
		public void PostDecrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"\tpublic void Run(int i)\r\n" +
							"\t{\r\n" +
							"\t\ti--\r\n" +
							"\t}\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\ti -= 1";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}		
		
		[Test]
		public void Add10()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"\tpublic void Run(int i)\r\n" +
							"\t{\r\n" +
							"\t\ti = i + 10\r\n" +
							"\t}\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\ti = i + 10";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}
		
		[Test]
		public void Add5()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"\tpublic void Run(int i)\r\n" +
							"\t{\r\n" +
							"\t\ti += 5\r\n" +
							"\t}\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\ti += 5";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}
		
		[Test]
		public void Subtract10()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"\tpublic void Run(int i)\r\n" +
							"\t{\r\n" +
							"\t\ti = i - 10\r\n" +
							"\t}\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\ti = i - 10";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}
	
		[Test]
		public void Subtract5()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"\tpublic void Run(int i)\r\n" +
							"\t{\r\n" +
							"\t\ti -= 5\r\n" +
							"\t}\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\ti -= 5";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}		
	}
}

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
	/// Static methods should not use "self" and they should by defined by using "staticmethod".
	/// </summary>
	[TestFixture]
	public class StaticMethodConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    static void Main(string[] args)\r\n" +
						"    {\r\n" +
						"        Stop();\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    static void Stop()\r\n" +
						"    {\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    public void Run()\r\n" +
						"    {\r\n" +
						"    }\r\n" +
						"}";
	
		string python;
		NRefactoryToPythonConverter converter;
		
		[SetUp]
		public void Init()
		{
			converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			python = converter.Convert(csharp);
		}
		
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedPython = "class Foo(object):\r\n" +
									"    def Main(args):\r\n" +
									"        Foo.Stop()\r\n" +
									"\r\n" +
									"    Main = staticmethod(Main)\r\n" +
									"\r\n" +
									"    def Stop():\r\n" +
									"        pass\r\n" +
									"\r\n" +
									"    Stop = staticmethod(Stop)\r\n" +
									"\r\n" +
									"    def Run(self):\r\n" +
									"        pass";
			Assert.AreEqual(expectedPython, python, python);
		}
		
		[Test]
		public void EntryPointMethodFound()
		{
			Assert.AreEqual(1, converter.EntryPointMethods.Count);
		}
		
		[Test]
		public void MainEntryPointMethodNameIsMain()
		{
			Assert.AreEqual("Main", converter.EntryPointMethods[0].Name);
		}
		
		[Test]
		public void GenerateCodeToCallMainMethod()
		{
			Assert.AreEqual("Foo.Main(None)", converter.GenerateMainMethodCall(converter.EntryPointMethods[0]));
		}
	}
}

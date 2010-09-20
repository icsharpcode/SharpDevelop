// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
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
	
		string Ruby;
		NRefactoryToRubyConverter converter;
		
		[SetUp]
		public void Init()
		{
			converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			Ruby = converter.Convert(csharp);
		}
		
		[Test]
		public void ConvertedRubyCode()
		{
			string expectedRuby =
				"class Foo\r\n" +
				"    def Foo.Main(args)\r\n" +
				"        Foo.Stop()\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def Foo.Stop()\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def Run()\r\n" +
				"    end\r\n" +
				"end";
			Assert.AreEqual(expectedRuby, Ruby, Ruby);
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
			Assert.AreEqual("Foo.Main(nil)", converter.GenerateMainMethodCall(converter.EntryPointMethods[0]));
		}
	}
}

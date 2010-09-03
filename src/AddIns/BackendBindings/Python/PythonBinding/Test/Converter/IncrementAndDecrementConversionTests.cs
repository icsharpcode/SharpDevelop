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

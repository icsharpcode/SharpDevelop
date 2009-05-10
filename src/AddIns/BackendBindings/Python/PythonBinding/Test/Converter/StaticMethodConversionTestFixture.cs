// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"    def Main(args):\r\n" +
									"        pass\r\n" +
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
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests the generation of code to call the class's Main method.
	/// </summary>
	[TestFixture]
	public class GenerateMainMethodCallTestFixture
	{
		string mainMethodWithNoParametersCode = "class Foo\r\n" +
						"{\r\n" +
						"    static void Main()\r\n" +
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
		public void GeneratedMainMethodCallWithNoParametersCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "  ";
			converter.Convert(mainMethodWithNoParametersCode);
			string code = converter.GenerateMainMethodCall(converter.EntryPointMethods[0]);	
			Assert.AreEqual("Foo.Main()", code);
		}
	}
}

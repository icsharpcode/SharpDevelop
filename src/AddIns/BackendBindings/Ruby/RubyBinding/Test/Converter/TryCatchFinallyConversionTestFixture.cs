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
	/// Converts a C# try-catch-finally to Ruby.
	/// </summary>
	[TestFixture]
	public class TryCatchFinallyConversionTestFixture
	{
		string csharp = "class Loader\r\n" +
						"{\r\n" +
						"    public void load(string xml)\r\n" +
						"    {\r\n" +
						"        try {\r\n" +
						"            XmlDocument doc = new XmlDocument();\r\n" +
						"            doc.LoadXml(xml);\r\n" +
						"        } catch (XmlException ex) {\r\n" +
						"            Console.WriteLine(ex.ToString());\r\n" +
						"        } finally {\r\n" +
						"            Console.WriteLine(xml);\r\n" +
						"        }\r\n" +
						"    }\r\n" +
						"}";
		
		[Test]
		public void ConvertedCode()
		{
			string expectedRuby =
				"class Loader\r\n" +
				"    def load(xml)\r\n" +
				"        begin\r\n" +
				"            doc = XmlDocument.new()\r\n" +
				"            doc.LoadXml(xml)\r\n" +
				"        rescue XmlException => ex\r\n" +
				"            Console.WriteLine(ex.ToString())\r\n" +
				"        ensure\r\n" +
				"            Console.WriteLine(xml)\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
		
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

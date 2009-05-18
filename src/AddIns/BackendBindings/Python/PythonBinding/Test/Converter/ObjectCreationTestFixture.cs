// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that C# code that creates a new XmlDocument object
	/// is converted to Python correctly.
	/// </summary>
	[TestFixture]
	public class ObjectCreationTestFixture
	{	
		string csharp = "class Foo\r\n" +
					"{\r\n" +
					"\tpublic Foo()\r\n" +
					"\t{\r\n" +
					"\t\tXmlDocument doc = new XmlDocument();\r\n" +
					"\t\tdoc.LoadXml(\"<root/>\");\r\n" +
					"\t}\r\n" +
					"}";
		
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tdoc = XmlDocument()\r\n" +
									"\t\tdoc.LoadXml(\"<root/>\")";
			
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that C# code that creates a new XmlDocument object
	/// is converted to Ruby correctly.
	/// </summary>
	[TestFixture]
	public class ObjectCreationTestFixture
	{	
		string csharp = "class Foo\r\n" +
					"{\r\n" +
					"    public Foo()\r\n" +
					"    {\r\n" +
					"        XmlDocument doc = new XmlDocument();\r\n" +
					"        doc.LoadXml(\"<root/>\");\r\n" +
					"    }\r\n" +
					"}";
		
		[Test]
		public void ConvertedRubyCode()
		{
			string expectedRuby =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        doc = XmlDocument.new()\r\n" +
				"        doc.LoadXml(\"<root/>\")\r\n" +
				"    end\r\n" +
				"end";
			
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

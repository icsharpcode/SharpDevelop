// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Object initializers should be converted to calling the appropriate property setters, but
	/// this means a single line statement would need to be replaced with multiple statements.
	/// </summary>
	[TestFixture]
	public class ObjectInitializerConversionTestFixture
	{		
		string csharp = "class Class1\r\n" +
						"{\r\n" +
						"	string name = String.Empty;\r\n" +
						"	string lastName = String.Empty;\r\n" +
						"\r\n" +
						"	public string Name {\r\n" +
						"		get { return name; }\r\n" +
						"		set { name = value; }\r\n" +
						"	}\r\n" +
						"\r\n" +
						"	public string LastName {\r\n" +
						"		get { return lastName; }\r\n" +
						"		set { lastName = value; }\r\n" +
						"	}\r\n" +
						"\r\n" +
						"	public Class1 Clone()\r\n" +
						"	{\r\n" +
						"		return new Class1 { Name = \"First\", LastName = \"Last\" };\r\n" +
						"	}\r\n" +
						"}";
				
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"class Class1\r\n" +
				"    def initialize()\r\n" +
				"        @name = String.Empty\r\n" +
				"        @lastName = String.Empty\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def Name\r\n" +
				"        return @name\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def Name=(value)\r\n" +
				"        @name = value\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def LastName\r\n" +
				"        return @lastName\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def LastName=(value)\r\n" +
				"        @lastName = value\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def Clone()\r\n" +
				"        return Class1.new(Name = \"First\", LastName = \"Last\")\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby, Ruby);
		}
	}
}

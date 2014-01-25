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

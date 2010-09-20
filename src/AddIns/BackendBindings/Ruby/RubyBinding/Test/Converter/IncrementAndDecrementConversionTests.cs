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
	/// <summary>
	/// Tests that an integer increments and decrements are converted
	/// from C# to Ruby correctly.
	/// </summary>
	[TestFixture]
	public class IncrementAndDecrementConversionTests
	{	
		[Test]
		public void PreIncrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        ++i;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i += 1\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void PostIncrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i++;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i += 1\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}		
		
		[Test]
		public void PreDecrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        --i\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i -= 1\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}

		[Test]
		public void PostDecrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i--\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i -= 1\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}		
		
		[Test]
		public void Add10()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i = i + 10\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i = i + 10\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void Add5()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i += 5\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i += 5\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void Subtract10()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i = i - 10\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i = i - 10\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
	
		[Test]
		public void Subtract5()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i -= 5\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i -= 5\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
	}
}

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
	public class UnaryOperatorConversionTests
	{	
		[Test]
		public void MinusOne()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i = -1;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"\tdef Run(i)\r\n" +
									"\t\ti = -1\r\n" +
									"\tend\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void PlusOne()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i = +1;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"\tdef Run(i)\r\n" +
									"\t\ti = +1\r\n" +
									"\tend\r\n" +
									"end";
		
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void NotOperator()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(bool i)\r\n" +
							"    {\r\n" +
							"        j = !i;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"\tdef Run(i)\r\n" +
									"\t\tj = not i\r\n" +
									"\tend\r\n" +
									"end";
		
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}		
		
		[Test]
		public void BitwiseNotOperator()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(bool i)\r\n" +
							"    {\r\n" +
							"        j = ~i;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"\tdef Run(i)\r\n" +
									"\t\tj = ~i\r\n" +
									"\tend\r\n" +
									"end";
		
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
	}
}

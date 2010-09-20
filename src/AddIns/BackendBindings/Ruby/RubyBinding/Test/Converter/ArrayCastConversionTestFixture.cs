// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that an array cast is correctly converted to Ruby.
	/// </summary>
	[TestFixture]
	public class ArrayCastConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public void Assign()\r\n" +
						"    {\r\n" +
						"        int[] elements = new int[10];\r\n" +
						"        List<int[]> list = new List<int[]>();\r\n" + 
						"        list.Add((int[])elements.Clone());\r\n" +
						"    }\r\n" +
						"}";
		
		[Test]
		public void GeneratedRubySourceCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"class Foo\r\n" +
				"    def Assign()\r\n" +
				"        elements = Array.CreateInstance(System::Int32, 10)\r\n" +
				"        list = List[Array[System::Int32]].new()\r\n" +
				"        list.Add(elements.Clone())\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

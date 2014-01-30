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
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class PythonParserParsePropertyTests
	{
		IProperty property;
		
		void ParseClassWithProperty()
		{
			string code =
				"class MyClass:\r\n" +
				"    def __init__(self):\r\n" +
        		"        self._count = 0\r\n" +
 				"\r\n" +
    			"    def get_Count(self):\r\n" +
        		"        return self._count\r\n" +
				"\r\n" +
 				"    def _set_Count(self, value):\r\n" +
        		"        self._count = value\r\n" +
				"\r\n" +
    			"    Count = property(fget=get_Count, fset=set_Count)\r\n";
			
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			property = parseInfo.CompilationUnit.Classes[0].Properties[0];
		}
		
		[Test]
		public void Parse_ClassHasPropertyCalledCount_ReturnParseInfoWithClassWithPropertyCalledCount()
		{
			ParseClassWithProperty();
			string name = property.Name;
			
			string expectedName = "Count";
			
			Assert.AreEqual(expectedName, name);
		}
		
		/// <summary>
		/// Dom regions are one based.
		/// </summary>
		[Test]
		public void Parse_ClassHasPropertyCalledCount_PropertyRegion()
		{
			ParseClassWithProperty();
			DomRegion region = property.Region;
			
			DomRegion expectedRegion = new DomRegion(
				beginLine: 11,
				beginColumn: 5,
				endLine: 11,
				endColumn: 5
			);
			
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void Parse_ClassMethodHasNoPropertyButHasAssignmentStatementSetToValueFromFunctionCall_ParseInfoHasNoPropertyAdded()
		{
			string code =
				"class MyClass:\r\n" +
				"    a = foo()\r\n" +
				"\r\n";
			
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			int count = parseInfo.CompilationUnit.Classes[0].Properties.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Parse_ClassMethodHasNoPropertyButHasAssignmentStatementUsingMemberExpression_ParseInfoHasNoPropertyAddedAndNoExceptionThrown()
		{
			string code =
				"class MyClass:\r\n" +
				"    a.b = foo()\r\n" +
				"\r\n";
			
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			int count = parseInfo.CompilationUnit.Classes[0].Properties.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Parse_ClassMethodHasNoPropertyButHasAssignmentStatementSetToIntegerValue_ParseInfoHasNoPropertyAddedAndNoExceptionThrown()
		{
			string code =
				"class MyClass:\r\n" +
				"    a = 1\r\n" +
				"\r\n";
			
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			int count = parseInfo.CompilationUnit.Classes[0].Properties.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Parse_ClassMethodHasNoPropertyButHasAssignmentStatementSetToValueFromMemberExpressionCall_ParseInfoHasNoPropertyAddedAndNoExceptionThrown()
		{
			string code =
				"class MyClass:\r\n" +
				"    a = foo.bar()\r\n" +
				"\r\n";
			
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			int count = parseInfo.CompilationUnit.Classes[0].Properties.Count;
			
			Assert.AreEqual(0, count);
		}
	}
}

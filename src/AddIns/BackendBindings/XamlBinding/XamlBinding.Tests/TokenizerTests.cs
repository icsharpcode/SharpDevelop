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
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class TokenizerTests
	{
		[Test]
		public void SimpleTest()
		{
			string input = "Test";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "Test");
		}
		
		[Test]
		public void PropertyGroupTest()
		{
			string input = "Test.Count";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "Test", ".", "Count");
		}
		
		[Test]
		public void PropertyGroupDotEndTest()
		{
			string input = "Test.";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "Test", ".");
		}
		
		[Test]
		public void PropertyIndexerSimpleTest()
		{
			string input = "Test[1]";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "Test", "[", "1", "]");
		}
		
		[Test]
		public void PropertyIndexerMultiTest()
		{
			string input = "Test[1,2]";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "Test", "[", "1", ",", "2", "]");
		}
		
		[Test]
		public void PropertyAtIndexerTest()
		{
			string input = "Test[";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "Test", "[");
		}
		
		[Test]
		public void PropertyAtCommaTest()
		{
			string input = "Test[1,";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "Test", "[", "1", ",");
		}
		
		[Test]
		public void PropertyGroupAfterIndexTest()
		{
			string input = "Test[1].Property";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "Test", "[", "1", "]", ".", "Property");
		}
		
		[Test]
		public void PropertyDotAfterIndexTest()
		{
			string input = "Test[1].";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "Test", "[", "1", "]", ".");
		}
		
		[Test]
		public void PropertyIndexAtStartTest()
		{
			string input = "[1].";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "[", "1", "]", ".");
		}
		
		[Test]
		public void PropertySourceTraversalTest()
		{
			string input = "propertyName/propertyNameX";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "propertyName", "/", "propertyNameX");
		}
		
		[Test]
		public void WhitespaceTest()
		{
			string input = "        propertyName        /         propertyNameX [ 1 , 2 ] .property";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "propertyName", "/", "propertyNameX", "[", "1", ",", "2", "]", ".", "property");
		}
		
		[Test]
		public void AttachedPropertiesTest()
		{
			string input = "(typeName.propertyName).(otherType.otherProperty).property";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "(", "typeName", ".", "propertyName", ")", ".", "(", "otherType", ".", "otherProperty", ")", ".", "property");
		}
		
		[Test]
		public void InvalidStringTest()
		{
			string input = "(typeName&.propert$$yName).(##otherType . otherProperty).property";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "(", "typeName", ".", "propert", "yName", ")", ".", "(", "otherType", ".", "otherProperty", ")", ".", "property");
		}
		
		[Test]
		public void InCompletionSituationTest1()
		{
			string input = "(typeName.";
			string[] result = PropertyPathTokenizer.Tokenize(input).ToArray();
			
			CompareResults(result, "(", "typeName", ".");
		}
		
		void CompareResults(string[] result, params string[] expected)
		{
			Assert.AreEqual(expected, result);
		}
	}
}

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
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that all the binary operators are converted to
	/// Ruby correctly.
	/// </summary>
	[TestFixture]
	public class BinaryOperatorConversionTests
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public int Run(i)\r\n" +
						"    {\r\n" +
						"        if (i BINARY_OPERATOR 0) {\r\n" +
						"            return 10;\r\n" +
						"        }\r\n" +
						"        return 0;\r\n" +
						"    }\r\n" +
						"}";
		
		[Test]
		public void GreaterThan()
		{
			Assert.AreEqual(">", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.GreaterThan));
		}

		[Test]
		public void NotEqual()
		{
			Assert.AreEqual("!=", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.InEquality));
		}
		
		[Test]
		public void Divide()
		{
			Assert.AreEqual("/", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Divide));
		}
		
		[Test]
		public void LessThan()
		{
			Assert.AreEqual("<", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.LessThan));
		}

		[Test]
		public void Equals()
		{
			string code = GetCode(@"==");
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string RubyCode = converter.Convert(code);
			string expectedRubyCode = "class Foo\r\n" +
						"    def Run(i)\r\n" +
						"        if i == 0 then\r\n" +
						"            return 10\r\n" +
						"        end\r\n" +
						"        return 0\r\n" +
						"    end\r\n" +
						"end";
			Assert.AreEqual(expectedRubyCode, RubyCode);
		}

		[Test]
		public void LessThanOrEqual()
		{
			Assert.AreEqual("<=", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.LessThanOrEqual));
		}

		[Test]
		public void GreaterThanOrEqual()
		{
			Assert.AreEqual(">=", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.GreaterThanOrEqual));
		}
		
		[Test]
		public void Add()
		{
			Assert.AreEqual("+", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Add));
		}
		
		[Test]
		public void Multiply()
		{
			Assert.AreEqual("*", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Multiply));
		}
		
		[Test]
		public void BitwiseAnd()
		{
			Assert.AreEqual("&", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.BitwiseAnd));
		}

		[Test]
		public void BitwiseOr()
		{
			Assert.AreEqual("|", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.BitwiseOr));
		}

		[Test]
		public void BooleanAnd()
		{
			Assert.AreEqual("and", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.LogicalAnd));
		}

		[Test]
		public void BooleanOr()
		{
			Assert.AreEqual("or", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.LogicalOr));
		}
		
		[Test]
		public void BooleanXor()
		{
			Assert.AreEqual("^", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.ExclusiveOr));
		}		

		[Test]
		public void Modulus()
		{
			Assert.AreEqual("%", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Modulus));
		}

		[Test]
		public void Subtract()
		{
			Assert.AreEqual("-", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Subtract));
		}
		
		[Test]
		public void DivideInteger()
		{
			Assert.AreEqual("/", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.DivideInteger));
		}

		[Test]
		public void ReferenceEquality()
		{
			Assert.AreEqual("is", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.ReferenceEquality));
		}
		
		[Test]
		public void BitShiftRight()
		{
			Assert.AreEqual(">>", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.ShiftRight));
		}
		
		[Test]
		public void BitShiftLeft()
		{
			Assert.AreEqual("<<", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.ShiftLeft));
		}
	
		/// <summary>
		/// Gets the C# code with the binary operator replaced with the
		/// specified string.
		/// </summary>
		string GetCode(string op)
		{
			return csharp.Replace("BINARY_OPERATOR", op);
		}
	}
}

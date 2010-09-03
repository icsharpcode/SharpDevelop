// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that all the binary operators are converted to
	/// Python correctly.
	/// </summary>
	[TestFixture]
	public class BinaryOperatorConversionTests
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic int Run(i)\r\n" +
						"\t{\r\n" +
						"\t\tif (i BINARY_OPERATOR 0) {\r\n" +
						"\t\t\treturn 10;\r\n" +
						"\t\t}\r\n" +
						"\treturn 0;\r\n" +
						"}";
		
		[Test]
		public void GreaterThan()
		{
			Assert.AreEqual(">", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.GreaterThan));
		}

		[Test]
		public void NotEqual()
		{
			Assert.AreEqual("!=", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.InEquality));
		}
		
		[Test]
		public void Divide()
		{
			Assert.AreEqual("/", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.Divide));
		}
		
		[Test]
		public void LessThan()
		{
			Assert.AreEqual("<", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.LessThan));
		}

		[Test]
		public void Equals()
		{
			string code = GetCode(@"==");
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string pythonCode = converter.Convert(code);
			string expectedPythonCode = "class Foo(object):\r\n" +
						"\tdef Run(self, i):\r\n" +
						"\t\tif i == 0:\r\n" +
						"\t\t\treturn 10\r\n" +
						"\t\treturn 0";
			Assert.AreEqual(expectedPythonCode, pythonCode);
		}

		[Test]
		public void LessThanOrEqual()
		{
			Assert.AreEqual("<=", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.LessThanOrEqual));
		}

		[Test]
		public void GreaterThanOrEqual()
		{
			Assert.AreEqual(">=", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.GreaterThanOrEqual));
		}
		
		[Test]
		public void Add()
		{
			Assert.AreEqual("+", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.Add));
		}
		
		[Test]
		public void Multiply()
		{
			Assert.AreEqual("*", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.Multiply));
		}
		
		[Test]
		public void BitwiseAnd()
		{
			Assert.AreEqual("&", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.BitwiseAnd));
		}

		[Test]
		public void BitwiseOr()
		{
			Assert.AreEqual("|", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.BitwiseOr));
		}

		[Test]
		public void BooleanAnd()
		{
			Assert.AreEqual("and", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.LogicalAnd));
		}

		[Test]
		public void BooleanOr()
		{
			Assert.AreEqual("or", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.LogicalOr));
		}
		
		[Test]
		public void BooleanXor()
		{
			Assert.AreEqual("^", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.ExclusiveOr));
		}		

		[Test]
		public void Modulus()
		{
			Assert.AreEqual("%", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.Modulus));
		}

		[Test]
		public void Subtract()
		{
			Assert.AreEqual("-", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.Subtract));
		}
		
		[Test]
		public void DivideInteger()
		{
			Assert.AreEqual("/", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.DivideInteger));
		}

		[Test]
		public void ReferenceEquality()
		{
			Assert.AreEqual("is", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.ReferenceEquality));
		}
		
		[Test]
		public void BitShiftRight()
		{
			Assert.AreEqual(">>", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.ShiftRight));
		}
		
		[Test]
		public void BitShiftLeft()
		{
			Assert.AreEqual("<<", NRefactoryToPythonConverter.GetBinaryOperator(BinaryOperatorType.ShiftLeft));
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

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
			Assert.AreEqual(">", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.GreaterThan));
		}

		[Test]
		public void NotEqual()
		{
			Assert.AreEqual("!=", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.InEquality));
		}
		
		[Test]
		public void Divide()
		{
			Assert.AreEqual("/", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.Divide));
		}
		
		[Test]
		public void LessThan()
		{
			Assert.AreEqual("<", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.LessThan));
		}

		[Test]
		public void Equals()
		{
			string code = GetCode(@"==");
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
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
			Assert.AreEqual("<=", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.LessThanOrEqual));
		}

		[Test]
		public void GreaterThanOrEqual()
		{
			Assert.AreEqual(">=", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.GreaterThanOrEqual));
		}
		
		[Test]
		public void Add()
		{
			Assert.AreEqual("+", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.Add));
		}
		
		[Test]
		public void Multiply()
		{
			Assert.AreEqual("*", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.Multiply));
		}
		
		[Test]
		public void BitwiseAnd()
		{
			Assert.AreEqual("&", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.BitwiseAnd));
		}

		[Test]
		public void BitwiseOr()
		{
			Assert.AreEqual("|", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.BitwiseOr));
		}

		[Test]
		public void BooleanAnd()
		{
			Assert.AreEqual("and", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.LogicalAnd));
		}

		[Test]
		public void BooleanOr()
		{
			Assert.AreEqual("or", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.LogicalOr));
		}

		[Test]
		public void Modulus()
		{
			Assert.AreEqual("%", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.Modulus));
		}

		[Test]
		public void Subtract()
		{
			Assert.AreEqual("-", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.Subtract));
		}
		
		[Test]
		public void DivideInteger()
		{
			Assert.AreEqual("/", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.DivideInteger));
		}

		[Test]
		public void ReferenceEquality()
		{
			Assert.AreEqual("is", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.ReferenceEquality));
		}
		
		[Test]
		public void BitShiftRight()
		{
			Assert.AreEqual(">>", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.ShiftRight));
		}
		
		[Test]
		public void BitShiftLeft()
		{
			Assert.AreEqual("<<", CSharpToPythonConverter.GetBinaryOperator(BinaryOperatorType.ShiftLeft));
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

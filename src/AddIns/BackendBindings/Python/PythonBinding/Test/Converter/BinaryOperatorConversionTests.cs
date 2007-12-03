// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
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
			string code = GetCode(">");
			CodeBinaryOperatorExpression expression = GetBinaryOperatorExpression(code);
			Assert.AreEqual(CodeBinaryOperatorType.GreaterThan, expression.Operator);
		}

		[Test]
		public void NotEqual()
		{
			string code = GetCode("!=");
			CodeBinaryOperatorExpression expression = GetBinaryOperatorExpression(code);
			Assert.AreEqual(CodeBinaryOperatorType.IdentityInequality, expression.Operator);
		}
		
		[Test]
		public void Divide()
		{
			string code = GetCode("/ 5 >");
			CodeBinaryOperatorExpression expression = GetBinaryOperatorExpression(code);
			CodeBinaryOperatorExpression divideExpression = expression.Left as CodeBinaryOperatorExpression;
			Assert.AreEqual(CodeBinaryOperatorType.Divide, divideExpression.Operator);
		}
		
		[Test]
		public void LessThan()
		{
			string code = GetCode("<");
			CodeBinaryOperatorExpression expression = GetBinaryOperatorExpression(code);
			Assert.AreEqual(CodeBinaryOperatorType.LessThan, expression.Operator);
		}

		[Test]
		public void Equals()
		{
			string code = GetCode("==");
			CodeBinaryOperatorExpression expression = GetBinaryOperatorExpression(code);
			Assert.AreEqual(CodeBinaryOperatorType.ValueEquality, expression.Operator);
		}

		[Test]
		public void LessThanOrEqual()
		{
			string code = GetCode("<=");
			CodeBinaryOperatorExpression expression = GetBinaryOperatorExpression(code);
			Assert.AreEqual(CodeBinaryOperatorType.LessThanOrEqual, expression.Operator);
		}

		[Test]
		public void GreaterThanOrEqual()
		{
			string code = GetCode(">=");
			CodeBinaryOperatorExpression expression = GetBinaryOperatorExpression(code);
			Assert.AreEqual(CodeBinaryOperatorType.GreaterThanOrEqual, expression.Operator);
		}
		
		[Test]
		public void Add()
		{
			CodeBinaryOperatorType type = NRefactoryToPythonConverter.ConvertBinaryOperatorType(BinaryOperatorType.Add);
			Assert.AreEqual(CodeBinaryOperatorType.Add, type);
		}
		
		[Test]
		public void BitwiseAnd()
		{
			CodeBinaryOperatorType type = NRefactoryToPythonConverter.ConvertBinaryOperatorType(BinaryOperatorType.BitwiseAnd);
			Assert.AreEqual(CodeBinaryOperatorType.BitwiseAnd, type);
		}

		[Test]
		public void BitwiseOr()
		{
			CodeBinaryOperatorType type = NRefactoryToPythonConverter.ConvertBinaryOperatorType(BinaryOperatorType.BitwiseOr);
			Assert.AreEqual(CodeBinaryOperatorType.BitwiseOr, type);
		}

		[Test]
		public void BooleanAnd()
		{
			CodeBinaryOperatorType type = NRefactoryToPythonConverter.ConvertBinaryOperatorType(BinaryOperatorType.LogicalAnd);
			Assert.AreEqual(CodeBinaryOperatorType.BooleanAnd, type);
		}

		[Test]
		public void BooleanOr()
		{
			CodeBinaryOperatorType type = NRefactoryToPythonConverter.ConvertBinaryOperatorType(BinaryOperatorType.LogicalOr);
			Assert.AreEqual(CodeBinaryOperatorType.BooleanOr, type);
		}

		[Test]
		public void Modulus()
		{
			CodeBinaryOperatorType type = NRefactoryToPythonConverter.ConvertBinaryOperatorType(BinaryOperatorType.Modulus);
			Assert.AreEqual(CodeBinaryOperatorType.Modulus, type);
		}

		[Test]
		public void Subtract()
		{
			CodeBinaryOperatorType type = NRefactoryToPythonConverter.ConvertBinaryOperatorType(BinaryOperatorType.Subtract);
			Assert.AreEqual(CodeBinaryOperatorType.Subtract, type);
		}
		
		[Test]
		public void DivideInteger()
		{
			CodeBinaryOperatorType type = NRefactoryToPythonConverter.ConvertBinaryOperatorType(BinaryOperatorType.DivideInteger);
			Assert.AreEqual(CodeBinaryOperatorType.Divide, type);
		}

		[Test]
		public void ReferenceEquality()
		{
			CodeBinaryOperatorType type = NRefactoryToPythonConverter.ConvertBinaryOperatorType(BinaryOperatorType.ReferenceEquality);
			Assert.AreEqual(CodeBinaryOperatorType.IdentityEquality, type);
		}
		
		/// <summary>
		/// Gets the C# code with the binary operator replaced with the
		/// specified string.
		/// </summary>
		string GetCode(string op)
		{
			return csharp.Replace("BINARY_OPERATOR", op);
		}
		
		/// <summary>
		/// Gets the binary operator for the if statement for this
		/// test fixture.
		/// </summary>
		CodeBinaryOperatorExpression GetBinaryOperatorExpression(string csharpCode)
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			CodeCompileUnit unit = converter.ConvertToCodeCompileUnit(csharpCode);
			if (unit.Namespaces.Count > 0) {
				CodeNamespace ns = unit.Namespaces[0];
				if (ns.Types.Count > 0) {
					CodeTypeDeclaration type = ns.Types[0];
					if (type.Members.Count > 0) {
						CodeMemberMethod method = type.Members[0] as CodeMemberMethod;
						if (method != null) {
							if (method.Statements.Count > 0) {
								CodeConditionStatement conditionStatement = method.Statements[0] as CodeConditionStatement;
								if (conditionStatement != null) {
									return conditionStatement.Condition as CodeBinaryOperatorExpression;
								}
							}
						}
					}
				}
			}
			return null;
		}		
	}
}

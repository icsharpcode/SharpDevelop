// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the conversion of a for loop to Python. 
	/// 
	/// C#:
	/// 
	/// for (int i = 0; i &lt; 5; ++i) {
	/// }
	/// 
	/// Python:
	/// 
	/// i = 0
	/// while i &lt; 5
	/// 	i = i + 1
	/// 
	/// Ideally we would convert it to:
	/// 
	/// for i in range(0, 5):
	/// 	pass
	/// 
	/// but this is not possible using the code dom.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class ForLoopConversionTestFixture
	{
		CodeIterationStatement forStatement;
		CodeMemberMethod method;
		CodeAssignStatement postIncrementStatement;
		
		string csharp = "class Foo\r\n" +
					"{\r\n" +
					"\tpublic int GetCount()\r\n" +
					"\t{\r\n" +
					"\t\tint count = 0;\r\n" +
					"\t\tfor (int i = 0; i < 5; i = i + 1) {\r\n" +
					"\t\t\tcount++;\r\n" +
					"\t\t}\r\n" +
					"\t\treturn count;\r\n" +
					"\t}\r\n" +
					"}";
			
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CSharpToPythonConverter	converter = new CSharpToPythonConverter();
			CodeCompileUnit codeCompileUnit = converter.ConvertToCodeCompileUnit(csharp);
			if (codeCompileUnit.Namespaces.Count > 0) {
				CodeNamespace codeNamespace = codeCompileUnit.Namespaces[0];
				if (codeNamespace.Types.Count > 0) {
					CodeTypeDeclaration codeTypeDeclaration = codeNamespace.Types[0];
					if (codeTypeDeclaration.Members.Count > 0) {
						method = codeTypeDeclaration.Members[0] as CodeMemberMethod;
						if (method != null) {
							foreach (CodeStatement statement in method.Statements) {
								if (statement is CodeIterationStatement) {
									forStatement = (CodeIterationStatement)statement;
									if (forStatement.Statements.Count > 0) {
										postIncrementStatement = forStatement.Statements[0] as CodeAssignStatement;
									}
								}
							}
						}
					}
				}
			}			
		}

		[Test]
		public void ConvertedPythonCode()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string code = converter.Convert(csharp);
			System.Console.WriteLine(code);
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef GetCount(self):\r\n" +
									"\t\tcount = 0\r\n" +
									"\t\ti = 0\r\n" +
									"\t\twhile i < 5:\r\n" +
									"\t\t\tcount = count + 1\r\n" +
									"\t\t\ti = i + 1\r\n" +
									"\t\treturn count";
		
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void ForStatementExists()
		{
			Assert.IsNotNull(forStatement);
		}
		
		[Test]
		public void ForInitStatementIsVariableDeclarationStatement()
		{
			Assert.IsInstanceOfType(typeof(CodeVariableDeclarationStatement), forStatement.InitStatement);
		}
		
		[Test]
		public void ForTestExpressionIsBinaryOperatorExpression()
		{
			Assert.IsInstanceOfType(typeof(CodeBinaryOperatorExpression), forStatement.TestExpression);
		}
		
		[Test]
		public void ForIncrementStatementIsAssignStatement()
		{
			Assert.IsInstanceOfType(typeof(CodeAssignStatement), forStatement.IncrementStatement);
		}
		
		[Test]
		public void ForTestExpressionBinaryOperatorTypeIsLessThan()
		{
			CodeBinaryOperatorExpression binaryOperatorExpression = forStatement.TestExpression as CodeBinaryOperatorExpression;
			Assert.AreEqual(CodeBinaryOperatorType.LessThan, binaryOperatorExpression.Operator);
		}		
		
		[Test]
		public void ForStatementHasOneChildStatement()
		{
			Assert.AreEqual(1, forStatement.Statements.Count);
		}
		
		[Test]
		public void ForStatementChildStatementIsAssignStatement()
		{
			Assert.IsInstanceOfType(typeof(CodeAssignStatement), forStatement.Statements[0]);
		}
		
		[Test]
		public void PostIncrementStatementLhsIsVariableReference()
		{
			Assert.IsInstanceOfType(typeof(CodeVariableReferenceExpression), postIncrementStatement.Left);
		}
		
		[Test]
		public void PostIncrementStatementRhsIsBinaryOperator()
		{
			Assert.IsInstanceOfType(typeof(CodeBinaryOperatorExpression), postIncrementStatement.Right);
		}
		
		[Test]
		public void BinaryOperatorLhsIsCodeVariableReference()
		{
			CodeBinaryOperatorExpression binaryOperatorExpression = postIncrementStatement.Right as CodeBinaryOperatorExpression;
			Assert.IsInstanceOfType(typeof(CodeVariableReferenceExpression), binaryOperatorExpression.Left);
		}
		
		[Test]
		public void BinaryOperatorLhsIsEqualToPostIncrementStatementLhs()
		{
			CodeBinaryOperatorExpression binaryOperatorExpression = postIncrementStatement.Right as CodeBinaryOperatorExpression;
			Assert.AreSame(postIncrementStatement.Left, binaryOperatorExpression.Left);
		}
		
		[Test]
		public void BinaryOperatorTypeIsAdd()
		{
			CodeBinaryOperatorExpression binaryOperatorExpression = postIncrementStatement.Right as CodeBinaryOperatorExpression;
			Assert.AreEqual(CodeBinaryOperatorType.Add, binaryOperatorExpression.Operator);
		}
		
		[Test]
		public void BinaryOperatorRhsIsPrimitiveExpression()
		{
			CodeBinaryOperatorExpression binaryOperatorExpression = postIncrementStatement.Right as CodeBinaryOperatorExpression;
			Assert.IsInstanceOfType(typeof(CodePrimitiveExpression), binaryOperatorExpression.Right);
		}		
		
		[Test]
		public void BinaryOperatorRhsPrimitiveExpressionHasIntegerValueOfOne()
		{
			CodeBinaryOperatorExpression binaryOperatorExpression = postIncrementStatement.Right as CodeBinaryOperatorExpression;
			CodePrimitiveExpression primitiveExpression = binaryOperatorExpression.Right as CodePrimitiveExpression;
			Assert.AreEqual(1, primitiveExpression.Value);
		}		
	}
}

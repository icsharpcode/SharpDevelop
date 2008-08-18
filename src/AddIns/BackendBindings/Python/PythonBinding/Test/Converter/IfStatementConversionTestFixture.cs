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
	/// Tests the conversion of a C# if-else statement to python.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class IfStatementConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeMemberMethod method;
		CodeConditionStatement ifStatement;
		CodeBinaryOperatorExpression binaryOperatorExpression;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tint i = 0;\r\n" +
						"\tpublic int GetCount()\r\n" +
						"\t{" +
						"\t\tif (i == 0) {\r\n" +
						"\t\t\treturn 10;\r\n" +
						"\t\t} else {\r\n" +
						"\t\t\treturn 4;\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"}";
				
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			converter = new CSharpToPythonConverter();
			codeCompileUnit = converter.ConvertToCodeCompileUnit(csharp);
			if (codeCompileUnit.Namespaces.Count > 0) {
				codeNamespace = codeCompileUnit.Namespaces[0];
				if (codeNamespace.Types.Count > 0) {
					codeTypeDeclaration = codeNamespace.Types[0];
					if (codeTypeDeclaration.Members.Count > 0) {
						foreach (CodeTypeMember member in codeTypeDeclaration.Members) {
							if ((member is CodeMemberMethod) && (member.Name == "GetCount")) {
								method = (CodeMemberMethod)member;
							}
						}
						if ((method != null) && (method.Statements.Count > 0)) {
							ifStatement = method.Statements[0] as CodeConditionStatement;
							if (ifStatement != null) {
								binaryOperatorExpression = ifStatement.Condition as CodeBinaryOperatorExpression;
							}
						}
					}
				}
			}			
		}
		
		[Test]
		public void ConvertedPythonCode()
		{
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._i = 0\r\n" +
									"\t\r\n" +
									"\tdef GetCount(self):\r\n" +
									"\t\tif self._i == 0:\r\n" +
									"\t\t\treturn 10\r\n" +
									"\t\telse:\r\n" +
									"\t\t\treturn 4";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void IfStatementExists()
		{
			Assert.IsNotNull(ifStatement);
		}
		
		[Test]
		public void IfStatementConditionIsBinaryOperatorExpression()
		{
			Assert.IsInstanceOfType(typeof(CodeBinaryOperatorExpression), ifStatement.Condition);
		}
		
		[Test]
		public void BinaryOperatorIsValueEquality()
		{
			Assert.AreEqual(CodeBinaryOperatorType.ValueEquality, binaryOperatorExpression.Operator);
		}
		
		[Test]
		public void BinaryOperatorLhsIsFieldReference()
		{
			Assert.IsInstanceOfType(typeof(CodeFieldReferenceExpression), binaryOperatorExpression.Left);
		}
		
		[Test]
		public void BinaryOperatorRightIsPrimitiveExpression()
		{
			Assert.IsInstanceOfType(typeof(CodePrimitiveExpression), binaryOperatorExpression.Right);
		}		
		
		[Test]
		public void ConditionHasOneTrueStatement()
		{
			Assert.AreEqual(1, ifStatement.TrueStatements.Count);
		}
		
		[Test]
		public void TrueStatementIsReturnStatement()
		{
			Assert.IsInstanceOfType(typeof(CodeMethodReturnStatement), ifStatement.TrueStatements[0]);
		}
		
		[Test]
		public void ConditionHasOneFalseStatement()
		{
			Assert.AreEqual(1, ifStatement.FalseStatements.Count);
		}

		[Test]
		public void FalseStatementIsReturnStatement()
		{
			Assert.IsInstanceOfType(typeof(CodeMethodReturnStatement), ifStatement.FalseStatements[0]);
		}		
	}
}

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
	/// Converts a C# else if statement to Python's elif.
	/// </summary>
	[TestFixture]
	public class ElseIfStatementConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeMemberMethod method;
		CodeConditionStatement ifStatement;
		CodeConditionStatement elseIfStatement;
		CodeBinaryOperatorExpression elseIfBinaryOperator;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic int GetCount(i)\r\n" +
						"\t{" +
						"\t\tif (i == 0) {\r\n" +
						"\t\t\treturn 10;\r\n" +
						"\t\t} else if (i < 1){\r\n" +
						"\t\t\treturn 4;\r\n" +
						"\t\t}\r\n" +
						"\t\treturn 2;\r\n" +
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
						if ((method != null) && (method.Statements.Count > 1)) {
							ifStatement = method.Statements[0] as CodeConditionStatement;
							if (ifStatement != null) {
								if (ifStatement.FalseStatements.Count > 0) {
									elseIfStatement = ifStatement.FalseStatements[0] as CodeConditionStatement;
									if (elseIfStatement != null) {
										elseIfBinaryOperator = elseIfStatement.Condition as CodeBinaryOperatorExpression;
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
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef GetCount(self, i):\r\n" +
									"\t\tif i == 0:\r\n" +
									"\t\t\treturn 10\r\n" +
									"\t\telse:\r\n" +
									"\t\t\tif i < 1:\r\n" +
									"\t\t\t\treturn 4\r\n" +
									"\t\treturn 2";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void ElseIfStatementExists()
		{
			Assert.IsNotNull(elseIfStatement);
		}		
		
		[Test]
		public void ElseIfBinaryOperatorIsLessThan()
		{
			Assert.AreEqual(CodeBinaryOperatorType.LessThan, elseIfBinaryOperator.Operator);
		}
		
		[Test]
		public void ElseIfBodyIsReturnStatement()
		{
			Assert.IsInstanceOfType(typeof(CodeMethodReturnStatement), elseIfStatement.TrueStatements[0]);
		}
		
		[Test]
		public void ElseIfBodyHasOneStatement()
		{
			Assert.AreEqual(1, elseIfStatement.TrueStatements.Count);
		}
	}
}

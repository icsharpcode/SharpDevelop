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
	/// Tests the CSharpToPythonConverter class can convert a method with
	/// simple integer assignment in the body.
	/// </summary>
	[TestFixture]
	public class MethodWithBodyConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeMemberMethod method;
		CodeVariableDeclarationStatement variableDeclarationStatement;
		CodePrimitiveExpression variableDeclarationExpression;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void Run()\r\n" +
						"\t{\r\n" +
						"\t\tint i = 0;\r\n" +
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
						method = codeTypeDeclaration.Members[0] as CodeMemberMethod;
						if (method.Statements.Count > 0) {
							variableDeclarationStatement = method.Statements[0] as CodeVariableDeclarationStatement;
							if (variableDeclarationStatement != null) {
								variableDeclarationExpression = variableDeclarationStatement.InitExpression as CodePrimitiveExpression;
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
									"\tdef Run(self):\r\n" +
									"\t\ti = 0";
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void MethodHasOneStatement()
		{
			Assert.AreEqual(1, method.Statements.Count);
		}
		
		[Test]
		public void MethodVariableDeclarationStatementExists()
		{
			Assert.IsNotNull(variableDeclarationStatement);
		}
		
		[Test]
		public void VariableDeclarationIsInt()
		{
			Assert.AreEqual("System.Int32", variableDeclarationStatement.Type.BaseType);
		}
		
		[Test]
		public void VariableDeclarationName()
		{
			Assert.AreEqual("i", variableDeclarationStatement.Name);
		}
		
		[Test]
		public void VariableHasInitExpression()
		{
			Assert.IsNotNull(variableDeclarationExpression);
		}
		
		[Test]
		public void VariableInitExpressionIsIntegerZero()
		{
			Assert.AreEqual(0, variableDeclarationExpression.Value);
		}
	}
}

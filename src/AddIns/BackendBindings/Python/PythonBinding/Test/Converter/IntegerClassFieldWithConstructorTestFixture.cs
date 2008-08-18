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
	/// Tests the CSharpToPythonConverter class does not add
	/// an extra constructor method to hold the statements to
	/// initialize the converted class's fields.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class IntegerClassFieldWithConstructorTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeConstructor constructor;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeVariableDeclarationStatement variableDeclarationStatement;
		CodePrimitiveExpression variableDeclarationExpression;

		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tprivate int i = 0;\r\n" +
						"\tpublic Foo()\r\n" +
						"\t{\r\n" +
						"\t\tint j = 0;\r\n" +
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
						constructor = codeTypeDeclaration.Members[0] as CodeConstructor;
						if (constructor.Statements.Count > 1) {
							// First statement should be the field initialization.							
							// Second statement should be the local variable assignment.
							variableDeclarationStatement = constructor.Statements[1] as CodeVariableDeclarationStatement;
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
									"\tdef __init__(self):\r\n" +
									"\t\tself._i = 0\r\n" +
									"\t\tj = 0";
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void ClassHasOneConstructor()
		{
			int constructorCount = 0;
			foreach (CodeTypeMember member in codeTypeDeclaration.Members) {
				if (member is CodeConstructor) {
					++constructorCount;
				}
			}
			
			Assert.AreEqual(1, constructorCount);
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
			Assert.AreEqual("j", variableDeclarationStatement.Name);
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

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
	/// Tests the CSharpToPythonConverter correctly converts a local
	/// variable in the constructor.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class LocalVariableAssignedInConstructorTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeConstructor constructor;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeAssignStatement assignStatement;
		CodeVariableReferenceExpression variableReferenceExpression;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic Foo()\r\n" +
						"\t{\r\n" +
						"\t\tint i = 0;\r\n" +
						"\t\ti = 2;\r\n" +
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
							// First statement is the local variable initializer.							
							// Second statement is the local variable assignment.
							assignStatement = constructor.Statements[1] as CodeAssignStatement;
							if (assignStatement != null) {
								variableReferenceExpression = assignStatement.Left as CodeVariableReferenceExpression;
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
									"\t\ti = 0\r\n" +
									"\t\ti = 2";
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void ClassHasNoFields()
		{
			int fieldCount = 0;
			foreach (CodeTypeMember member in codeTypeDeclaration.Members) {
				if (member is CodeMemberField) {
					fieldCount++;
				}
			}
			Assert.AreEqual(0, fieldCount);
		}
		
		[Test]
		public void AssignStatementExists()
		{
			Assert.IsNotNull(assignStatement);
		}
		
		[Test]
		public void AssignLhsIsVariableReferenceExpression()
		{
			Assert.IsNotNull(variableReferenceExpression);
		}
		
		[Test]
		public void VariableNameUsedInVariableReferenceExpression()
		{
			Assert.AreEqual("i", variableReferenceExpression.VariableName);
		}
	}
}

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
	/// Tests the CSharpToPythonConverter correctly converts the class
	/// constructor when a value is assigned to one of its fields.
	/// </summary>
	[TestFixture]
	public class IntegerClassFieldInitializedInConstructorTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeConstructor constructor;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeAssignStatement assignStatement;
		CodeFieldReferenceExpression fieldReferenceExpression;
		CodePrimitiveExpression primitiveExpression;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tprivate int i;\r\n" +
						"\tpublic Foo()\r\n" +
						"\t{\r\n" +
						"\t\ti = 0;\r\n" +
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
						if (constructor.Statements.Count > 0) {
							assignStatement = constructor.Statements[0] as CodeAssignStatement;
							fieldReferenceExpression = assignStatement.Left as CodeFieldReferenceExpression;
							primitiveExpression = assignStatement.Right as CodePrimitiveExpression;
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
									"\t\tself._i = 0";
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void ConstructorHasOneStatement()
		{
			Assert.AreEqual(1, constructor.Statements.Count);
		}
		
		[Test]
		public void AssignStatementLeftIsFieldReferenceExpression()
		{
			Assert.IsNotNull(fieldReferenceExpression);
		}
		
		[Test]
		public void AssignStatementRightIsPrimitiveExpression()
		{
			Assert.IsNotNull(primitiveExpression);
		}
		
		[Test]
		public void FieldName()
		{
			Assert.AreEqual("_i", fieldReferenceExpression.FieldName);
		}
		
		[Test]
		public void FieldTargetObjectIsThisReference()
		{
			Assert.IsInstanceOfType(typeof(CodeThisReferenceExpression), fieldReferenceExpression.TargetObject);
		}
		
		[Test]
		public void PrimitiveExpressionIsIntegerZero()
		{
			Assert.AreEqual(0, primitiveExpression.Value);
		}		
		
		[Test]
		public void AssignStatementExists()
		{
			Assert.IsNotNull(assignStatement);
		}
	}
}

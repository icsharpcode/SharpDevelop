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
	/// Tests that C# code such as "System.Console.WriteLine("Test");"
	/// is converted to Python code correctly.
	/// </summary>
	[TestFixture]
	public class StaticClassReferenceTestFixture
	{
		CodeConstructor constructor;
		CodeExpressionStatement expressionStatement;
		CodeMethodInvokeExpression methodInvokeExpression;
		CodeMethodReferenceExpression methodReferenceExpression;
		CodeFieldReferenceExpression fieldReferenceExpression;
		CodeVariableReferenceExpression variableReferenceExpression;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic Foo()\r\n" +
						"\t{\r\n" +
						"\t\tSystem.Console.WriteLine(\"Test\");\r\n" +
						"\t}\r\n" +
						"}";
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			CodeCompileUnit codeCompileUnit = converter.ConvertToCodeCompileUnit(csharp);
			if (codeCompileUnit.Namespaces.Count > 0) {
				CodeNamespace codeNamespace = codeCompileUnit.Namespaces[0];
				if (codeNamespace.Types.Count > 0) {
					CodeTypeDeclaration codeTypeDeclaration = codeNamespace.Types[0];
					if (codeTypeDeclaration.Members.Count > 0) {
						constructor = codeTypeDeclaration.Members[0] as CodeConstructor;
						if (constructor.Statements.Count > 0) {
							expressionStatement = constructor.Statements[0] as CodeExpressionStatement;
							if (expressionStatement != null) {
								methodInvokeExpression = expressionStatement.Expression as CodeMethodInvokeExpression;
								if (methodInvokeExpression != null) {
									methodReferenceExpression = methodInvokeExpression.Method;
									if (methodReferenceExpression != null) {
										fieldReferenceExpression = methodReferenceExpression.TargetObject as CodeFieldReferenceExpression;
										if (fieldReferenceExpression != null) {
											variableReferenceExpression = fieldReferenceExpression.TargetObject as CodeVariableReferenceExpression;
										}
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
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tSystem.Console.WriteLine('Test')";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void ExpressionStatementExists()
		{
			Assert.IsNotNull(expressionStatement);
		}

		[Test]
		public void MethodInvokeExpressionExists()
		{
			Assert.IsNotNull(methodInvokeExpression);
		}
		
		[Test]
		public void MethodReferenceExpressionExists()
		{
			Assert.IsNotNull(methodReferenceExpression);
		}
		
		[Test]
		public void MethodReferenceIsWriteLine()
		{
			Assert.AreEqual("WriteLine", methodReferenceExpression.MethodName);
		}
		
		[Test]
		public void FieldReferenceExpressionExists()
		{
			Assert.IsNotNull(fieldReferenceExpression);
		}
		
		[Test]
		public void FieldReferenceName()
		{
			Assert.AreEqual("Console", fieldReferenceExpression.FieldName);
		}
		
		[Test]
		public void VariableReferenceExpressionExists()
		{
			Assert.IsNotNull(variableReferenceExpression);
		}

		[Test]
		public void VariableReferenceName()
		{
			Assert.IsNotNull("System", variableReferenceExpression.VariableName);
		}
		
		[Test]
		public void MethodInvokeHasOneParameter()
		{
			Assert.AreEqual(1, methodInvokeExpression.Parameters.Count);
		}
		
		[Test]
		public void MethodInvokeParameterIsPrimitiveExpression()
		{
			Assert.IsInstanceOfType(typeof(CodePrimitiveExpression), methodInvokeExpression.Parameters[0]);
		}
		
		[Test]
		public void MethodInvokeParameterString()
		{
			CodePrimitiveExpression primitiveExpression = methodInvokeExpression.Parameters[0] as CodePrimitiveExpression;
			Assert.AreEqual("Test", primitiveExpression.Value);
		}
	}
}

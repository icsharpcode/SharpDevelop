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
	/// Tests the conversion of a foreach loop to Python. Since
	/// the code dom does not support foreach we use the IEnumerator interface.
	/// 
	/// C#:
	/// 
	/// foreach (int i in items) {
	/// }
	/// 
	/// Code DOM equivalent:
	/// 
	/// for (System.Collection.Enumerator iterator = items.GetEnumerator();
	/// 	iterator.MoveNext(); )
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class ForeachConversionTestFixture
	{
		CodeIterationStatement foreachStatement;
		CodeMemberMethod method;
		CodeVariableDeclarationStatement variableDeclaration;
		CodeMethodInvokeExpression testExpression;
		CodeMethodInvokeExpression variableDeclarationMethodInvoke;
		CodeVariableDeclarationStatement forLoopVariableDeclaration;
		
		string csharp = "class Foo\r\n" +
					"{\r\n" +
					"\tpublic int GetCount(int[] items)\r\n" +
					"\t{\r\n" +
					"\t\tint count = 0;\r\n" +
					"\t\tforeach (int item in items) {\r\n" +
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
									foreachStatement = (CodeIterationStatement)statement;
									if (foreachStatement != null) {
										variableDeclaration = foreachStatement.InitStatement as CodeVariableDeclarationStatement;
										testExpression = foreachStatement.TestExpression as CodeMethodInvokeExpression;
										if (variableDeclaration != null) {
											variableDeclarationMethodInvoke = variableDeclaration.InitExpression as CodeMethodInvokeExpression;
										}
										if (foreachStatement.Statements.Count > 0) {
											forLoopVariableDeclaration = foreachStatement.Statements[0] as CodeVariableDeclarationStatement;
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
			string code = converter.Convert(csharp);
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef GetCount(self, items):\r\n" +
									"\t\tcount = 0\r\n" +
									"\t\tenumerator = items.GetEnumerator()\r\n" +
									"\t\twhile enumerator.MoveNext():\r\n" +
									"\t\t\titem = enumerator.Current\r\n" +
									"\t\t\tcount = count + 1\r\n" +
									"\t\treturn count";
		
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void ForeachStatementExists()
		{
			Assert.IsNotNull(foreachStatement);
		}
		
		[Test]
		public void ForeachHasTwoStatementsInBody()
		{
			Assert.AreEqual(2, foreachStatement.Statements.Count);
		}
		
		[Test]
		public void ForeachBodySecondStatementIsAssignStatement()
		{
			Assert.IsInstanceOfType(typeof(CodeAssignStatement), foreachStatement.Statements[1]);
		}
		
		[Test]
		public void TestExpressionIsMethodInvoke()
		{
			Assert.IsInstanceOfType(typeof(CodeMethodInvokeExpression), foreachStatement.TestExpression);
		}
		
		[Test]
		public void ForeachInitializerStatementIsVariableDeclaration()
		{
			Assert.IsInstanceOfType(typeof(CodeVariableDeclarationStatement), foreachStatement.InitStatement);
		}

		[Test]
		public void VariableDeclarationInitStatementIsMethodInvoke()
		{
			Assert.IsInstanceOfType(typeof(CodeMethodInvokeExpression), variableDeclaration.InitExpression);
		}
		
		[Test]
		public void TestExpressionMethodName()
		{
			Assert.AreEqual("MoveNext", testExpression.Method.MethodName);
		}
		
		[Test]
		public void VariableDeclarationMethodInvokeName()
		{
			Assert.AreEqual("GetEnumerator", variableDeclarationMethodInvoke.Method.MethodName);
		}
		
		[Test]
		public void MethodInvokeMethodReferenceTargetObjectIsVariableReference()
		{
			Assert.IsInstanceOfType(typeof(CodeVariableReferenceExpression), variableDeclarationMethodInvoke.Method.TargetObject);
		}
		
		[Test]
		public void GetEnumeratorTargetObjectIsVariableReference()
		{
			CodeVariableReferenceExpression variableRef = variableDeclarationMethodInvoke.Method.TargetObject as CodeVariableReferenceExpression;
			Assert.AreEqual("items", variableRef.VariableName);
		}
		
		[Test]
		public void VariableDeclarationNameIsEnumerator()
		{
			Assert.AreEqual("enumerator", variableDeclaration.Name);
		}
		
		[Test]
		public void TestExpressionVariableNameIsEnumerator()
		{
			CodeVariableReferenceExpression variableRef = testExpression.Method.TargetObject as CodeVariableReferenceExpression;
			Assert.AreEqual("enumerator", variableRef.VariableName);
		}

		/// <summary>
		/// The first statement in the for loop's body should be 
		/// "i = enumerator.Current".
		/// </summary>
		[Test]
		public void ForeachBodyVariableDeclarationExists()
		{
			Assert.IsNotNull(forLoopVariableDeclaration);
		}
		
		[Test]
		public void ForeachBodyVariableDeclarationName()
		{
			Assert.AreEqual("item", forLoopVariableDeclaration.Name);
		}
		
		[Test]
		public void ForeachBodyVariableDeclarationInitExpressionIsPropertyRef()
		{
			Assert.IsInstanceOfType(typeof(CodePropertyReferenceExpression), forLoopVariableDeclaration.InitExpression);
		}
		
		[Test]
		public void ForeachBodyVariableDeclarationInitExpressionPropertyRefName()
		{
			CodePropertyReferenceExpression propertyRef = forLoopVariableDeclaration.InitExpression as CodePropertyReferenceExpression;
			Assert.AreEqual("Current", propertyRef.PropertyName);
		}
		
		[Test]
		public void ForeachBodyVariableDeclarationInitExpressionPropertyRefTargetObjectIsEnumerator()
		{
			CodePropertyReferenceExpression propertyRef = forLoopVariableDeclaration.InitExpression as CodePropertyReferenceExpression;
			CodeVariableReferenceExpression variableRef = propertyRef.TargetObject as CodeVariableReferenceExpression;
			
			Assert.AreEqual("enumerator", variableRef.VariableName);
		}
	}
}

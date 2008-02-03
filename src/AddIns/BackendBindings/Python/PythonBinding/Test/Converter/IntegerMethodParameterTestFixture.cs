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
	/// Tests that a method call is converted correctly.
	/// </summary>
	[TestFixture]
	public class IntegerMethodParameterTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public void Print()\r\n" +
						"    {\r\n" +
						"        int i = 0;\r\n" +
						"        PrintInt(i);\r\n" +
						"    }\r\n" +
						"}";
		
		CodeMethodReferenceExpression methodRefExpression;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			CodeCompileUnit unit = converter.ConvertToCodeCompileUnit(csharp);
			if (unit.Namespaces.Count > 0) {
				CodeNamespace ns = unit.Namespaces[0];
				if (ns.Types.Count > 0) {
					CodeTypeDeclaration type = ns.Types[0];
					if (type.Members.Count > 0) {
						CodeMemberMethod method = type.Members[0] as CodeMemberMethod;
						if (method != null && method.Statements.Count > 0) {
							CodeExpressionStatement statement = method.Statements[method.Statements.Count - 1] as CodeExpressionStatement;
							if (statement != null) {
								CodeMethodInvokeExpression methodInvokeExpression = statement.Expression as CodeMethodInvokeExpression;
								if (methodInvokeExpression != null) {
									methodRefExpression = methodInvokeExpression.Method;
								}
							}
						}
					}
				}
			}
		}
		
		[Test]
		public void MethodRefExpressionExists()
		{
			Assert.IsNotNull(methodRefExpression);
		}
		
		[Test]
		public void MethodNameIsPrintInt()
		{
			Assert.AreEqual("PrintInt", methodRefExpression.MethodName);
		}
		
		[Test]
		public void MethodRefTargetObjectIsThisRef()
		{
			Assert.IsInstanceOfType(typeof(CodeThisReferenceExpression), methodRefExpression.TargetObject);
		}
		
		[Test]
		public void GeneratedPythonSourceCode()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
				"\tdef Print(self):\r\n" +
				"\t\ti = 0\r\n" +
				"\t\tself.PrintInt(i)";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

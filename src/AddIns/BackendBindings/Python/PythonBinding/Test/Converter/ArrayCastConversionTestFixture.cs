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
	/// Tests that an array cast is correctly converted to Python.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class ArrayCastConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public void Assign()\r\n" +
						"    {\r\n" +
						"        int[] elements = new int[10];\r\n" +
						"        List<int[]> list = new List<int[]>();\r\n" + 
						"        list.Add((int[])elements.Clone());\r\n" +
						"    }\r\n" +
						"}";
		
		CodeExpression expression;
		
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
								if (methodInvokeExpression != null && methodInvokeExpression.Parameters.Count > 0) {
									expression = methodInvokeExpression.Parameters[0];
								}
							}
						}
					}
				}
			}
		}
		
		[Test]
		public void CodeExpressionExists()
		{
			Assert.IsNotNull(expression);
		}
		
		[Test]
		public void GeneratedPythonSourceCode()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
				"\tdef Assign(self):\r\n" +
				"\t\telements = System.Array.CreateInstance(int,0)\r\n" +
				"\t\tlist = List()\r\n" +
				"\t\tlist.Add(elements.Clone())";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}

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
	/// Tests that a base class reference through the base keyword
	/// is converted to the self keyword in Python when converting
	/// from C# to Python correctly.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class BaseClassReferenceTestFixture
	{
		CodeMethodInvokeExpression methodInvoke;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic string Run()\r\n" +
						"\t{" +
						"\t\treturn base.ToString();\r\n" +
						"\t}\r\n" +
						"}";
		
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
							CodeMethodReturnStatement returnStatement = method.Statements[0] as CodeMethodReturnStatement;
							if (returnStatement != null) {
								methodInvoke = returnStatement.Expression as CodeMethodInvokeExpression;
							}	
						}
					}
				}
			}
		}
		
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\treturn self.ToString()";
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void MethodInvokeTargetIsThisReference()
		{
			Assert.IsInstanceOfType(typeof(CodeThisReferenceExpression), methodInvoke.Method.TargetObject);
		}
		
		[Test]
		public void MethodInvokeExists()
		{
			Assert.IsNotNull(methodInvoke);
		}
	}
}

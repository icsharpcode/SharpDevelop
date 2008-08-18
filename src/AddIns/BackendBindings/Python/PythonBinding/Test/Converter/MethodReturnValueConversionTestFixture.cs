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
	/// Tests the CSharpToPythonConverter class can convert a method that
	/// returns an integer.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class MethodReturnValueConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeMemberMethod method;
		CodeMethodReturnStatement returnStatement;
		CodePrimitiveExpression primitiveExpression;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic int Run()\r\n" +
						"\t{\r\n" +
						"\t\treturn 10;\r\n" +
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
							returnStatement = method.Statements[0] as CodeMethodReturnStatement;
							if (returnStatement != null) {
								primitiveExpression = returnStatement.Expression as CodePrimitiveExpression;
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
									"\t\treturn 10";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void ReturnStatementExists()
		{
			Assert.IsNotNull(returnStatement);
		}
		
		[Test]
		public void ReturnExpressionIsPrimitiveExpression()
		{
			Assert.IsNotNull(primitiveExpression);
		}
	}
}

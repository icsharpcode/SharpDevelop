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
	/// Tests that a throw statement is converted to a
	/// raise keyword in Python when converting
	/// from C#.
	/// </summary>
	[TestFixture]
	public class ThrowExceptionConversionTestFixture
	{		
		CodeThrowExceptionStatement throwException;
		CodeMemberMethod method;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic string Run()\r\n" +
						"\t{" +
						"\t\tthrow new XmlException();\r\n" +
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
						method = type.Members[0] as CodeMemberMethod;
						if (method != null && method.Statements.Count > 0) {
							throwException = method.Statements[0] as CodeThrowExceptionStatement;
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
									"\t\traise XmlException()";
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}	
		
		[Test]
		public void ThrowExceptionStatementExists()
		{
			Assert.IsNotNull(throwException);
		}
		
		[Test]
		public void ThrowExceptionTypeIsCreateObject()
		{
			Assert.IsInstanceOfType(typeof(CodeObjectCreateExpression), throwException.ToThrow);
		}
	}
}

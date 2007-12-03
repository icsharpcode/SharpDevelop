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
	/// Tests the conversion of an if-else statement where the
	/// if and else blocks both have more than one statement.
	/// </summary>
	[TestFixture]
	public class IfBlockStatementConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeMemberMethod method;
		CodeConditionStatement ifStatement;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tint i = 0;\r\n" +
						"\tpublic int GetCount()\r\n" +
						"\t{" +
						"\t\tif (i == 0) {\r\n" +
						"\t\ti = 10;\r\n" +
						"\t\t\treturn 10;\r\n" +
						"\t\t} else {\r\n" +
						"\t\ti = 4;\r\n" +
						"\t\t\treturn 4;\r\n" +
						"\t\t}\r\n" +
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
						foreach (CodeTypeMember member in codeTypeDeclaration.Members) {
							if ((member is CodeMemberMethod) && (member.Name == "GetCount")) {
								method = (CodeMemberMethod)member;
							}
						}
						if ((method != null) && (method.Statements.Count > 0)) {
							ifStatement = method.Statements[0] as CodeConditionStatement;
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
									"\t\r\n" +
									"\tdef GetCount(self):\r\n" +
									"\t\tif self._i == 0:\r\n" +
									"\t\t\tself._i = 10\r\n" +
									"\t\t\treturn 10\r\n" +
									"\t\telse:\r\n" +
									"\t\t\tself._i = 4\r\n" +
									"\t\t\treturn 4";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void IfStatementIsFirstMethodStatement()
		{
			Assert.IsInstanceOfType(typeof(CodeConditionStatement), method.Statements[0]);
		}
	}
}

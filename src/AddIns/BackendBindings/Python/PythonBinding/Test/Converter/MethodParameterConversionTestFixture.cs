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
	/// Tests that method parameters are converted into Python
	/// correctly.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class MethodParameterConversionTestFixture
	{
		CodeMemberMethod method;
		CodeParameterDeclarationExpression parameter;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic int Run(int i)\r\n" +
						"\t{\r\n" +
						"\t\treturn i;\r\n" +
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
						if (method.Parameters.Count > 0) {
							parameter = method.Parameters[0];
						}
					}
				}
			}
		}
		
		[Test]
		public void ConvertedPythonCode()
		{
			CSharpToPythonConverter	converter = new CSharpToPythonConverter();
			string code = converter.Convert(csharp);
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\treturn i";
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void MethodHasOneParameter()
		{
			Assert.AreEqual(1, method.Parameters.Count);
		}
		
		[Test]
		public void ParameterName()
		{
			Assert.AreEqual("i", parameter.Name);
		}
	}
}

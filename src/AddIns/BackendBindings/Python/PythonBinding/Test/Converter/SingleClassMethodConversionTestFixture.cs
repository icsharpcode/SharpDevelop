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
	/// Tests a single class method is converted.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class SingleClassMethodConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration typeDeclaration;
		CodeMemberMethod method;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void Init()\r\n" +
						"\t{\r\n" +
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
					typeDeclaration = codeNamespace.Types[0];
					if (typeDeclaration.Members.Count > 0) {
						method = typeDeclaration.Members[0] as CodeMemberMethod;
					}
				}
			}
		}
			
		[Test]
		public void GeneratedPythonCode()
		{
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Init(self):\r\n" +
									"\t\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}

		[Test]
		public void ClassHasOneMember()
		{
			Assert.AreEqual(1, typeDeclaration.Members.Count);
		}
		
		[Test]
		public void MethodName()
		{
			Assert.AreEqual("Init", method.Name);
		}
		
		[Test]
		public void MethodIsPublic()
		{
			Assert.AreEqual(method.Attributes, MemberAttributes.Public);
		}
		
		[Test]
		public void MethodHasAcceptsUserDataKey()
		{
			Assert.IsTrue(method.UserData.Contains("HasAccepts"));
		}
		
		[Test]
		public void MethodHasAcceptsUserDataIsFalse()
		{
			Assert.IsFalse((bool)method.UserData["HasAccepts"]);
		}
		
		[Test]
		public void MethodHasReturnsUserDataKey()
		{
			Assert.IsTrue(method.UserData.Contains("HasReturns"));
		}
		
		[Test]
		public void MethodHasReturnsUserDataIsFalse()
		{
			Assert.IsFalse((bool)method.UserData["HasReturns"]);
		}		
	}
}

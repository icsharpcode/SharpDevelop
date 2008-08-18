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
	/// Tests the CSharpToPythonConverter class can convert a 
	/// class constructor.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class ClassConstructorConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
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
				}
			}
		}
			
		[Test]
		public void EmptyClass()
		{
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object): pass";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void OneRootNamespace()
		{
			Assert.AreEqual(1, codeCompileUnit.Namespaces.Count);
		}
		
		[Test]
		public void NamespaceHasNoName()
		{
			Assert.AreEqual(String.Empty, codeNamespace.Name);
		}
		
		[Test]
		public void OneClassInRootNamespace()
		{
			Assert.AreEqual(1, codeNamespace.Types.Count);
		}
		
		[Test]
		public void ClassName()
		{
			Assert.AreEqual("Foo", codeTypeDeclaration.Name);
		}
		
		[Test]
		public void ClassIsPrivateAndFinal()
		{
			MemberAttributes expectedAttributes = MemberAttributes.Private | MemberAttributes.Final;
			Assert.AreEqual(expectedAttributes, codeTypeDeclaration.Attributes);
		}
		
		[Test]
		public void ClassUserDataHasSlotsIsFalse()
		{
			Assert.IsFalse((bool)codeTypeDeclaration.UserData["HasSlots"]);
		}
		
		[Test]
		public void ClassUserDataHasSlotsExists()
		{
			Assert.IsTrue(codeTypeDeclaration.UserData.Contains("HasSlots"));
		}
	}
}

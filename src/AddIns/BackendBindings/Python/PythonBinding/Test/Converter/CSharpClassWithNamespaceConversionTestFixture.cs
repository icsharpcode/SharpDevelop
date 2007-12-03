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
	/// Tests the CSharpToPythonConverter class. Python's namespaces
	/// are determined by the file the code is in so there are no
	/// explicit namespace expression.
	/// </summary>
	[TestFixture]
	public class CSharpClassWithNamespaceConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		
		string csharp = "namespace MyNamespace\r\n" +
						"{\r\n" +
						"\tclass Foo\r\n" +
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
					codeTypeDeclaration = codeNamespace.Types[0];
				}
			}
		}
			
		[Test]
		public void GeneratedPythonSourceCode()
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
	}
}

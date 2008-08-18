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
	/// Tests that a VB.NET class is converted to Python successfully.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class VBClassConversionTestFixture
	{
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		
		string vb = "Namespace DefaultNamespace\r\n" +
					"\tPublic Class Class1\r\n" +
					"\tEnd Class\r\n" +
					"End Namespace";

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			VBNetToPythonConverter converter = new VBNetToPythonConverter();
			codeCompileUnit = converter.ConvertToCodeCompileUnit(vb);
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
			VBNetToPythonConverter converter = new VBNetToPythonConverter();
			string python = converter.Convert(vb);
			string expectedPython = "class Class1(object): pass";
			
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
			Assert.AreEqual("Class1", codeTypeDeclaration.Name);
		}
	}
}

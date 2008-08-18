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
	/// Tests the CSharpToPythonConverter class.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class EmptyCSharpClassConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeConstructor constructor;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic Foo()\r\n" +
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
					if (codeTypeDeclaration.Members.Count > 0) {
						constructor = codeTypeDeclaration.Members[0] as CodeConstructor;
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
									"\t\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void ConstructorExists()
		{
			Assert.IsNotNull(constructor);
		}
		
		[Test]
		public void ConstructorName()
		{
			Assert.AreEqual("Foo", constructor.Name);
		}		
	}
}

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
	/// Tests the C# to Python converter does not add an 
	/// assignment for a variable declaration that has no 
	/// initial value assigned.
	/// </summary>
	[TestFixture]
	public class FieldDeclarationWithNoInitializerTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeConstructor constructor;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeVariableDeclarationStatement variableDeclarationStatement;

		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tprivate int i;\r\n" +
						"\tpublic Foo()\r\n" +
						"\t{\r\n" +
						"\t\tint j = 0;\r\n" +
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
						if (constructor.Statements.Count > 0) {
							// First statement should be the local variable assignment.
							// There should be no field defined since it is
							// never used or initialized in the class.
							variableDeclarationStatement = constructor.Statements[0] as CodeVariableDeclarationStatement;
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
									"\t\tj = 0";
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void ConstructorHasOneStatement()
		{
			Assert.AreEqual(1, constructor.Statements.Count);
		}

		[Test]
		public void VariableAssignmentExists()
		{
			Assert.IsNotNull(variableDeclarationStatement);
		}
	}
}

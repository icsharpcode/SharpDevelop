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
	/// Tests that C# code that creates a new XmlDocument object
	/// is converted to Python correctly.
	/// </summary>
	[TestFixture]
	public class ObjectCreationTestFixture
	{
		CodeObjectCreateExpression objectCreateExpression;
		CodeVariableDeclarationStatement variableDeclarationStatement;
		
		string csharp = "class Foo\r\n" +
					"{\r\n" +
					"\tpublic Foo()\r\n" +
					"\t{\r\n" +
					"\t\tXmlDocument doc = new XmlDocument();\r\n" +
					"\t\tdoc.LoadXml(\"<root/>\");\r\n" +
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
						CodeConstructor ctor = type.Members[0] as CodeConstructor;
						if (ctor != null && ctor.Statements.Count > 0) {
							variableDeclarationStatement = ctor.Statements[0] as CodeVariableDeclarationStatement;
							if (variableDeclarationStatement != null) {
								objectCreateExpression = variableDeclarationStatement.InitExpression as CodeObjectCreateExpression;
							}
						}
					}
				}
			}
		}
		
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tdoc = XmlDocument()\r\n" +
									"\t\tdoc.LoadXml('<root/>')";
			
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void VariableDeclarationStatementExists()
		{
			Assert.IsNotNull(variableDeclarationStatement);
		}
		
		[Test]
		public void VariableName()
		{
			Assert.AreEqual("doc", variableDeclarationStatement.Name);
		}
		
		[Test]
		public void ObjectCreateExpressionExists()
		{
			Assert.IsNotNull(objectCreateExpression);
		}
		
		[Test]
		public void ObjectCreateExpressionType()
		{
			Assert.AreEqual("XmlDocument", objectCreateExpression.CreateType.BaseType);
		}
	}
}

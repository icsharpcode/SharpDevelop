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
	/// Tests that removing a method from an event handler is converted
	/// from C# to Python correctly.
	/// </summary>
	[TestFixture]
	public class RemoveHandlerConversionTestFixture
	{
		CodeRemoveEventStatement removeEventStatement;
		CodeDelegateCreateExpression createDelegateExpression;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic Foo()\r\n" +
						"\t{" +
						"\t\tbutton = new Button();\r\n" +
						"\t\tbutton.Click -= ButtonClick;\r\n" +
						"\t}\r\n" +
						"\r\n" +
						"\tvoid ButtonClick(object sender, EventArgs e)\r\n" +
						"\t{\r\n" +
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
						if (ctor != null && ctor.Statements.Count > 1) {
							removeEventStatement = ctor.Statements[1] as CodeRemoveEventStatement;
							if (removeEventStatement != null) {
								createDelegateExpression = removeEventStatement.Listener as CodeDelegateCreateExpression;
							}
						}
					}
				}
			}
		}
		
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tbutton = Button()\r\n" +
									"\t\tbutton.Click -= ButtonClick\r\n" +
									"\t\r\n" +
									"\tdef ButtonClick(self, sender, e):\r\n" +
									"\t\tpass";
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}

		[Test]
		public void RemoveHandlerStatementExists()
		{
			Assert.IsNotNull(removeEventStatement);
		}
		
		[Test]
		public void AttachTargetObjectIsFieldRef()
		{
			Assert.IsInstanceOfType(typeof(CodeFieldReferenceExpression), removeEventStatement.Event.TargetObject);
		}
		
		[Test]
		public void ListenerExists()
		{
			Assert.IsNotNull(removeEventStatement.Listener);
		}
		
		[Test]
		public void ListenerIsCreateDelegateExpression()
		{
			Assert.IsInstanceOfType(typeof(CodeDelegateCreateExpression), removeEventStatement.Listener);
		}
		
		[Test]
		public void CreateDelegateMethodName()
		{
			Assert.AreEqual("ButtonClick", createDelegateExpression.MethodName);
		}		
	}
}

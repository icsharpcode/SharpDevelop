// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that a TextBox on a form in python source code is
	/// correctly converted to a code dom that the forms designer
	/// can use by the PythonDesignerCodeDomGenerator class.
	/// 
	/// A code statement like:
	/// 
	/// textBox1.Name = 'textBox'
	/// 
	/// should be converted to the code dom:
	/// 
	/// CodeAssignStatement
	/// 	Left: CodePropertyReferenceExpression
	/// 		TargetObject: CodeFieldReferenceExpression
	/// 			FieldName: textBox1
	/// 			TargetObject: CodeThisReferenceExpression
	/// 	Right: CodePrimitiveExpression
	/// 		Value: textbox1
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class TextBoxOnFormTestFixture
	{
		CodeCompileUnit unit;
		CodeTypeDeclaration mainFormDeclaration;
		CodeMemberField textBoxField;
		CodeAssignStatement textBoxNameAssignStatement;
		CodeMemberMethod initializeComponentMethod;
		int fieldCount;
		CodePropertyReferenceExpression textBoxNamePropertyRef;
		CodePrimitiveExpression textBoxNamePrimitiveExpression;
		CodeAssignStatement textBoxLocationAssignStatement;
		
		string python = "import System.Drawing\r\n" +
						"import System.Windows.Forms\r\n" +
						"\r\n" +
						"from System.Windows.Forms import *\r\n" +
						"from System.Drawing import *\r\n" +
						"\r\n" +
						"class MainForm(Form):\r\n" +
						"\tdef __init__(self):\r\n" +
						"\t\tself.InitializeComponent()\r\n" +
						"\r\n" +
						"\tdef InitializeComponent(self):\r\n" +
						"\t\tself._textBox1 = System.Windows.Forms.TextBox()\r\n" +
						"\t\tself._textBox1.Name = 'textBox1'\r\n" +
						"\t\tself._textBox1.Location = System.Drawing.Point(71, 62)\r\n" +
						"\t\tself.Controls.Add(self._textBox1)\r\n" +
						"\t\tself.Name = 'MainForm'";
			
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			unit = PythonDesignerCodeDomGenerator.Parse(new StringReader(python));
			
			// Find main form.
			foreach (CodeNamespace ns in unit.Namespaces) {
				foreach (CodeTypeDeclaration typeDec in ns.Types) {
					if (typeDec.Name == "MainForm") {
						mainFormDeclaration = typeDec;
					}
				}
			}	
			
			if (mainFormDeclaration != null) {
				foreach (CodeTypeMember member in mainFormDeclaration.Members) {
					CodeMemberField field = member as CodeMemberField;
					CodeMemberMethod method = member as CodeMemberMethod;
					if (field != null) {
						fieldCount++;
						if (field.Name == "textBox1") {
							textBoxField = field;
						}
					} else if (method != null && method.Name == "InitializeComponent") {
						initializeComponentMethod = method;
						if (initializeComponentMethod.Statements.Count > 2) {
							// textBox1.Name statement.
							textBoxNameAssignStatement = initializeComponentMethod.Statements[1] as CodeAssignStatement;
							textBoxNamePropertyRef = textBoxNameAssignStatement.Left as CodePropertyReferenceExpression;
							textBoxNamePrimitiveExpression = textBoxNameAssignStatement.Right as CodePrimitiveExpression;
						
							// textBox1.Location statement.
							textBoxLocationAssignStatement = initializeComponentMethod.Statements[2] as CodeAssignStatement;
						}
					}
				}
			}
		}
				
		[Test]
		public void MainFormExists()
		{
			Assert.IsNotNull(mainFormDeclaration);
		}
		
		[Test]
		public void MainFormBaseTypeIsFullyQualified()
		{
			CodeTypeReference reference = mainFormDeclaration.BaseTypes[0];
			Assert.AreEqual("System.Windows.Forms.Form", reference.BaseType);
		}
		
		[Test]
		public void TextBoxFieldExists()
		{
			Assert.IsNotNull(textBoxField);
		}
		
		[Test]
		public void TextBoxFieldCodeTypeReferenceIsTextBoxType()
		{
			Assert.AreEqual("System.Windows.Forms.TextBox", textBoxField.Type.BaseType);
		}

		[Test]
		public void TextBoxNameAssignStatementExists()
		{
			Assert.IsNotNull(textBoxNameAssignStatement);
		}
		
		[Test]
		public void TextBoxNameAssignStatementLhsIsPropertyReference()
		{
			Assert.IsInstanceOfType(typeof(CodePropertyReferenceExpression), textBoxNameAssignStatement.Left);
		}
		
		[Test]
		public void MainFormHasOneField()
		{
			Assert.AreEqual(1, fieldCount);
		}
		
		[Test]
		public void TextBoxNamePropertyReferenceTargetObjectIsCodeFieldReference()
		{
			Assert.IsInstanceOfType(typeof(CodeFieldReferenceExpression), textBoxNamePropertyRef.TargetObject);
		}
		
		[Test]
		public void TextBoxNameTargetObjectFieldRefName()
		{
			CodeFieldReferenceExpression fieldRef = textBoxNamePropertyRef.TargetObject as CodeFieldReferenceExpression;
			Assert.AreEqual("textBox1", fieldRef.FieldName);
		}
		
		[Test]
		public void TextBoxNamePropertyReferenceName()
		{
			Assert.AreEqual("Name", textBoxNamePropertyRef.PropertyName);
		}
		
		[Test]
		public void TextBoxAssignStatementRhsIsCodePrimitiveExpression()
		{
			Assert.IsInstanceOfType(typeof(CodePrimitiveExpression), textBoxNameAssignStatement.Right);
		}
		
		[Test]
		public void TextBoxNamePrimitiveExpression()
		{
			Assert.AreEqual("textBox1", textBoxNamePrimitiveExpression.Value as string);
		}
		
		[Test]
		public void FieldRefTargetObjectIsCodeThisReferenceExpression()
		{
			CodeFieldReferenceExpression fieldRef = textBoxNamePropertyRef.TargetObject as CodeFieldReferenceExpression;
			Assert.IsInstanceOfType(typeof(CodeThisReferenceExpression), fieldRef.TargetObject);
		}
		
		[Test]
		public void TextBoxLocationStatementExists()
		{
			Assert.IsNotNull(textBoxLocationAssignStatement);
		}
	}
}

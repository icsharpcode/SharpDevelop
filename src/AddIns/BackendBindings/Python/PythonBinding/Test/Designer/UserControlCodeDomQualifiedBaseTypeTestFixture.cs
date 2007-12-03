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
	/// Tests that the code dom created when a user control is being
	/// designed has the correct fully qualified base type.
	/// </summary>
	[TestFixture]
	public class UserControlCodeDomQualifiedBaseTypeTestFixture
	{
		CodeCompileUnit unit;
		CodeTypeDeclaration userControlDeclaration;
		
		string python = "import System.Drawing\r\n" +
						"import System.Windows.Forms\r\n" +
						"\r\n" +
						"from System.Windows.Forms import *\r\n" +
						"from System.Drawing import *\r\n" +
						"\r\n" +
						"class MyUserControl(UserControl):\r\n" +
						"\tdef __init__(self):\r\n" +
						"\t\tself.InitializeComponent()\r\n" +
						"\r\n" +
						"\tdef InitializeComponent(self):\r\n" +
						"\t\tself._textBox1 = System.Windows.Forms.TextBox()\r\n" +
						"\t\tself._textBox1.Name = 'textBox1'\r\n" +
						"\t\tself._textBox1.Location = System.Drawing.Point(71, 62)\r\n" +
						"\t\tself.Controls.Add(self._textBox1)\r\n" +
						"\t\tself.Name = 'MyUserControl'";
			
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			unit = PythonDesignerCodeDomGenerator.Parse(new StringReader(python));
			
			// Find main form.
			foreach (CodeNamespace ns in unit.Namespaces) {
				foreach (CodeTypeDeclaration typeDec in ns.Types) {
					if (typeDec.Name == "MyUserControl") {
						userControlDeclaration = typeDec;
					}
				}
			}
		}
				
		[Test]
		public void MainFormExists()
		{
			Assert.IsNotNull(userControlDeclaration);
		}
		
		[Test]
		public void UserControlBaseTypeIsFullyQualified()
		{
			CodeTypeReference reference = userControlDeclaration.BaseTypes[0];
			Assert.AreEqual("System.Windows.Forms.UserControl", reference.BaseType);
		}
	}
}

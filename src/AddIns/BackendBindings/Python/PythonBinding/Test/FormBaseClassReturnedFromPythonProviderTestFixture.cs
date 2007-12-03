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
using IronPython.CodeDom;
using NUnit.Framework;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Tests that a Form's base class is fully qualifed when
	/// the PythonProvider parses the code.
	/// </summary>
	[TestFixture]
	[Ignore("IronPython PythonProvider implementation is most likely by design.")]
	public class FormBaseClassReturnedFromPythonProviderTestFixture
	{
		CodeCompileUnit codeCompileUnit;
		CodeTypeDeclaration formClass;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonProvider provider = new PythonProvider();
			codeCompileUnit = provider.Parse(new StringReader(GetFormCode()));
			foreach (CodeNamespace n in codeCompileUnit.Namespaces) {
				foreach (CodeTypeDeclaration typeDecl in n.Types) {
					foreach (CodeTypeMember m in typeDecl.Members) {
						if (m.Name == "InitializeComponent") {
							formClass = typeDecl;
							break;
						}
					}
				}
			}
		}

		[Test]
		public void FormClassFound()
		{
			Assert.IsNotNull(formClass);	
		}
		
		[Test]
		public void BaseClassIsFullyQualified()
		{
			CodeTypeReference reference = formClass.BaseTypes[0];
			Assert.AreEqual("System.Windows.Forms.Form", reference.BaseType);
		}
		
		string GetFormCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponent(self):\r\n" +
					"\t\tpass\r\n"; 
		}		
	}
}

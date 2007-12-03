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
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using IronPython.CodeDom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// The PythonProvider that is part of IronPython does not resolve
	/// the Form base class to the fully qualified name: System.Windows.Forms.Form
	/// so the PythonDesignerLoader fixes this when the Parse is called.
	/// </summary>
	[TestFixture]
	public class FormCodeTypeRefHasFullyQualifiedBaseClassTestFixture
	{
		CodeCompileUnit codeCompileUnit;
		CodeTypeDeclaration formClass;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonDesignerGenerator generator = new PythonDesignerGenerator();
			DocumentFactory factory = new DocumentFactory();
			IDocument doc = factory.CreateDocument();
			doc.TextContent = GetFormCode();
			DerivedPythonDesignerLoader loader = new DerivedPythonDesignerLoader(doc, generator);
			codeCompileUnit = loader.CallParse();
			
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
